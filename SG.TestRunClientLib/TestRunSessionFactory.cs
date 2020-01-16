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
                Azure_ProductBuildDefinitionId = productBuildDefinitionId,
                Azure_ProductBuildId = productBuildId,
                Azure_ProductBuildNumber = productBuildNumber,
                Azure_TestBuildId = testBuildId,
                Azure_TestBuildNumber = testBuildNumber,
                SourceVersion = sourceVersion,
                SuiteName = suiteName,
                StartTime = DateTime.Now,
                Outcome = TestRunService.Common.Data.TestSessionOutcome.NotStarted,
            };

            return await TestRunSessionAgent.CreateAsync(sessionRequest);
        }
    }
}

