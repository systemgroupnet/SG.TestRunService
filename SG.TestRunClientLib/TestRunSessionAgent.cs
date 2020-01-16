using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunClientLib
{
    public class TestRunSessionAgent
    {
        TestRunClient _client;
        TestRunSessionResponse _session;

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

        public void SetTests(IEnumerable<TestRequest> tests)
        {

        }
    }
}
