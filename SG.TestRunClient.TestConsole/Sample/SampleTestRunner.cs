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
        readonly int productBuildId = 3;
        readonly string productBuildNumber = "SampleBuild.Dvp_3";
        readonly int testBuildId = 13;
        readonly string testBuildNumber = "SampleTest.Dvp_3";
        readonly string suite = "Dvp";
        readonly int sourceVersion = 132;

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
                new TestRunClientJsonFileConfiguration("appsettings.json"),
                _devOpsServerHandle,
                build, suite, testBuildId, testBuildNumber, new ConsoleLogger());
            _suiteTestCases = GetTestCases();
            await _agent.IntroduceTestCasesAsync(CreateTestCaseRequests(_suiteTestCases));
            var testsToRun = await _agent.GetTestsToRunAsync();
            await _agent.RecordSessionTestsAsync(testsToRun);
            await RunTests(testsToRun);
            await _agent.RecordTestSessionEndAsync(TestRunSessionState.RanToEnd);
        }

        public async Task RunTests(IReadOnlyList<TestCaseInfo> tests)
        {
            for (int i = 0; i < tests.Count; i++)
            {
                var test = tests[i];
                await _agent.StartTestRunAsync(test, TestRunState.FixtureQueued);
                await Task.Delay(2000);
                await _agent.AdvanceTestRunStateAsync(test, TestRunState.WaitingForWeb);
                await Task.Delay(1000);
                await _agent.AdvanceTestRunStateAsync(test, TestRunState.Running);
                await Task.Delay(3000);
                await _agent.RecordTestRunEndAsync(test, TestRunOutcome.Successful, null, GetTestImpactFiles(test, productBuildDefinitionId));
            }
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

        public static string[] files = new[]
        {
            "$/Dvp/General/Program.cs",
            "$/Dvp/General/Test/Test.cs",
            "$/Dvp/Sales/OrderService.cs",
            "$/Dvp/Sales/IOrderService.cs",
            "$/Dvp/General/Party/PartyService.cs",
        };

        private IEnumerable<string> GetTestImpactFiles(TestCaseInfo testCase, int azureBuildDefId)
        {
            if (azureBuildDefId == 10)
            {
                if (testCase.AzureTestCaseId == 200)
                    return new[] { files[1], files[4] };
                else if (testCase.AzureTestCaseId == 201)
                    return new[] { files[0], files[4] };
            }
            else if (azureBuildDefId == 11)
            {
                if (testCase.AzureTestCaseId == 200)
                    return new[] { files[2], files[3] };
                else if (testCase.AzureTestCaseId == 201)
                    return new[] { files[3], files[4] };
            }
            throw new InvalidOperationException();
        }
    }
}
