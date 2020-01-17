using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var azureTestCaseIds = new HashSet<int>(await _client.GetAzureTestCaseIdsAsync(_session.TeamProject));
            var newTestCases = tests.Where(t => !azureTestCaseIds.Contains(t.AzureTestCaseId));
            foreach(var tc in newTestCases)
            {
                await _client.InsertTestCaseAsync(tc);
            }
        }
    }
}
