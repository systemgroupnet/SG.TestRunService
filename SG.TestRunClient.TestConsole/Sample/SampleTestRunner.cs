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
        readonly string project = "Sample";
        readonly int productBuildDefinitionId = 10;
        readonly int productBuildId = 1;
        readonly string productBuildNumber = "SampleBuild.Dvp_1";
        readonly int testBuildId = 1;
        readonly string testBuildNumber = "SampleTest.Dvp_1";
        readonly string suite = "Dvp";
        readonly int sourceVersion = 130;

        TestRunSessionAgent _agent;
        IList<TestCase> _suiteTestCases;
        readonly SampleAzureDevopsHandle _devOpsServerHandle = new SampleAzureDevopsHandle();

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
            _agent = await TestRunSessionFactory.StartAsync(
                _devOpsServerHandle,
                build, suite, testBuildId, testBuildNumber); ;

            _suiteTestCases = GetTestCases();
            await _agent.IntroduceTestCases(CreateTestCaseRequests(_suiteTestCases));
            var testsToRun = await _agent.GetTestsToRun();
            RunTests(testsToRun);
        }

        public void RunTests(IEnumerable<TestCaseRequest> tests)
        {

        }

        public static IList<TestCase> GetTestCases()
        {
            return new List<TestCase>()
            {
                new TestCase(200, "بررسی 1", "/Tests/1.sgts"),
                new TestCase(201, "تست بررسی 2", "/Tests/2.sgts"),
            };
        }

        private IList<TestCaseRequest> CreateTestCaseRequests(IEnumerable<TestCase> testCases)
        {
            return testCases
                .Select(tc =>
                    new TestCaseRequest(project, tc.AzureId, tc.Title)
                    {
                        ExtraData = { ["scriptPath"] = new ExtraDataValue(tc.ScriptPath) }
                    })
                .ToList();
        }
    }
}
