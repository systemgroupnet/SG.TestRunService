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
        readonly TestRunSessionResponse _session;

        private TestRunSessionAgent(TestRunClient client, TestRunSessionResponse session)
        {
            _client = client;
            _session = session;
        }

        public static async Task<TestRunSessionAgent> CreateAsync(TestRunSessionRequest sessionRequest)
        {
            var client = TestRunClientFactory.CreateClient();
            var response = await client.InsertSessionAsync(sessionRequest);
            return new TestRunSessionAgent(client, response);
        }

        public async Task IntorduceTestCases(IEnumerable<TestCaseRequest> tests)
        {
            var azureTestCaseIds = new HashSet<int>(await _client.GetAzureTestCaseIdsAsync(_session.ProductBuild.TeamProject));
            var newTestCases = tests.Where(t => !azureTestCaseIds.Contains(t.AzureTestCaseId));
            foreach (var tc in newTestCases)
            {
                await _client.InsertTestCaseAsync(tc);
            }
        }

        public async Task<string> GetBaseBuildSourceVersionAsync()
        {
            var lastUpdate = await _client.GetLastImpactUpdateAsync(_session.ProductBuild.AzureBuildDefinitionId);
            return lastUpdate.ProductBuild.SourceVersion;
        }

        public async Task PublishChangesAndGetTestsToRun(IEnumerable<string> changedFiles)
        {
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                TestRunSessionId = _session.Id,
                Changes = changedFiles.Select(f => new CodeSignature(f, CalculateSignature(f))).ToList()
            };
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
