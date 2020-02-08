using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using SG.TestRunService.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunClientLib
{
    public class TestRunSessionAgent
    {
        private readonly TestRunClient _client;
        private readonly IDevOpsServerHandle _devOpsServerHandle;
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
            IDevOpsServerHandle devOpsServerHandle,
            TestRunSessionResponse session,
            ILogger logger)
        {
            _client = client;
            _configuration = configuration;
            _devOpsServerHandle = devOpsServerHandle;
            _session = session;
            _project = session.ProductBuild.TeamProject;
            _logger = logger;
        }

        public static async Task<TestRunSessionAgent> CreateAsync(
            ITestRunClientConfiguration configuration,
            IDevOpsServerHandle devOpsServerHandle,
            TestRunSessionRequest sessionRequest,
            ILogger logger = null)
        {
            logger = logger ?? new NullLogger();
            var client = new TestRunClient(configuration.TestRunServiceUrl);
            logger.Info("'TestRunClient' created.");

            var response = await client.InsertSessionAsync(sessionRequest);
            logger.Debug("'TestRunSession' inserted: " + ObjToString(response));

            return new TestRunSessionAgent(client, configuration, devOpsServerHandle, response, logger);
        }

        public async Task IntroduceTestCasesAsync(IEnumerable<TestCaseRequest> tests)
        {
            _testCaseRequests = tests.ToList();
            _logger.Info("Total test cases: " + _testCaseRequests.Count);
            var azureTestCaseIds = new HashSet<int>(await _client.GetAzureTestCaseIdsAsync(_project));
            var newTestCases = _testCaseRequests
                .Where(t => !azureTestCaseIds.Contains(t.AzureTestCaseId))
                .ToList();
            if (newTestCases.Count > 0)
            {
                _logger.Info("New test cases: " + newTestCases.Count);
                foreach (var tc in newTestCases)
                {
                    await _client.InsertTestCaseAsync(tc);
                }
            }
        }

        private async Task<string> GetBaseBuildSourceVersionAsync()
        {
            var build = _session.ProductBuild;
            var lastUpdate = await _client.GetLastImpactUpdateAsync(build.AzureBuildDefinitionId);
            if (lastUpdate == null)
            {
                LogDebug("No previous impact info is available for pipeline " + build.AzureBuildDefinitionId);
                return null;
            }
            LogDebug("Previous update information:", lastUpdate);
            return lastUpdate.ProductBuild.SourceVersion;
        }

        public async Task<IReadOnlyList<TestCaseInfo>> GetTestsToRunAsync()
        {
            if (_testCaseRequests == null)
                throw new InvalidOperationException($"Test cases are not available. Call `{nameof(IntroduceTestCasesAsync)}` first.");

            if (_testsToRun != null)
            {
                _logger.Debug("`_testsToRun` was already set. Count: " + _testsToRun.Count);
                return _testsToRun;
            }

            var build = _session.ProductBuild;
            var currentSourceVersion = build.SourceVersion;
            var baseSourceVersion = await GetBaseBuildSourceVersionAsync();
            if (baseSourceVersion == null)
            {
                _logger.Info("This is the first test session for pipeline " + build.AzureBuildDefinitionId + ". All tests will be run.");
                return await PublishNoChangeAndGetAllTestsAsync();
            }
            if (baseSourceVersion == currentSourceVersion)
            {
                _logger.Warn("This test session is being run for the same source version as the previous session: " + currentSourceVersion);
            }
            else if (!_devOpsServerHandle.IsChronologicallyAfter(currentSourceVersion, baseSourceVersion))
            {
                string message = $"Source version used for this test ({currentSourceVersion}) is older than the last test ran on this build definition ({baseSourceVersion}).";
                _logger.Info(message);
                _logger.Debug($"Deciding by configuration '{nameof(_configuration.RunForOlderVersionBeahvior)}': {_configuration.RunForOlderVersionBeahvior}");

                switch (_configuration.RunForOlderVersionBeahvior)
                {
                    case TestOlderVersionBehavior.Fail:
                        throw new Exception(message);
                    case TestOlderVersionBehavior.RunNotSuccessfulTests:
                        return await PublishChangesAndGetTestsToRunAsync(Enumerable.Empty<string>());
                    case TestOlderVersionBehavior.RunImpactedAndNotSuccessfulTests:
                        // default behavior
                        break;
                    case TestOlderVersionBehavior.RunAllTests:
                        return await PublishNoChangeAndGetAllTestsAsync();
                    default:
                        throw new NotSupportedException($"The value '{_configuration.RunForOlderVersionBeahvior}' for configuration '{nameof(_configuration.RunForOlderVersionBeahvior)}' is not valid.");
                }
            }
            var changedFiles = _devOpsServerHandle.GetChangedFiles(
                _project,
                _session.ProductBuild.AzureBuildDefinitionId,
                baseSourceVersion, currentSourceVersion);
            LogChangedFiles(currentSourceVersion, baseSourceVersion, changedFiles);
            return await PublishChangesAndGetTestsToRunAsync(changedFiles);
        }

        private void LogChangedFiles(string currentSourceVersion, string baseSourceVersion, IReadOnlyList<string> changedFiles)
        {
            if (_logger.IsEnabled)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Changed files between source version {baseSourceVersion} and {currentSourceVersion}:");
                sb.AppendLine("===============================================================================");
                foreach (var c in changedFiles)
                {
                    sb.AppendLine(c);
                }
                sb.AppendLine("===============================================================================");
                _logger.Info(sb.ToString());
                _logger.Info("Total changed files: " + changedFiles.Count);
            }
        }

        public async Task<IReadOnlyList<TestRunResponse>> RecordSessionTestsAsync(IReadOnlyCollection<TestCaseInfo> testCases)
        {
            LogTestsToRun(testCases);

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
            sb.AppendLine($"Tests to run (total {testCases.Count} tests):");
            foreach (var testCase in testCases)
                sb.AppendLine(testCase.ToString());
            _logger.Info(sb.ToString());
        }

        public async Task<TestRunResponse> StartTestRunAsync(TestCaseInfo testCase, TestRunState state)
        {
            if (_session.State == TestRunSessionState.NotStarted)
            {
                await SetSessionStateAsync(TestRunSessionState.Running);
            }
            var patch = new JsonPatchDocument<TestRunRequest>();
            patch.Add(r => r.StartTime, DateTime.Now);
            patch.Add(r => r.State, state);
            _logger.Debug($"Start testing test case {testCase.AzureTestCaseId}, State: {state}");
            return await _client.PatchTestRunAsync(_session.Id, testCase.TestRunId, patch);
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
            string errorMessage, IEnumerable<string> impactFiles)
        {
            var patch = new JsonPatchDocument<TestRunRequest>();
            patch.Add(r => r.State, TestRunState.Finished);
            patch.Add(r => r.Outcome, outcome);
            patch.Add(r => r.FinishTime, DateTime.Now);
            patch.Add(r => r.ErrorMessage, errorMessage);
            var runResponse = await _client.PatchTestRunAsync(_session.Id, testCase.TestRunId, patch);

            _logger.Info("Test run finished: " + ObjToString(runResponse));

            var impactRequest = new TestCaseImpactUpdateRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                CodeSignatures = impactFiles?.Select(f => new CodeSignature(f, CalculateSignature(f))).ToList()
            };

            LogDebug("Updating test impact information: ", impactRequest);

            await _client.UpdateTestImpactAsync(testCase.Id, impactRequest);

            var lastStateRequest = new TestLastStateUpdateRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                TestRunSessionId = _session.Id,
                Outcome = outcome
            };

            LogDebug("Updating test last state:", lastStateRequest);

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

        private async Task<IReadOnlyList<TestCaseInfo>> PublishChangesAndGetTestsToRunAsync(IEnumerable<string> changedFiles)
        {
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                TestRunSessionId = _session.Id,
                Changes = changedFiles.Select(f => new CodeSignature(f, CalculateSignature(f))).ToList()
            };
            return await PublishChangesAndGetTestsToRunAsync(req);
        }

        public async Task<IReadOnlyList<TestCaseInfo>> PublishNoChangeAndGetAllTestsAsync()
        {
            PublishImpactChangesRequest req = new PublishImpactChangesRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                TestRunSessionId = _session.Id,
                RunAllTests = true
            };
            return await PublishChangesAndGetTestsToRunAsync(req);
        }

        private async Task<IReadOnlyList<TestCaseInfo>> PublishChangesAndGetTestsToRunAsync(PublishImpactChangesRequest req)
        {
            LogDebug("Publishing changes to the service and updating last states of tests...");

            var response = await _client.PublishImpactChangesAsync(req);

            var testsToRun = new List<TestCaseInfo>();
            var azureIdToTestCases = _testCaseRequests.ToDictionary(t => t.AzureTestCaseId);
            foreach (var tr in response.TestsToRun)
            {
                if (azureIdToTestCases.TryGetValue(tr.AzureTestCaseId, out var testCase))
                {
                    testsToRun.Add(
                        new TestCaseInfo(
                            tr.Id, tr.AzureTestCaseId,
                            testCase.Title, tr.RunReason));
                }
            }
            _testsToRun = testsToRun;
            return _testsToRun;
        }

        private string CalculateSignature(string value)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var hash = sha1.ComputeHash(Encoding.Unicode.GetBytes(value));
            return GetBytesHexString(hash);
        }

        private string GetBytesHexString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture));
            }
            return builder.ToString();
        }

        private void LogDebug(string text, object obj = null)
        {
            if (_logger.IsEnabled)
            {
                _logger.Debug(text + (obj != null ? Environment.NewLine + ObjToString(obj) : string.Empty));
            }
        }

        private static JsonSerializerSettings CreateLogSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        private static string ObjToString(object obj)
        {
            return obj != null
                ? Environment.NewLine + JsonConvert.SerializeObject(obj, _logSerializerSettings)
                : string.Empty;
        }
    }
}
