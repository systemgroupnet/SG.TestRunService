using Microsoft.AspNetCore.JsonPatch;
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

        private TestRunSessionAgent(
            TestRunClient client,
            ITestRunClientConfiguration configuration,
            IDevOpsServerHandle devOpsServerHandle,
            TestRunSessionResponse session)
        {
            _client = client;
            _configuration = configuration;
            _devOpsServerHandle = devOpsServerHandle;
            _session = session;
            _project = session.ProductBuild.TeamProject;
        }

        public static async Task<TestRunSessionAgent> CreateAsync(
            ITestRunClientConfiguration configuration,
            IDevOpsServerHandle devOpsServerHandle,
            TestRunSessionRequest sessionRequest)
        {
            var client = new TestRunClient(configuration.TestRunServiceUrl);
            var response = await client.InsertSessionAsync(sessionRequest);
            return new TestRunSessionAgent(client, configuration, devOpsServerHandle, response);
        }

        public async Task IntroduceTestCasesAsync(IEnumerable<TestCaseRequest> tests)
        {
            _testCaseRequests = tests.ToList();
            var azureTestCaseIds = new HashSet<int>(await _client.GetAzureTestCaseIdsAsync(_project));
            var newTestCases = _testCaseRequests.Where(t => !azureTestCaseIds.Contains(t.AzureTestCaseId));
            foreach (var tc in newTestCases)
            {
                await _client.InsertTestCaseAsync(tc);
            }
        }

        private async Task<string> GetBaseBuildSourceVersionAsync()
        {
            var lastUpdate = await _client.GetLastImpactUpdateAsync(_session.ProductBuild.AzureBuildDefinitionId);
            if (lastUpdate == null)
                return null;
            return lastUpdate.ProductBuild.SourceVersion;
        }

        public async Task<IReadOnlyList<TestCaseInfo>> GetTestsToRunAsync()
        {
            if (_testCaseRequests == null)
                throw new InvalidOperationException($"Test cases are not available. Call `{nameof(IntroduceTestCasesAsync)}` first.");

            if (_testsToRun != null)
                return _testsToRun;

            var currentSourceVersion = _session.ProductBuild.SourceVersion;
            var baseSourceVersion = await GetBaseBuildSourceVersionAsync();
            if (baseSourceVersion == null)
                return await PublishNoChangeAndGetAllTestsAsnc();
            if (baseSourceVersion == currentSourceVersion)
            {
                // issue a warning
            }
            else if (!_devOpsServerHandle.IsChronologicallyAfter(currentSourceVersion, baseSourceVersion))
            {
                switch (_configuration.RunForOlderVersionBeahvior)
                {
                    case TestOlderVersionBehavior.Fail:
                        throw new Exception($"Source version used for this test (${currentSourceVersion})is older than the last test ran on this build definition (${baseSourceVersion}).");
                    case TestOlderVersionBehavior.RunNotSuccessfulTests:
                        return await PublishChangesAndGetTestsToRunAsync(Enumerable.Empty<string>());
                    case TestOlderVersionBehavior.RunImpactedAndNotSuccessfulTests:
                        break;
                    case TestOlderVersionBehavior.RunAllTests:
                        return await PublishNoChangeAndGetAllTestsAsnc();
                    default:
                        throw new NotSupportedException($"The value '{_configuration.RunForOlderVersionBeahvior}' for configuration '{nameof(_configuration.RunForOlderVersionBeahvior)}' is not valid.");
                }
            }
            var changedFiles = _devOpsServerHandle.GetChangedFiles(
                _project,
                _session.ProductBuild.AzureBuildDefinitionId,
                baseSourceVersion, currentSourceVersion);
            return await PublishChangesAndGetTestsToRunAsync(changedFiles);
        }

        public async Task<IReadOnlyList<TestRunResponse>> RecordSessionTestsAsync(IEnumerable<TestCaseInfo> testCases)
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

        public async Task<TestRunResponse> StartTestRunAsync(TestCaseInfo testCase, TestRunState state)
        {
            if (_session.State == TestRunSessionState.NotStarted)
            {
                await SetSessionStateAsync(TestRunSessionState.Running);
            }
            var patch = new JsonPatchDocument<TestRunRequest>();
            patch.Add(r => r.StartTime, DateTime.Now);
            patch.Add(r => r.State, state);
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

        public async Task<TestRunResponse> RecordTestRunEndAsync(TestCaseInfo testCase, TestRunOutcome outcome, IEnumerable<string> impactFiles)
        {
            var patch = new JsonPatchDocument<TestRunRequest>();
            patch.Add(r => r.State, TestRunState.Finished);
            patch.Add(r => r.Outcome, outcome);
            patch.Add(r => r.FinishTime, DateTime.Now);
            var runResponse = await _client.PatchTestRunAsync(_session.Id, testCase.TestRunId, patch);
            await _client.UpdateTestImpactAsync(testCase.Id, new TestCaseImpactUpdateRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                CodeSignatures = impactFiles.Select(f => new CodeSignature(f, CalculateSignature(f))).ToList()
            });
            await _client.UpdateTestLastStateAsync(testCase.Id, new TestLastStateUpdateRequest()
            {
                AzureProductBuildDefinitionId = _session.ProductBuild.AzureBuildDefinitionId,
                TestRunSessionId = _session.Id,
                Outcome = outcome
            });
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
        }

        private Task<TestRunResponse> SetTestRunStateAsync(TestCaseInfo testCase, TestRunState state)
        {
            var patch = new JsonPatchDocument<TestRunRequest>();
            patch.Add(r => r.State, state);
            return _client.PatchTestRunAsync(_session.Id, testCase.TestRunId, patch);
        }

        private async Task SetSessionStateAsync(TestRunSessionState state)
        {
            var sessionPatch = new JsonPatchDocument<TestRunSessionRequest>();
            sessionPatch.Add(s => s.State, state);
            var newSession = await _client.PatchTestRunSessionAsync(_session.Id, sessionPatch);
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

        private async Task<IReadOnlyList<TestCaseInfo>> PublishNoChangeAndGetAllTestsAsnc()
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
    }
}
