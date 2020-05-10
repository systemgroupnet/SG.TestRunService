using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DalSoft.RestClient;
using DalSoft.RestClient.Handlers;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using SG.TestRunService;
using SG.TestRunService.Common.Models;

namespace SG.TestRunClientLib
{
    public class TestRunClient
    {
        internal const string JsonMediaType = "application/json";

        private readonly string _serviceUri;
        private readonly HttpClient _client;
        private const string ApiResourcePrefix = "api/";

        public TestRunClient(string serviceUri, int timeoutSeconds = 600)
        {
            if (string.IsNullOrWhiteSpace(serviceUri))
                throw new ArgumentException("`serviceUri` cannot be empty.");
            _serviceUri = serviceUri;
            _client = new HttpClient(new RetryHandler());
            _client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
            _client.BaseAddress = new Uri(_serviceUri.TrimEnd('/') + "/");
        }

        public TestRunClient(HttpClient client)
        {
            if (client == null)
                throw new ArgumentException("`client` argument should not be null");
            _client = client;
        }

        private async Task<object> SendAsync(HttpMethod method, string resource, object content = null, Type returnType = null)
        {
            var apiResource = ApiResourcePrefix + resource;
            var message = new HttpRequestMessage(method, apiResource);
            if (content != null)
                message.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, JsonMediaType);
            var response = await _client.SendAsync(message);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(responseContent))
                    return null;
                if (returnType == null)
                    return responseContent;

                if (response.Content.Headers.ContentType.MediaType.Equals(JsonMediaType, StringComparison.OrdinalIgnoreCase))
                {
                    return JsonConvert.DeserializeObject(responseContent, returnType);
                }
                throw new Exception("Response content is not JSON. The response is:\r\n" + responseContent);
            }
            var msg = $"An error occurred while sending request to Uri '{apiResource}'.\r\nThe status code is: {(int)response.StatusCode}.";
            if (!string.IsNullOrWhiteSpace(responseContent))
                msg += "\r\nThe response body is:\r\n" + responseContent;
            throw new Exception(msg);
        }

        private async Task<T> SendAsync<T>(HttpMethod method, string resource, object content = null)
        {
            return (T)await SendAsync(method, resource, content, typeof(T));
        }

        public async Task<IReadOnlyList<TestRunSessionResponse>> GetSessionsAsync()
        {
            return await SendAsync<IReadOnlyList<TestRunSessionResponse>>(HttpMethod.Get, "sessions");
        }

        public async Task<TestRunSessionResponse> GetSessionAsync(int id)
        {
            return await SendAsync<TestRunSessionResponse>(HttpMethod.Get, "sessions/" + id);
        }

        public async Task<TestRunSessionResponse> InsertSessionAsync(TestRunSessionRequest session)
        {
            return await SendAsync<TestRunSessionResponse>(HttpMethod.Post, "sessions", session);
        }

        public async Task<IReadOnlyList<int>> GetAzureTestCaseIdsAsync()
        {
            return await SendAsync<IReadOnlyList<int>>(HttpMethod.Get, "testcases?fields=" + nameof(TestCaseResponse.AzureTestCaseId));
        }

        public async Task<IReadOnlyList<TestCaseResponse>> GetTestCasesAsync(IEnumerable<string> fieldNames)
        {
            return await SendAsync<IReadOnlyList<TestCaseResponse>>(HttpMethod.Get, "testcases?fields=" + string.Join(",", fieldNames));
        }

        public async Task<TestCaseResponse> InsertTestCasesAsync(IEnumerable<TestCaseRequest> testCaseRequests)
        {
            return await SendAsync<TestCaseResponse>(HttpMethod.Post, "testcases", testCaseRequests);
        }

        public async Task<TestCaseResponse> InsertTestCaseAsync(TestCaseRequest testCaseRequest)
        {
            return await SendAsync<TestCaseResponse>(HttpMethod.Post, "testcases", testCaseRequest);
        }

        public async Task<LastImpactUpdateResponse> GetLastImpactUpdateAsync(int azureProductBuildDefId)
        {
            return await SendAsync<LastImpactUpdateResponse>(HttpMethod.Get, "impact/lastUpdate/" + azureProductBuildDefId);
        }

        public async Task<PublishImpactChangesResponse> PublishImpactChangesAsync(
            PublishImpactChangesRequest impactChangeRequest)
        {
            return await SendAsync<PublishImpactChangesResponse>(HttpMethod.Post, "impact/changes", impactChangeRequest);
        }

        public async Task<TestRunResponse> InsertTestRunAsync(int sessionId, TestRunRequest testRunRequest)
        {
            return await SendAsync<TestRunResponse>(HttpMethod.Post, $"sessions/{sessionId}/runs", testRunRequest);
        }

        public async Task<TestRunSessionResponse> PatchTestRunSessionAsync(int sessionId, JsonPatchDocument<TestRunSessionRequest> patch)
        {
            return await SendAsync<TestRunSessionResponse>(new HttpMethod("PATCH"), $"sessions/{sessionId}", patch);
        }

        public async Task<TestRunResponse> PatchTestRunAsync(int sessionId, int testRunId, JsonPatchDocument<TestRunRequest> patch)
        {
            return await SendAsync<TestRunResponse>(new HttpMethod("PATCH"), $"sessions/{sessionId}/runs/{testRunId}", patch);
        }

        public async Task UpdateTestImpactAsync(int testCaseId, TestCaseImpactUpdateRequest request)
        {
            await SendAsync(HttpMethod.Post, "impact/testrun/" + testCaseId, request);
        }

        public async Task<IReadOnlyCollection<TestLastStateResponse>> GetTestLastStatesAsync(int testCaseId)
        {
            return await SendAsync<IReadOnlyCollection<TestLastStateResponse>>(HttpMethod.Get, "impact/lastState/" + testCaseId);
        }

        public async Task UpdateTestLastStateAsync(int testCaseId, TestLastStateUpdateRequest request)
        {
             await SendAsync<IReadOnlyCollection<TestLastStateResponse>>(HttpMethod.Post, "impact/lastState/" + testCaseId, request);
        }
    }
}

