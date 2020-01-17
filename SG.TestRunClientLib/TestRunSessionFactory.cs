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
            string teamProject, int productBuildDefinitionId,
            int productBuildId, string productBuildNumber,
            int testBuildId, string testBuildNumber,
            string suiteName, string sourceVersion)
        {
            TestRunSessionRequest sessionRequest = new TestRunSessionRequest()
            {
                TeamProject = teamProject,
                AzureProductBuildDefinitionId = productBuildDefinitionId,
                AzureProductBuildId = productBuildId,
                AzureProductBuildNumber = productBuildNumber,
                AzureTestBuildId = testBuildId,
                AzureTestBuildNumber = testBuildNumber,
                SourceVersion = sourceVersion,
                SuiteName = suiteName,
                StartTime = DateTime.Now,
                Outcome = TestSessionOutcome.NotStarted,
            };

            return await TestRunSessionAgent.CreateAsync(sessionRequest);
        }
    }
}

