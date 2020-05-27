using Microsoft.EntityFrameworkCore.Query.Internal;
using SG.TestRunClientLib;
using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
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


        private const string AgentMachineName = "test-server300";
        private const string AgentMachineUrl = "http://test:8080/proj1/agents/869";
        private const string FixtureSessionId = "{339b496b-2fde-45b6-b4be-c7617dee2d4a}";

        public TestRunSessionsTests(TestingWebAppFactory appFactory)
        {
            _appFactory = appFactory;
            _httpClient = _appFactory.CreateClient();
            _client = new TestRunClient(_httpClient);
        }

        private TestRunSessionRequest CreateSampleRequst()
        {
            return
                new TestRunSessionRequest()
                {
                    AzureTestBuildId = 1101,
                    AzureTestBuildNumber = "Test1.20200203.5",
                    StartTime = DateTime.Now,
                    SuiteName = "Dvp",
                    ProductBuild = new BuildInfo()
                    {
                        AzureBuildDefinitionId = 10,
                        AzureBuildId = 544,
                        BuildNumber = "Build1.20200201.9",
                        Date = DateTime.Now.AddDays(-3),
                        SourceVersion = "4000",
                        TeamProject = "TestTeam",
                    },
                    ExtraData = new Dictionary<string, ExtraDataValue>()
                    {
                        [nameof(AgentMachineName)] = new ExtraDataValue(AgentMachineName, AgentMachineUrl),
                        [nameof(FixtureSessionId)] = new ExtraDataValue(FixtureSessionId, null)
                    }
                };
        }

        [Fact]
        public async void InsertSession_AfterFetched_ShouldMatch()
        {
            var request = CreateSampleRequst();

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
    }
}
