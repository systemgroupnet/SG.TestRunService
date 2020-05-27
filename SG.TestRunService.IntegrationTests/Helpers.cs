using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.IntegrationTests
{
    public static class Helpers
    {
        public const string AgentMachineName = "test-server300";
        public const string AgentMachineUrl = "http://test:8080/proj1/agents/869";
        public const string FixtureSessionId = "{339b496b-2fde-45b6-b4be-c7617dee2d4a}";
        public const string TeamProject1 = "INV";
        public const string TeamProject2 = "Sales";

        public static TestRunSessionRequest CreateSampleTestRunSessionRequst()
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

        public static IReadOnlyList<TestCaseRequest> CreateSampleTestCases()
        {
            return new[]
            {
                new TestCaseRequest()
                {
                    AzureTestCaseId = 240,
                    TeamProject = TeamProject1,
                    Title = "test case number 240"
                },
                new TestCaseRequest()
                {
                    AzureTestCaseId = 241,
                    TeamProject = TeamProject2,
                    Title = "test case number 241"
                }
            };
        }

        public static IEnumerable<TestRunRequest> CreateSampleTestRunRequests(IEnumerable<TestCaseResponse> testCases)
        {
            return testCases.Select(tc =>
                new TestRunRequest()
                {
                    StartTime = DateTime.Now.AddMinutes(-10),
                    FinishTime = DateTime.Now.AddMinutes(-9),
                    State = TestRunState.Finished,
                    Outcome = TestRunOutcome.Successful,
                    TestCaseId = tc.Id,
                    ExtraData = new Dictionary<string, ExtraDataValue>()
                    {
                        ["FixtureWait"] = new ExtraDataValue("20")
                    }
                });
        }
    }
}
