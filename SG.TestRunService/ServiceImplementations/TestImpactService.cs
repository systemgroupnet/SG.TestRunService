using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        readonly IConfiguration _configuration;

        public TestImpactService(IBaseDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            _configuration = configuration;
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
            return lastImpactUpdate.ToResponse();
        }

        public async Task<IReadOnlyList<TestToRunResponse>> PublishImpactChangesAsync(PublishImpactChangesRequest request)
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
            else if (request.AzureProductBuildId.HasValue)
                buildInfo = await _dbService
                    .Query<Data.BuildInfo>().Where(b => b.AzureBuildId == request.AzureProductBuildId.Value)
                    .FirstAsync();
            else
                throw new ArgumentException("`PublishImpactChangesRequest.BuildInfoId` is not set.");

            lastUpdate.ProductBuildInfoId = buildInfo.Id;
            lastUpdate.UpdateDate = DateTime.Now;

            int azureBuildDefId = buildInfo.AzureBuildDefinitionId;

            var tlsUpdater = new ImpactedTestsFinder(_dbService, _configuration, buildInfo);
            IReadOnlyCollection<TestToRun> testsToRun;
            if (request.RunAllTests)
            {
                testsToRun = await tlsUpdater.UpdateAndGetAllTestsToRun();
            }
            else
            {
                testsToRun = await tlsUpdater.UpdateAndGetTestsToRun(request.Changes.Select(c => c.Signature).ToList());
            }
            var response = testsToRun
                .Select(t =>
                    new TestToRunResponse()
                    {
                        Id = t.TestLastState.TestCaseId,
                        AzureTestCaseId = t.AzureTestCaseId,
                        RunReason = t.RunReason
                    })
                .ToList();
            await _dbService.SaveChangesAsync();
            return response;
        }

        public async Task UpdateTestCaseImpactAsync(int testCaseId, TestCaseImpactUpdateRequest request)
        {
            var impactItemsOnDb = (await _dbService
                .GetFilteredAsync<TestCaseImpactItem>(cs =>
                    cs.TestCaseId == testCaseId &&
                    cs.AzureProductBuildDefinitionId == request.AzureProductBuildDefinitionId))
                .ToDictionary(cs => cs.CodeSignature.Signature);

            var present = new HashSet<TestCaseImpactItem>();
            var codeSignaturesToAdd = new List<Common.Models.CodeSignature>();
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
                                Path = csToAdd.FileName,
                                Signature = csToAdd.Signature
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
            foreach (var impactCodeSignatureEntity in impactItemsOnDb.Values)
                if (!present.Contains(impactCodeSignatureEntity))
                {
                    impactCodeSignatureEntity.IsDeleted = true;
                    impactCodeSignatureEntity.DateRemoved = now;
                }
            await _dbService.SaveChangesAsync();
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

            var productBuildInfoId = await _dbService.Query<TestRunSession>(s => s.Id == lastStateUpdateRequest.TestRunSessionId)
                .Select(s => s.ProductBuildInfoId)
                .FirstOrDefaultAsync();
            if (productBuildInfoId == default)
            {
                return ServiceError.NotFound("Requested test run session (or related build info) not found. Id: " + lastStateUpdateRequest.TestRunSessionId);
            }

            bool hasImpactData = _dbService
                .Query<TestCaseImpactItem>(t =>
                    t.TestCaseId == testCaseId &&
                    t.AzureProductBuildDefinitionId == lastStateUpdateRequest.AzureProductBuildDefinitionId &&
                    !t.IsDeleted)
                .Any();
            var outcome = lastStateUpdateRequest.Outcome; ;
            testLastState.LastOutcome = outcome;
            switch (outcome)
            {
                case TestRunOutcome.Successful:
                    if (hasImpactData)
                    {
                        testLastState.ShouldBeRun = false;
                        testLastState.RunReason = null;
                    }
                    else
                    {
                        testLastState.ShouldBeRun = true;
                        testLastState.RunReason = RunReason.ImpactDataNotAvailable;
                    }
                    break;
                case TestRunOutcome.Failed:
                    testLastState.ShouldBeRun = true;
                    testLastState.RunReason = RunReason.Failed;
                    break;
                case TestRunOutcome.Aborted:
                    testLastState.ShouldBeRun = true;
                    testLastState.RunReason = RunReason.New;
                    break;
                case TestRunOutcome.FatalError:
                case TestRunOutcome.Unknown:
                    testLastState.ShouldBeRun = true;
                    testLastState.RunReason = RunReason.Failed;
                    break;
                default:
                    return ServiceError.UnprocessableEntity("Invalid outcome: " + outcome);
            }
            testLastState.UpdateDate = DateTime.Now;
            testLastState.ProductBuildInfoId = productBuildInfoId;

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
            return lastState.ToResponse();
        }
    }
}
