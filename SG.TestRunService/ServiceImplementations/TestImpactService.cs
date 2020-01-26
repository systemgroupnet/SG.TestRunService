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
            var testRunSessionBuildInfo = await _dbService
                .Query<TestRunSession>(id: request.TestRunSessionId)
                .Select(s => s.ProductBuildInfo)
                .FirstAsync();

            lastUpdate.ProductBuildInfoId = testRunSessionBuildInfo.Id;
            lastUpdate.UpdateDate = DateTime.Now;

            string project = testRunSessionBuildInfo.TeamProject;
            int azureBuildDefId = testRunSessionBuildInfo.AzureBuildDefinitionId;

            var tlsUpdater = new ImpactedTestsFinder(_dbService, _configuration, testRunSessionBuildInfo);
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
            var originalCodeSignatures = (await _dbService
                .GetFilteredAsync<TestCaseImpactCodeSignature>(cs =>
                    cs.TestCaseId == testCaseId &&
                    cs.AzureProductBuildDefinitionId == request.AzureProductBuildDefinitionId))
                .ToDictionary(cs => cs.Signature, cs => (ImpactCodeSignatureEntity: cs, Present: false));
            foreach (var rcs in request.CodeSignatures)
            {
                if (originalCodeSignatures.TryGetValue(rcs.Signature, out var testImpactCodeSignature))
                {
                    testImpactCodeSignature.Present = true;
                    var entity = testImpactCodeSignature.ImpactCodeSignatureEntity;
                    if (entity.IsDeleted)
                    {
                        entity.IsDeleted = false;
                        entity.DateAdded = DateTime.Now;
                    }
                }
                else
                {
                    _dbService.Add(
                        new TestCaseImpactCodeSignature()
                        {
                            AzureProductBuildDefinitionId = request.AzureProductBuildDefinitionId,
                            DateAdded = DateTime.Now,
                            Signature = rcs.Signature,
                            FilePath = rcs.FileName,
                            TestCaseId = testCaseId
                        });
                }
            }
            foreach (var (impactCodeSignatureEntity, present) in originalCodeSignatures.Values)
                if (!present)
                {
                    impactCodeSignatureEntity.IsDeleted = true;
                    impactCodeSignatureEntity.DateRemoved = DateTime.Now;
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
            var outcome = lastStateUpdateRequest.Outcome; ;
            testLastState.LastOutcome = outcome;
            switch (outcome)
            {
                case TestRunOutcome.Successful:
                    testLastState.ShouldBeRun = false;
                    testLastState.RunReason = null;
                    break;
                case TestRunOutcome.Failed:
                    testLastState.ShouldBeRun = true;
                    testLastState.RunReason = RunReason.Failed;
                    break;
                case TestRunOutcome.Aborted:
                    testLastState.ShouldBeRun = true;
                    testLastState.RunReason = RunReason.NotRan;
                    break;
                case TestRunOutcome.FatalError:
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
