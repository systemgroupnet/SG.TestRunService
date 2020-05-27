using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SG.TestRunClientLib;
using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SG.TestRunService.IntegrationTests
{
    public class TestRunSessionsTests : IClassFixture<TestingWebAppFactory>
    {
        private TestingWebAppFactory _appFactory;
        private HttpClient _httpClient;
        private TestRunClient _client;


        public TestRunSessionsTests(TestingWebAppFactory appFactory)
        {
            _appFactory = appFactory;
            _httpClient = _appFactory.CreateClient();
            _client = new TestRunClient(_httpClient);
        }

        [Fact]
        public async void InsertSession_AfterFetched_ShouldMatch()
        {
            var request = Helpers.CreateSampleTestRunSessionRequst();

            var response1 = await _client.InsertSessionAsync(request);
            Assert.True(response1.Id > 0);

            var response2 = await _client.GetSessionAsync(response1.Id);
            Assert.Equal(response1.Id, response2.Id);
            Assert.Equal(response1.StartTime, response2.StartTime);
            Assert.Equal(response1.AzureTestBuildNumber, response2.AzureTestBuildNumber);
            Assert.Equal(response1.AzureTestBuildId, response2.AzureTestBuildId);
            Assert.Equal(response1.SuiteName, response2.SuiteName);
            Assert.Equal(response1.ExtraData.Count, response2.ExtraData.Count);
            foreach (var key in response1.ExtraData.Keys)
            {
                Assert.Equal(response1.ExtraData[key], response2.ExtraData[key]);
            }
        }

        [Fact]
        public async void InsertBasicDataAndUpdateTestRunExtraData()
        {
            var testCaseRequest = Helpers.CreateSampleTestCases();
            await _client.InsertTestCasesAsync(testCaseRequest);
            var testCaseResponses = await _client.GetTestCasesAsync();
            Assert.Equal(testCaseRequest.Count, testCaseResponses.Count);
            foreach (var tc in testCaseResponses)
                Assert.True(tc.Id > 0, "Invalid TestCase.Id");
            var sessionRequest = Helpers.CreateSampleTestRunSessionRequst();
            var sessionResponse = await _client.InsertSessionAsync(sessionRequest);
            Assert.True(sessionResponse.Id > 0, "Invalid TestRunSession.Id");
            var testRunRequests = Helpers.CreateSampleTestRunRequests(testCaseResponses);
            var testRunResponses = new List<TestRunResponse>();
            foreach (var runRequest in testRunRequests)
            {
                var runResponse = await _client.InsertTestRunAsync(sessionResponse.Id, runRequest);
                Assert.True(runResponse.Id > 0, "Invalid TestRunResponse.Id");
                Assert.Equal(runRequest.ExtraData.Count, runResponse.ExtraData.Count);
                foreach (var ed in runRequest.ExtraData)
                    Assert.Equal(ed.Value, runResponse.ExtraData[ed.Key]);
                testRunResponses.Add(runResponse);
            }

            var testRunToUpdate = testRunResponses[0];
            var originalExtraData = testRunToUpdate.ExtraData.ToList();
            var newTestRunExtraDataDict =
                new Dictionary<string, ExtraDataValue>()
                {
                    ["ex2"] = new ExtraDataValue("ex2Value")
                };
            var testRunPatch = HttpHelper.CreateJsonPatchToAddOrUpdateExtraData<TestRunRequest>(newTestRunExtraDataDict);
            var updatedTestRun = await _client.PatchTestRunAsync(
                testRunToUpdate.TestRunSessionId, testRunToUpdate.Id, testRunPatch);
            Assert.Equal(testRunToUpdate.ExtraData.Count + 1, updatedTestRun.ExtraData.Count);
            foreach (var extraData in originalExtraData.Concat(newTestRunExtraDataDict))
                Assert.Equal(extraData.Value, updatedTestRun.ExtraData[extraData.Key]);
        }
    }
}
