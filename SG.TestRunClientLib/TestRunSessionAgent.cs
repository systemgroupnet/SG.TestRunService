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
        private readonly string _project;
        private IReadOnlyList<TestCaseRequest> _testCaseRequests;
        private IReadOnlyList<TestCaseInfo> _testsToRun;

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
            _project = session.ProductBuild.TeamProject;
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
            _testCaseRequests = tests.ToList();
            var azureTestCaseIds = new HashSet<int>(await _client.GetAzureTestCaseIdsAsync(_project));
            var newTestCases = _testCaseRequests.Where(t => !azureTestCaseIds.Contains(t.AzureTestCaseId));
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

        public async Task<IReadOnlyList<TestCaseInfo>> GetTestsToRun()
        {
            if (_testCaseRequests == null)
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
                        return await GetAllTestCaseInfos();
                    default:
                        throw new NotSupportedException($"The value '{_configuration.RunForOlderVersionBeahvior}' for configuration '{nameof(_configuration.RunForOlderVersionBeahvior)}' is not valid.");
                }
            }
            var changedFiles = _devOpsServerHandle.GetChangedFiles(
                _project,
                _session.ProductBuild.AzureBuildDefinitionId,
                baseSourceVersion, currentSourceVersion);
            return await PublishChangesAndGetTestsToRun(changedFiles);
        }

        private async Task<IReadOnlyList<TestCaseInfo>> GetAllTestCaseInfos()
        {
            if (_testsToRun != null)
                return _testsToRun;
            if (_testCaseRequests == null || _testCaseRequests.Count == 0)
                return new List<TestCaseInfo>();
            var testCaseResponses = await _client.GetTestCases(_project,
                new[]
                {
                    nameof(TestCaseResponse.Id),
                    nameof(TestCaseResponse.AzureTestCaseId)
                });

            var testCaseAzureIdsToServiceIds = testCaseResponses.ToDictionary(t => t.AzureTestCaseId, t => t.Id);

            List<TestCaseInfo> result = new List<TestCaseInfo>();
            foreach (var tc in _testCaseRequests)
                result.Add(
                    new TestCaseInfo(
                        testCaseAzureIdsToServiceIds[tc.AzureTestCaseId],
                        tc.AzureTestCaseId, tc.Title, null));
            _testsToRun = result;
            return _testsToRun;
        }

        private async Task<IReadOnlyList<TestCaseInfo>> PublishChangesAndGetTestsToRun(IEnumerable<string> changedFiles)
        {
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                TestRunSessionId = _session.Id,
                Changes = changedFiles.Select(f => new CodeSignature(f, CalculateSignature(f))).ToList()
            };
            var response = await _client.PublishImpactChangesAsync(req);
            var testsToRun = new List<TestCaseInfo>();
            var azureIdToTestCases = _testCaseRequests.ToDictionary(t => t.AzureTestCaseId);
            foreach (var tr in response.TestsToRun)
            {
                if (azureIdToTestCases.TryGetValue(tr.AzureTestCaseId, out var testCase))
                {
                    testsToRun.Add(
                        new TestCaseInfo(
                            tr.Id, tr.AzureTestCaseId,
                            testCase.Title, tr.RunReason));
                }
            }
            _testsToRun = testsToRun;
            return _testsToRun;
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
