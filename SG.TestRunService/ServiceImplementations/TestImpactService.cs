using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using SG.TestRunService.Data.Services;
using SG.TestRunService.ServiceImplementations.Auxiliary;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations
{
    public class TestImpactService : ITestImpactService
    {
        private readonly IBaseDbService _dbService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<TestImpactService> _logger;

        public TestImpactService(IBaseDbService dbService, ILoggerFactory loggerFactory, ILogger<TestImpactService> logger)
        {
            _dbService = dbService;
            _loggerFactory = loggerFactory;
            _logger = logger;
        }

        public async Task<IReadOnlyList<LastImpactUpdateResponse>> GetLastImpactUpdatesAsync()
        {
            return await _dbService.Query<LastImpactUpdate>().Project().ToListAsync();
        }

        private Task<T> GetLastImpactUpdateInternal<T>(
            int azureProductBuildDefId,
            Func<IQueryable<LastImpactUpdate>, IQueryable<T>> projector)
        {
            return projector(_dbService
                .Query<LastImpactUpdate>(
                    i => i.AzureProductBuildDefinitionId == azureProductBuildDefId))
                .FirstOrDefaultAsync();
        }

        public Task<LastImpactUpdateResponse> GetLastImpactUpdateAsync(int azureProductBuildDefId)
        {
            return GetLastImpactUpdateInternal(azureProductBuildDefId, ModelMappingExtensions.Project);
        }

        public async Task<LastImpactUpdateResponse> DeleteLastImpactUpdateAsync(int azureProductBuildDefId)
        {
            var lastImpactUpdate = await _dbService
                .Query<LastImpactUpdate>(l => l.AzureProductBuildDefinitionId == azureProductBuildDefId)
                .FirstOrDefaultAsync();
            if (lastImpactUpdate == null)
                return null;
            await _dbService.DeleteAsync(lastImpactUpdate);

            _logger.LogInformation("LastImpactUpdate {LastImpactUpdate} deleted", lastImpactUpdate);

            return lastImpactUpdate.ToResponse();
        }

        public async Task<(PublishImpactChangesResponse, ServiceError)> PublishImpactChangesAsync(PublishImpactChangesRequest request)
        {
            var lastUpdate = await GetLastImpactUpdateInternal(request.AzureProductBuildDefinitionId, e => e);
            if (lastUpdate == null)
            {
                lastUpdate = new LastImpactUpdate()
                {
                    AzureProductBuildDefinitionId = request.AzureProductBuildDefinitionId
                };
                _dbService.Add(lastUpdate);
            }

            lastUpdate.TestRunSessionId = request.TestRunSessionId;
            Data.BuildInfo buildInfo;
            if (request.TestRunSessionId.HasValue)
                buildInfo = await _dbService
                    .Query<TestRunSession>(id: request.TestRunSessionId.Value)
                    .Select(s => s.ProductBuildInfo)
                    .FirstAsync();
            else if (request.ProductBuild != null)
            {
                buildInfo = await _dbService
                    .Query<Data.BuildInfo>().Where(b => b.AzureBuildId == request.ProductBuild.AzureBuildId)
                    .FirstOrDefaultAsync();
                if (buildInfo == null)
                {
                    if (request.ProductBuild.AzureBuildDefinitionId == default)
                        request.ProductBuild.AzureBuildDefinitionId = request.AzureProductBuildDefinitionId;
                    if (string.IsNullOrEmpty(request.ProductBuild.SourceVersion))
                        return (null, ServiceError.BadRequest($"Required value `{nameof(request.ProductBuild)}.{nameof(request.ProductBuild.SourceVersion)}` is missing."));
                    if (string.IsNullOrEmpty(request.ProductBuild.BuildNumber))
                        return (null, ServiceError.BadRequest($"Required value `{nameof(request.ProductBuild)}.{nameof(request.ProductBuild.BuildNumber)}` is missing."));

                    buildInfo = request.ProductBuild.ToDataModel();
                    _dbService.Add(buildInfo);
                }
            }
            else
                throw new ArgumentException("`PublishImpactChangesRequest.BuildInfoId` is not set.");

            lastUpdate.ProductBuildInfo = buildInfo;
            lastUpdate.UpdateDate = DateTime.Now;

            int azureBuildDefId = buildInfo.AzureBuildDefinitionId;

            var tlsUpdater = new ImpactedTestUpdater(_dbService, buildInfo, _loggerFactory.CreateLogger<ImpactedTestUpdater>());
            PublishImpactChangesResponse response = null;
            if (request.NoBaseBuild)
            {
                await tlsUpdater.UpdateToNoBaseBuild();
            }
            else
            {
                if (request.CodeSignatures == null)
                    return (null, ServiceError.BadRequest("Code signatures are missing."));
                response = await tlsUpdater.UpdateImpactedTests(request.CodeSignatures);
            }
            await _dbService.SaveChangesAsync();
            return (response, ServiceError.NoError());
        }

        public async Task UpdateTestCaseImpactAsync(int testCaseId, TestCaseImpactUpdateRequest request)
        {
            bool logInformation = _logger.IsEnabled(LogLevel.Information);
            int? azureTestCaseId = null;
            if (logInformation)
            {
                azureTestCaseId = await _dbService
                    .Query<TestCase>(testCaseId)
                    .Select(t => (int?)t.AzureTestCaseId)
                    .FirstOrDefaultAsync();
                _logger.LogInformation("Updating impact items for test case {AzureTestCaseId} in build definition {BuildDefinitionId}:" +
                    " {FileSignaturesCount} files, {MethodSignaturesCount} methods",
                    azureTestCaseId, request.AzureProductBuildDefinitionId,
                    request.CodeSignatures.Where(r => r.Type == CodeSignatureType.File).Count(),
                    request.CodeSignatures.Where(r => r.Type == CodeSignatureType.Method).Count());
            }

            var impactItemsOnDb = (await _dbService
                .GetFilteredAsync(
                    filter: (TestCaseImpactItem tci) =>
                        tci.TestCaseId == testCaseId &&
                        tci.AzureProductBuildDefinitionId == request.AzureProductBuildDefinitionId,
                    projection: (TestCaseImpactItem tci) =>
                        new { tci, tci.CodeSignature.Signature }))
                .ToDictionary(t => t.Signature, t => t.tci);

            var present = new HashSet<TestCaseImpactItem>();
            var codeSignaturesToAdd = new List<Common.Models.CodeSignature>();
            var recoverCount = 0;
            var now = DateTime.Now;

            if (request.CodeSignatures != null)
            {
                foreach (var rcs in request.CodeSignatures)
                {
                    if (impactItemsOnDb.TryGetValue(rcs.Signature, out var testImpactCodeSignature))
                    {
                        present.Add(testImpactCodeSignature);
                        if (testImpactCodeSignature.IsDeleted)
                        {
                            testImpactCodeSignature.IsDeleted = false;
                            testImpactCodeSignature.DateAdded = now;
                            ++recoverCount;
                        }
                    }
                    else
                    {
                        codeSignaturesToAdd.Add(rcs);
                    }
                }
                if (codeSignaturesToAdd.Count > 0)
                {
                    var signatures = codeSignaturesToAdd.Select(cs => cs.Signature).ToList();
                    var availableDbCodeSignatures = (await _dbService
                        .GetFilteredAsync<Data.CodeSignature>(cs => signatures.Contains(cs.Signature)))
                        .ToDictionary(cs => cs.Signature);

                    foreach (var csToAdd in codeSignaturesToAdd)
                    {
                        if (!availableDbCodeSignatures.TryGetValue(csToAdd.Signature, out var dbCodeSignature))
                        {
                            dbCodeSignature = new Data.CodeSignature()
                            {
#pragma warning disable 618
                                Path = csToAdd.Path ?? csToAdd.FileName,
#pragma warning restore 618
                                Signature = csToAdd.Signature,
                                Type = csToAdd.Type == 0 ? CodeSignatureType.File : csToAdd.Type
                            };
                            _dbService.Add(dbCodeSignature);
                        }
                        _dbService.Add(
                            new TestCaseImpactItem()
                            {
                                AzureProductBuildDefinitionId = request.AzureProductBuildDefinitionId,
                                DateAdded = now,
                                TestCaseId = testCaseId,
                                CodeSignature = dbCodeSignature
                            });
                    }
                }
            }

            int deleteCount = 0;
            foreach (var impactCodeSignatureEntity in impactItemsOnDb.Values)
                if (!present.Contains(impactCodeSignatureEntity))
                {
                    impactCodeSignatureEntity.IsDeleted = true;
                    impactCodeSignatureEntity.DateRemoved = now;
                    ++deleteCount;
                }

            await _dbService.SaveChangesAsync();

            if (logInformation)
                _logger.LogInformation("Impact items updated for test case {AzureTestCaseId} " +
                    "in build definition {BuildDefinitionId} - Code signatures: " +
                    "{AddedItemsCount} added, {RecoveredItemsCount} recovered (had been deleted), {DeletedItemsCount} deleted",
                    azureTestCaseId, request.AzureProductBuildDefinitionId,
                    codeSignaturesToAdd.Count, recoverCount, deleteCount);
        }

        public async Task<ServiceError> UpdateTestLastStateAsync(int testCaseId, TestLastStateUpdateRequest lastStateUpdateRequest)
        {
            var testLastState = await _dbService.Query<TestLastState>(tls =>
                    tls.TestCaseId == testCaseId &&
                    tls.AzureProductBuildDefinitionId == lastStateUpdateRequest.AzureProductBuildDefinitionId)
                .FirstOrDefaultAsync();
            if (testLastState == null)
            {
                testLastState = new TestLastState()
                {
                    TestCaseId = testCaseId,
                    AzureProductBuildDefinitionId = lastStateUpdateRequest.AzureProductBuildDefinitionId
                };
                _dbService.Add(testLastState);
            }

            var testRunSessionInfo = await _dbService.Query<TestRunSession>(s => s.Id == lastStateUpdateRequest.TestRunSessionId)
                .Select(s => new { SessionStartTime = s.StartTime, BuildInfoId = s.ProductBuildInfoId })
                .FirstOrDefaultAsync();
            if (testRunSessionInfo == default)
            {
                return ServiceError.NotFound("Requested test run session not found. Id: " + lastStateUpdateRequest.TestRunSessionId);
            }

            bool hasImpactData = _dbService
                .Query<TestCaseImpactItem>(t =>
                    t.TestCaseId == testCaseId &&
                    t.AzureProductBuildDefinitionId == lastStateUpdateRequest.AzureProductBuildDefinitionId &&
                    !t.IsDeleted)
                .Any();
            var outcome = lastStateUpdateRequest.Outcome;
            testLastState.LastOutcome = outcome;
            switch (outcome)
            {
                case TestRunOutcome.Successful:
                    if (hasImpactData)
                    {
                        if (!testLastState.IsImpactedAfter(testRunSessionInfo.SessionStartTime))
                        {
                            testLastState.ShouldBeRun = false;
                            testLastState.RunReason = null;
                        }
                    }
                    else
                    {
                        testLastState.ShouldBeRun = true;
                        testLastState.RunReason = RunReason.ImpactDataNotAvailable;
                    }
                    break;
                case TestRunOutcome.Failed:
                case TestRunOutcome.FatalError:
                    testLastState.ShouldBeRun = true;
                    testLastState.RunReason = RunReason.Failed;
                    break;
                case TestRunOutcome.Aborted:
                case TestRunOutcome.Unknown:
                    testLastState.ShouldBeRun = true;
                    testLastState.RunReason = RunReason.Aborted;
                    break;
                default:
                    return ServiceError.UnprocessableEntity("Invalid outcome: " + outcome);
            }
            testLastState.LastOutcomeDate = DateTime.Now;
            testLastState.LastOutcomeProductBuildInfoId = testRunSessionInfo.BuildInfoId;

            await _dbService.SaveChangesAsync();
            return ServiceError.NoError();
        }

        public async Task<IReadOnlyList<TestLastStateResponse>> GetTestLastStatesAsync(int testCaseId)
        {
            return await _dbService
                .Query<TestLastState>(t => t.TestCaseId == testCaseId)
                .Project()
                .ToListAsync();
        }

        public async Task<TestLastStateResponse> DeleteTestLastStateAsync(int testCaseId, int azureProductBuildDefId)
        {
            var lastState = await _dbService
                .Query<TestLastState>(t =>
                   t.TestCaseId == testCaseId &&
                   t.AzureProductBuildDefinitionId == azureProductBuildDefId)
                .FirstOrDefaultAsync();
            if (lastState == null)
                return null;
            await _dbService.DeleteAsync(lastState);

            _logger.LogInformation("TestLastState {TestLastState} deleted", lastState);

            return lastState.ToResponse();
        }

        public async Task<IReadOnlyCollection<TestToRunResponse>> GetTestsToRun(
            int azureBuildDefinitionId, bool allTests = false)
        {
            var testLastStates = _dbService
                .Query<TestLastState>(tl =>
                    tl.AzureProductBuildDefinitionId == azureBuildDefinitionId);

            var joined =
                    from tc in _dbService.Query<TestCase>()
                    join tl in testLastStates on tc.Id equals tl.TestCaseId into tlj
                    from tl in tlj.DefaultIfEmpty()
                    select new { TestCase = tc, TestLastState = tl };

            if (!allTests)
                joined = joined.Where(j => j.TestLastState == null || j.TestLastState.ShouldBeRun);

            var testsToRun =
                from j in joined
                select new TestToRunResponse()
                {
                    TestCaseId = j.TestCase.Id,
                    AzureTestCaseId = j.TestCase.AzureTestCaseId,
                    RunReason = j.TestLastState == null ? RunReason.New : (j.TestLastState.RunReason ?? RunReason.ForceRun)
                };

            return await testsToRun.ToListAsync();
        }
    }
}
