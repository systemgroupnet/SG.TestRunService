using SG.TestRunClientLib;
using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunClient.TestConsole
{
    public class SampleTestRunner
    {
        string project = "Sample";
        int productBuildDefinitionId = 10;
        int productBuildId = 1;
        string productBuildNumber = "SampleBuild.Dvp_1";
        int testBuildDefinitionId = 11;
        int testBuildId = 1;
        string testBuildNumber = "SampleTest.Dvp_1";
        string suite = "Dvp";
        int sourceVersion = 130;

        public async Task RunAsync()
        {
            var agent = await TestRunSessionFactory.StartAsync(project, productBuildDefinitionId, productBuildId,
                productBuildNumber, testBuildId, testBuildNumber, suite, sourceVersion.ToString());
            await agent.IntorduceTestCases(GetTestCases());
        }

        public IList<TestCaseRequest> GetTestCases()
        {
            return new List<TestCaseRequest>()
            {
                new TestCaseRequest(project, 200, "بررسی 1")
                {
                    ExtraData = { ["scriptPath"] = new ExtraDataValue("/Tests/1.sgts") }
                },
                new TestCaseRequest(project, 201, "تست بررسی 2")
                {
                    ExtraData = { ["scriptPath"] = new ExtraDataValue("/Tests/2.sgts") }
                },
            };
        }
    }
}
