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
            _client = new RestClient(new Uri(new Uri(_serviceUri), "api").AbsoluteUri);
        }

        public async Task<IReadOnlyList<TestRunSessionResponse>> GetSessionsAsync()
        {
            return await _client.Sessions.get();
        }

        public async Task<TestRunSessionResponse> GetSessionAsync(int id)
        {
            return await _client.Sessions(id).get();
        }

        public async Task<TestRunSessionResponse> InsertSessionAsync(TestRunSessionRequest session)
        {
            return await _client.Session.Post(session);
        }

        public async Task<IReadOnlyList<int>> GetAzureTestCaseIdsAsync(string teamProject)
        {
            return await _client.TestCases
                .Query(new { project = teamProject, fields = nameof(TestCaseResponse.AzureTestCaseId) })
                .Get();
        }

        public async Task<TestCaseResponse> InsertTestCaseAsync(TestCaseRequest testCaseRequest)
        {
            return await _client.TestCases.Post(testCaseRequest);
        }
    }
}
