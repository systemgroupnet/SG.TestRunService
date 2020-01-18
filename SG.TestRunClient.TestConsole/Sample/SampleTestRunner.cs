using SG.TestRunClientLib;
using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunClient.TestConsole.Sample
{
    public class SampleTestRunner
    {
        string project = "Sample";
        int productBuildDefinitionId = 10;
        int productBuildId = 1;
        string productBuildNumber = "SampleBuild.Dvp_1";
        int testBuildId = 1;
        string testBuildNumber = "SampleTest.Dvp_1";
        string suite = "Dvp";
        int sourceVersion = 130;

        TestRunSessionAgent _agent;
        IList<TestCase> _allTestCases;

        public async Task RunAsync()
        {
            var build = new BuildInfo()
            {
                TeamProject = project,
                AzureBuildDefinitionId = productBuildDefinitionId,
                AzureBuildId = productBuildId,
                BuildNumber = productBuildNumber,
                Date = new DateTime(2010, 1, 1),
                SourceVersion = sourceVersion.ToString()
            };
            _agent = await TestRunSessionFactory.StartAsync(build, suite, testBuildId, testBuildNumber);
            _allTestCases = GetTestCases();
            await _agent.IntorduceTestCases(CreateTestCaseRequests(_allTestCases));
        }

        public IList<TestCase> GetTestCases()
        {
            return new List<TestCase>()
            {
                new TestCase(project, 200, "بررسی 1", "/Tests/1.sgts"),
                new TestCase(project, 201, "تست بررسی 2", "/Tests/2.sgts"),
            };
        }

        public IList<TestCaseRequest> CreateTestCaseRequests(IEnumerable<TestCase> testCases)
        {
            return testCases
                .Select(tc =>
                    new TestCaseRequest(tc.TeamProject, tc.AzureId, tc.Title)
                    {
                        ExtraData = { ["scriptPath"] = new ExtraDataValue(tc.ScriptPath) }
                    })
                .ToList();
        }
    }
}
