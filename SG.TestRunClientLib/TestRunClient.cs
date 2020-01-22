using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DalSoft.RestClient;
using Microsoft.AspNetCore.JsonPatch;
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
            var config = new Config().UseRetryHandler();
            _client = new RestClient(new Uri(new Uri(_serviceUri), "api").AbsoluteUri, config);
        }

        public async Task<IReadOnlyList<TestRunSessionResponse>> GetSessionsAsync()
        {
            return await _client.sessions.Get();
        }

        public async Task<TestRunSessionResponse> GetSessionAsync(int id)
        {
            return await _client.sessions(id).Get();
        }

        public async Task<TestRunSessionResponse> InsertSessionAsync(TestRunSessionRequest session)
        {
            return await _client.sessions.Post(session);
        }

        public async Task<IReadOnlyList<int>> GetAzureTestCaseIdsAsync(string teamProject)
        {
            return await _client.testcases
                .Query(new { project = teamProject, fields = nameof(TestCaseResponse.AzureTestCaseId) })
                .Get();
        }

        public async Task<IReadOnlyList<TestCaseResponse>> GetTestCases(string teamProject, IEnumerable<string> fieldNames)
        {
            return await _client.testcases
                .Query(new { project = teamProject, fields = string.Join(",", fieldNames) })
                .Get();
        }

        public async Task<TestCaseResponse> InsertTestCaseAsync(TestCaseRequest testCaseRequest)
        {
            return await _client.testcases.Post(testCaseRequest);
        }

        public async Task<LastImpactUpdateResponse> GetLastImpactUpdateAsync(int azureProductBuildDefId)
        {
            return await _client.impact.lastUpdate.Get(azureProductBuildDefId);
        }

        public async Task<PublishImpactChangesResponse> PublishImpactChangesAsync(
            PublishImpactChangesRequest impactChangeRequest)
        {
            return await _client.impact.changes.Post(impactChangeRequest);
        }

        public async Task<TestRunResponse> InsertTestRunAsync(int sessionId, TestRunRequest testRunRequest)
        {
            return await _client.sessions(sessionId).runs(testRunRequest);
        }

        public async Task<TestRunResponse> PatchTestRunSession(int sessionId, JsonPatchDocument<TestRunRequest> patch)
        {
            return await _client.sessions(sessionId).Patch(patch);
        }

        public async Task<TestRunResponse> PatchTestRun(int sessionId, int testRunId, JsonPatchDocument<TestRunRequest> patch)
        {
            return await _client.sessions(sessionId).runs(testRunId).Patch(patch);
        }
    }
}

