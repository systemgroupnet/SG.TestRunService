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
            BuildInfo productBuild, string suiteName,
            int testBuildId, string testBuildNumber,
            ILogger logger = null,
            IDictionary<string, ExtraDataValue> extraData = null)
        {
            TestRunSessionRequest sessionRequest = new TestRunSessionRequest()
            {
                ProductBuild = productBuild,
                AzureTestBuildId = testBuildId,
                AzureTestBuildNumber = testBuildNumber,
                SuiteName = suiteName,
                StartTime = DateTime.Now,
                State = TestRunSessionState.NotStarted,
                ExtraData = extraData
            };

            return await TestRunSessionAgent.CreateAsync(configuration, sessionRequest, logger);
        }
    }
}

