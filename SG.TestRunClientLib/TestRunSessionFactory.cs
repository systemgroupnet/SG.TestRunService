using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunClientLib
{
    public static class TestRunSessionFactory
    {
        public static async Task<TestRunSessionAgent> StartAsync(
            ITestRunClientConfiguration configuration,
            IDevOpsServerHandle devOpsServerHandle,
            BuildInfo productBuild, string suiteName,
            int testBuildId, string testBuildNumber)
        {
            TestRunSessionRequest sessionRequest = new TestRunSessionRequest()
            {
                ProductBuild = productBuild,
                AzureTestBuildId = testBuildId,
                AzureTestBuildNumber = testBuildNumber,
                SuiteName = suiteName,
                StartTime = DateTime.Now,
                Outcome = TestSessionOutcome.NotStarted,
            };

            return await TestRunSessionAgent.CreateAsync(configuration,devOpsServerHandle, sessionRequest);
        }
    }
}

