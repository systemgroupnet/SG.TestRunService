using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunClientLib
{
    public class TestRunSessionAgent
    {
        private readonly TestRunClient _client;
        private readonly IDevOpsServerHandle _devOpsServerHandle;
        private readonly ITestRunClientConfiguration _configuration;
        private readonly TestRunSessionResponse _session;
        private Dictionary<int, TestCaseRequest> _testCases;

        private TestRunSessionAgent(
            TestRunClient client,
            ITestRunClientConfiguration configuration,
            IDevOpsServerHandle devOpsServerHandle,
            TestRunSessionResponse session)
        {
            _client = client;
            _configuration = configuration;
            _devOpsServerHandle = devOpsServerHandle;
            _session = session;
        }

        public static async Task<TestRunSessionAgent> CreateAsync(
            ITestRunClientConfiguration configuration,
            IDevOpsServerHandle devOpsServerHandle,
            TestRunSessionRequest sessionRequest)
        {
            var client = new TestRunClient(configuration.TestRunServiceUrl);
            var response = await client.InsertSessionAsync(sessionRequest);
            return new TestRunSessionAgent(client, configuration, devOpsServerHandle, response);
        }

        public async Task IntroduceTestCases(IEnumerable<TestCaseRequest> tests)
        {
            _testCases = tests.ToDictionary(t => t.AzureTestCaseId);
            var azureTestCaseIds = new HashSet<int>(await _client.GetAzureTestCaseIdsAsync(_session.ProductBuild.TeamProject));
            var newTestCases = tests.Where(t => !azureTestCaseIds.Contains(t.AzureTestCaseId));
            foreach (var tc in newTestCases)
            {
                await _client.InsertTestCaseAsync(tc);
            }
        }

        private async Task<string> GetBaseBuildSourceVersionAsync()
        {
            var lastUpdate = await _client.GetLastImpactUpdateAsync(_session.ProductBuild.AzureBuildDefinitionId);
            return lastUpdate.ProductBuild.SourceVersion;
        }

        public async Task<IReadOnlyList<TestCaseRequest>> GetTestsToRun()
        {
            if (_testCases == null)
                throw new InvalidOperationException($"Test cases are not available. Call `{nameof(IntroduceTestCases)}` first.");

            var currentSourceVersion = _session.ProductBuild.SourceVersion;
            var baseSourceVersion = await GetBaseBuildSourceVersionAsync();
            if (baseSourceVersion == currentSourceVersion)
            {
                // issue a warning
            }
            else if (!_devOpsServerHandle.IsChronologicallyAfter(currentSourceVersion, baseSourceVersion))
            {
                switch (_configuration.RunForOlderVersionBeahvior)
                {
                    case TestOlderVersionBehavior.Fail:
                        throw new Exception($"Source version used for this test (${currentSourceVersion})is older than the last test ran on this build definition (${baseSourceVersion}).");
                    case TestOlderVersionBehavior.RunNotSuccessfulTests:
                        return await PublishChangesAndGetTestsToRun(Enumerable.Empty<string>());
                    case TestOlderVersionBehavior.RunImpactedAndNotSuccessfulTests:
                        break;
                    case TestOlderVersionBehavior.RunAllTests:
                        return _testCases.Values.ToList();
                    default:
                        throw new NotSupportedException($"The value '{_configuration.RunForOlderVersionBeahvior}' for configuration '{nameof(_configuration.RunForOlderVersionBeahvior)}' is not valid.");
                }
            }
            var changedFiles = _devOpsServerHandle.GetChangedFiles(
                _session.ProductBuild.TeamProject,
                _session.ProductBuild.AzureBuildDefinitionId,
                baseSourceVersion, currentSourceVersion);
            return await PublishChangesAndGetTestsToRun(changedFiles);
        }

        private async Task<IReadOnlyList<TestCaseRequest>> PublishChangesAndGetTestsToRun(IEnumerable<string> changedFiles)
        {
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                TestRunSessionId = _session.Id,
                Changes = changedFiles.Select(f => new CodeSignature(f, CalculateSignature(f))).ToList()
            };
            var response = await _client.PublishImpactChangesAsync(req);
            var testCasesToRun = new List<TestCaseRequest>();
            foreach (var tr in response.TestsToRun)
            {
                if (_testCases.TryGetValue(tr.AzureTestCaseId, out var tcToRun))
                    testCasesToRun.Add(tcToRun);
            }
            return testCasesToRun;
        }

        private string CalculateSignature(string value)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var hash = sha1.ComputeHash(Encoding.Unicode.GetBytes(value));
            return GetBytesHexString(hash);
        }

        private string GetBytesHexString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture));
            }
            return builder.ToString();
        }
    }
}
