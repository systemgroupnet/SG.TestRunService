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
        readonly TestRunClient _client;
        readonly IDevOpsServerHandle _devOpsServerHandle;
        readonly TestRunSessionResponse _session;
        Dictionary<int, TestCaseRequest> _testCases;

        private TestRunSessionAgent(
            TestRunClient client,
            IDevOpsServerHandle devOpsServerHandle,
            TestRunSessionResponse session)
        {
            _client = client;
            _devOpsServerHandle = devOpsServerHandle;
            _session = session;
        }

        public static async Task<TestRunSessionAgent> CreateAsync(
            IDevOpsServerHandle devOpsServerHandle, TestRunSessionRequest sessionRequest)
        {
            var client = TestRunClientFactory.CreateClient();
            var response = await client.InsertSessionAsync(sessionRequest);
            return new TestRunSessionAgent(client, devOpsServerHandle, response);
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
            }
            else if (_devOpsServerHandle.IsChronologicallyAfter(currentSourceVersion, baseSourceVersion))
            {
            }
            else
            {

            }
            var changedFiles = _devOpsServerHandle.GetChangedFiles(
                _session.ProductBuild.TeamProject,
                _session.ProductBuild.AzureBuildDefinitionId,
                baseSourceVersion, currentSourceVersion);

            var testsToRun = await PublishChangesAndGetTestsToRun(changedFiles);
            var testCasesToRun = new List<TestCaseRequest>();
            foreach (var tr in testsToRun)
            {
                if (_testCases.TryGetValue(tr.AzureTestCaseId, out var tcToRun))
                    testCasesToRun.Add(tcToRun);
            }
            return testCasesToRun;
        }

        private async Task<IReadOnlyList<TestToRunResponse>> PublishChangesAndGetTestsToRun(IEnumerable<string> changedFiles)
        {
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                TestRunSessionId = _session.Id,
                Changes = changedFiles.Select(f => new CodeSignature(f, CalculateSignature(f))).ToList()
            };
            var response = await _client.PublishImpactChangesAsync(req);
            return response.TestsToRun;
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
