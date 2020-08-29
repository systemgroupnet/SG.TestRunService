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
            string productLineKey = null,
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
                ExtraData = extraData,
                ProductLine = new ProductLine()
                {
                    Key = productLineKey ?? ExtractBuildName(productBuild) ?? productBuild.AzureBuildDefinitionId.ToString(),
                    AzureProductBuildDefinitionId = productBuild.AzureBuildDefinitionId
                }
            };

            return await TestRunSessionAgent.CreateAsync(configuration, sessionRequest, logger);
        }

        private static string ExtractBuildName(BuildInfo build)
        {
            var i = build.BuildNumber.LastIndexOf('_');
            if (i > 0)
                return build.BuildNumber.Substring(0, i);
            return null;
        }
    }
}

