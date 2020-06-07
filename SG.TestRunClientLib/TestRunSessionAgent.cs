using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunClientLib
{
    public class TestRunSessionAgent
    {
        private readonly TestRunClient _client;
        private readonly ITestRunClientConfiguration _configuration;
        private readonly TestRunSessionResponse _session;
        private readonly string _project;
        private IReadOnlyList<TestCaseRequest> _testCaseRequests;
        private IReadOnlyList<TestCaseInfo> _testsToRun;
        private readonly ILogger _logger;

        private static readonly JsonSerializerSettings _logSerializerSettings = CreateLogSerializerSettings();

        private TestRunSessionAgent(
            TestRunClient client,
            ITestRunClientConfiguration configuration,
            TestRunSessionResponse session,
            ILogger logger)
        {
            _client = client;
            _configuration = configuration;
            _session = session;
            _project = session.ProductBuild.TeamProject;
            _logger = logger;
        }

        public static async Task<TestRunSessionAgent> CreateAsync(
            ITestRunClientConfiguration configuration,
            TestRunSessionRequest sessionRequest,
            ILogger logger = null)
        {
            logger = logger ?? new NullLogger();
            var client = new TestRunClient(configuration.TestRunServiceUrl);
            logger.Info("'TestRunClient' created.");

            var response = await client.InsertSessionAsync(sessionRequest);
            logger.Debug("'TestRunSession' inserted: " + ObjToString(response));

            return new TestRunSessionAgent(client, configuration, response, logger);
        }

        public async Task IntroduceTestCasesAsync(IEnumerable<TestCaseRequest> tests)
        {
            _testCaseRequests = tests.ToList();
            _logger.Info("Total test cases: " + _testCaseRequests.Count);
            var azureTestCaseIds = new HashSet<int>(await _client.GetAzureTestCaseIdsAsync());
            var newTestCases = _testCaseRequests
                .Where(t => !azureTestCaseIds.Contains(t.AzureTestCaseId))
                .ToList();
            if (newTestCases.Count > 0)
            {
                _logger.Info("New test cases: " + newTestCases.Count);
                await _client.InsertTestCasesAsync(newTestCases);
            }
        }

        private async Task<BuildInfo> GetBaseBuildAsync()
        {
            var build = _session.ProductBuild;
            var lastUpdate = await _client.GetLastImpactUpdateAsync(build.AzureBuildDefinitionId);
            if (lastUpdate == null)
            {
                LogDebug("No previous impact info is available for pipeline " + build.AzureBuildDefinitionId);
                return null;
            }
            LogDebug("Previous update information:", lastUpdate);
            return lastUpdate.ProductBuild;
        }

        public async Task<IReadOnlyList<TestCaseInfo>> GetTestsToRunAsync(bool runAllTests = false)
        {
            if (_testCaseRequests == null)
                throw new InvalidOperationException($"Test cases are not available. Call `{nameof(IntroduceTestCasesAsync)}` first.");

            if (_testsToRun != null)
            {
                _logger.Debug("`_testsToRun` was already set. Count: " + _testsToRun.Count);
                return _testsToRun;
            }

            var response = await _client.GetTestsToRun(_session.ProductBuild.AzureBuildDefinitionId, runAllTests);
            var azureIdToTestCases = _testCaseRequests.ToDictionary(t => t.AzureTestCaseId);
            var testsToRun = new List<TestCaseInfo>();
            foreach (var tr in response)
            {
                if (azureIdToTestCases.TryGetValue(tr.AzureTestCaseId, out var testCase))
                {
                    testsToRun.Add(
                        new TestCaseInfo(
                            tr.TestCaseId, tr.AzureTestCaseId,
                            testCase.Title, tr.RunReason));
                }
            }

            LogTestsToRun(testsToRun);

            _testsToRun = testsToRun;
            return _testsToRun;
        }

        public async Task<IReadOnlyList<TestRunResponse>> RecordSessionTestsAsync(IReadOnlyCollection<TestCaseInfo> testCases)
        {
            var responses = new List<TestRunResponse>();

            foreach (var testCase in testCases)
            {
                var testRunResponse = await _client.InsertTestRunAsync(
                        _session.Id,
                        new TestRunRequest()
                        {
                            TestCaseId = testCase.Id,
                            State = TestRunState.NotStarted
                        });
                responses.Add(testRunResponse);
                testCase.TestRunId = testRunResponse.Id;
            }
            return responses;
        }

        private void LogTestsToRun(IReadOnlyCollection<TestCaseInfo> testCases)
        {
            StringBuilder sb = new StringBuilder();
            var symbolStr = new string('=', 20);
            var headerLine = symbolStr + $"   Tests to run (total {testCases.Count} tests)   " + symbolStr;
            sb.AppendLine().AppendLine(headerLine);
            foreach (var testCase in testCases)
                sb.AppendLine(testCase.ToString());
            sb.AppendLine(new string('=', headerLine.Length));
            if (testCases.Count > 0)
            {
                var reasons = string.Join(",",
                    testCases.GroupBy(t => t.RunReason).Select(g => g.Count().ToString() + " " + g.Key));
                sb.Append("Reasons: ").AppendLine(reasons);
            }
            _logger.Info(sb.ToString());
        }

        public async Task<TestRunResponse> StartTestRunAsync(TestCaseInfo testCase, TestRunState state)
        {
            if (_session.State == TestRunSessionState.NotStarted)
            {
                await SetSessionStateAsync(TestRunSessionState.Running);
            }
            _logger.Debug($"Start testing test case {testCase.AzureTestCaseId}, State: {state}");
            if (testCase.TestRunId == 0)
            {
                var testRunResponse = await _client.InsertTestRunAsync(
                        _session.Id,
                        new TestRunRequest()
                        {
                            TestCaseId = testCase.Id,
                            StartTime = DateTime.Now,
                            State = state,
                        });
                testCase.TestRunId = testRunResponse.Id;
                return testRunResponse;
            }
            else
            {
                var patch = new JsonPatchDocument<TestRunRequest>();
                patch.Add(r => r.StartTime, DateTime.Now);
                patch.Add(r => r.State, state);
                return await _client.PatchTestRunAsync(_session.Id, testCase.TestRunId, patch);
            }
        }

        public Task<TestRunResponse> AdvanceTestRunStateAsync(TestCaseInfo testCase, TestRunState state)
        {
            if (state == TestRunState.NotStarted)
                throw new ArgumentException($"Test run should have already been started. State cannot be {state}.");
            if (state == TestRunState.Finished)
                throw new ArgumentException($"Finishing test run should be recorded with `{nameof(RecordTestRunEndAsync)}`.");
            return SetTestRunStateAsync(testCase, state);
        }

        public async Task<TestRunResponse> RecordTestRunEndAsync(TestCaseInfo testCase, TestRunOutcome outcome,
            string errorMessage, IEnumerable<string> impactFiles, IEnumerable<string> impactMethods)
        {
            var patch = new JsonPatchDocument<TestRunRequest>();
            patch.Add(r => r.State, TestRunState.Finished);
            patch.Add(r => r.Outcome, outcome);
            patch.Add(r => r.FinishTime, DateTime.Now);
            patch.Add(r => r.ErrorMessage, errorMessage);
            var runResponse = await _client.PatchTestRunAsync(_session.Id, testCase.TestRunId, patch);

            _logger.Info("Test run finished: " + ObjToString(runResponse));

            List<CodeSignature> codeSignatures = new List<CodeSignature>();
            if (impactFiles != null)
                codeSignatures.AddRange(
                    impactFiles.Select(f => new CodeSignature(f, CodeSignatureUtils.CalculateSignature(f), CodeSignatureType.File)));
            if (impactMethods != null)
                codeSignatures.AddRange(
                    impactMethods.Select(f => new CodeSignature(f, CodeSignatureUtils.CalculateSignature(f), CodeSignatureType.Method)));

            var impactRequest = new TestCaseImpactUpdateRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                CodeSignatures = codeSignatures
            };

            var impactDataCount = codeSignatures.Count;

            LogDebug("Updating test impact information. Number of files/methods (code signatures): " + impactDataCount);
            if (impactDataCount == 0)
                LogDebug("(No impact data is available)");

            await _client.UpdateTestImpactAsync(testCase.Id, impactRequest);

            var lastStateRequest = new TestLastStateUpdateRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                TestRunSessionId = _session.Id,
                Outcome = outcome
            };

            LogDebug($"Updating test last state. Azure Test Case Id: {testCase.AzureTestCaseId}, Outcome: {outcome}");

            await _client.UpdateTestLastStateAsync(testCase.Id, lastStateRequest);

            return runResponse;
        }

        public async Task RecordTestSessionEndAsync(TestRunSessionState state)
        {
            var sessionPatch = new JsonPatchDocument<TestRunSessionRequest>();
            sessionPatch.Add(s => s.State, state);
            sessionPatch.Add(s => s.FinishTime, DateTime.Now);
            var response = await _client.PatchTestRunSessionAsync(_session.Id, sessionPatch);
            _session.State = response.State;
            _session.FinishTime = response.FinishTime;

            LogDebug("Test session finished:", response);
        }

        private Task<TestRunResponse> SetTestRunStateAsync(TestCaseInfo testCase, TestRunState state)
        {
            var patch = new JsonPatchDocument<TestRunRequest>();
            patch.Add(r => r.State, state);
            _logger.Debug($"Updating test case state: {testCase.AzureTestCaseId} => {state}");
            return _client.PatchTestRunAsync(_session.Id, testCase.TestRunId, patch);
        }

        private async Task SetSessionStateAsync(TestRunSessionState state)
        {
            var sessionPatch = new JsonPatchDocument<TestRunSessionRequest>();
            sessionPatch.Add(s => s.State, state);
            var newSession = await _client.PatchTestRunSessionAsync(_session.Id, sessionPatch);
            _logger.Debug("Session state updated to " + newSession.State);
            _session.State = newSession.State;
        }

        public async Task AddExtraDataAsync(IDictionary<string, ExtraDataValue> extraDataValues)
        {
            var patch = HttpHelper.CreateJsonPatchToAddOrUpdateExtraData<TestRunSessionRequest>(extraDataValues);
            var newSession = await _client.PatchTestRunSessionAsync(_session.Id, patch);
            _logger.Debug("Session extra data updated.");
            foreach (var ex in newSession.ExtraData)
            {
                _session.ExtraData[ex.Key] = ex.Value;
            }
        }

        public async Task AddRunExtraDataAsync(int runId, IDictionary<string, ExtraDataValue> extraDataValues)
        {
            var patch = HttpHelper.CreateJsonPatchToAddOrUpdateExtraData<TestRunRequest>(extraDataValues);
            await _client.PatchTestRunAsync(_session.Id, runId, patch);
        }

        private void LogDebug(string text, object obj = null)
        {
            if (_logger.IsEnabled)
            {
                _logger.Debug(text + (obj != null ? ObjToString(obj) : string.Empty));
            }
        }

        private static JsonSerializerSettings CreateLogSerializerSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }

        private static string ObjToString(object obj)
        {
            return obj != null
                ? Environment.NewLine + JsonConvert.SerializeObject(obj, _logSerializerSettings)
                : string.Empty;
        }
    }
}
