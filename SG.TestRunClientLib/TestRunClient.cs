using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DalSoft.RestClient;
using SG.TestRunService;
using SG.TestRunService.Common.Models;

namespace SG.TestRunClientLib
{
    public class TestRunClient
    {
        private readonly string _serviceUri;
        private readonly dynamic _client;

        public TestRunClient(string serviceUri)
        {
            _serviceUri = serviceUri;
            _client = new RestClient(_serviceUri);
        }

        public async Task<IReadOnlyList<TestRunSessionResponse>> GetSessionsAsync()
        {
            return await _client.sessions.get();
        }

        public async Task<TestRunSessionResponse> GetSessionAsync(int id)
        {
            return await _client.sessions.get(id);
        }

        public async Task<TestRunSessionResponse> InsertSessionAsync(TestRunSessionRequest session)
        {
            return await _client.session.post(session);
        }
    }
}
