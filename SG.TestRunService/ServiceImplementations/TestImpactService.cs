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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations
{
    public class TestImpactService : ITestImpactService
    {
        private readonly IBaseDbService _dbService;
        private readonly IProductLineService _productLineService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<TestImpactService> _logger;

        public TestImpactService(IBaseDbService dbService, IProductLineService productLineService, ILoggerFactory loggerFactory, ILogger<TestImpactService> logger)
        {
            _dbService = dbService;
            _productLineService = productLineService;
            _loggerFactory = loggerFactory;
            _logger = logger;
        }

        public async Task<IReadOnlyList<LastImpactUpdateResponse>> GetLastImpactUpdatesAsync()
        {
            return await _dbService
                .Query<LastImpactUpdate>()
                .Project()
                .ToListAsync();
        }

        private async Task<(T, ServiceError)> GetLastImpactUpdateInternalAsync<T>(
            ProductLineIdOrKey productLine, Func<IQueryable<LastImpactUpdate>, IQueryable<T>> projector)
        {
            var (predicate, error) = GetLastImpactUpdateFetchPredicate(productLine);
            if (!error.IsSuccessful())
                return (default, error);

            var response = await projector(_dbService.Query(predicate)).FirstOrDefaultAsync();
            if (response == null)
                return (default, ServiceError.NotFound());
            return (response, ServiceError.NoError);
        }

        public Task<(LastImpactUpdateResponse, ServiceError)> GetLastImpactUpdateAsync(ProductLineIdOrKey productLine)
        {
            return GetLastImpactUpdateInternalAsync(productLine, ModelMappingExtensions.Project);
        }

        private static (Expression<Func<LastImpactUpdate, bool>>, ServiceError) GetLastImpactUpdateFetchPredicate(ProductLineIdOrKey productLine)
        {
            if (productLine.Id == null && productLine.Key == null)
                return (null, ServiceError.BadRequest("ProductLineId or ProductLineKey is required."));

            Expression<Func<LastImpactUpdate, bool>> predicate;

            if (productLine.Id.HasValue)
                predicate = l => l.ProductLineId == productLine.Id;
            else
                predicate = l => l.ProductLine.Key == productLine.Key;
            return (predicate, ServiceError.NoError);
        }

        public async Task<(LastImpactUpdateResponse, ServiceError)> DeleteLastImpactUpdateAsync(ProductLineIdOrKey productLine)
        {
            var (lastImpactUpdate, error) = await GetLastImpactUpdateInternalAsync(productLine, q => q);
            if (!error.IsSuccessful())
                return (null, error);

            await _dbService.DeleteAsync(lastImpactUpdate);

            _logger.LogInformation("LastImpactUpdate {LastImpactUpdate} deleted", lastImpactUpdate);

            return (lastImpactUpdate.ToResponse(), ServiceError.NoError);
        }

        public async Task<(PublishImpactChangesResponse, ServiceError)> PublishImpactChangesAsync(PublishImpactChangesRequest request)
        {
            int productLineId;

            var (lastUpdate, error) = await GetLastImpactUpdateInternalAsync(request.ProductLine, q => q);
            if (!error.IsSuccessful())
            {
                if (error.Category == ServiceErrorCategory.NotFound)
                {
                    Data.ProductLine productLine;
                    (productLine, error) = await _productLineService.GetOrInsertProductLineAsync(request.ProductLine);
                    if (!error.IsSuccessful())
                        return (null, error);

                    productLineId = productLine.Id;

                    lastUpdate = new LastImpactUpdate()
                    {
                        ProductLineId = productLine.Id
                    };
                    _dbService.Add(lastUpdate);
                }
                else
                {
                    return (null, error);
                }
            }
            else
            {
                productLineId = lastUpdate.ProductLineId;
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
                        return (null, ServiceError.BadRequest($"Required value `{nameof(request.ProductBuild)}.{nameof(request.ProductBuild.AzureBuildDefinitionId)}` is missing."));
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

            var tlsUpdater = new ImpactedTestUpdater(_dbService, productLineId, buildInfo, _loggerFactory.CreateLogger<ImpactedTestUpdater>());
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

            if (response != null)
            {
                await InsertImpactHistoryAfterUpdate(productLineId, buildInfo, response.CodeSignatureImpactedTestCaseIds);
            }

            return (response, ServiceError.NoError);
        }

        private async Task InsertImpactHistoryAfterUpdate(
            int productLineId,
            Data.BuildInfo productBuildInfo,
            IDictionary<string, IReadOnlyList<int>> codeSignatureImpactedTestCaseIds)
        {
            try
            {
                var now = DateTime.Now;

                var signatures = codeSignatureImpactedTestCaseIds.Keys;
                var signatureIds = await _dbService.Query<Data.CodeSignature>(cs => signatures.Contains(cs.Signature))
                    .Select(cs => new { cs.Id, cs.Signature })
                    .ToDictionaryAsync(cs => cs.Signature, cs => cs.Id);

                foreach (var keyVal in codeSignatureImpactedTestCaseIds)
                {
                    var signature = keyVal.Key;
                    var testCaseIds = keyVal.Value;
                    if (!signatureIds.TryGetValue(signature, out var signatureId))
                    {
                        _logger.LogWarning("Code Signature {Signature} not found in database!", signature);
                        continue;
                    }
                    foreach (var tcId in testCaseIds)
                    {
                        _dbService.Add(new TestCaseImpactHistory()
                        {
                            ProductLineId = productLineId,
                            CodeSignatureId = signatureId,
                            TestCaseId = tcId,
                            Date = now,
                            ProductBuildInfoId = productBuildInfo.Id
                        });
                    }
                }

                await _dbService.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Adding history items for impacted test cases on build {@BuildInfo} failed.", productBuildInfo);
            }
        }

        public async Task<ServiceError> UpdateTestCaseImpactAsync(int testCaseId, TestCaseImpactUpdateRequest request)
        {
            bool logInformation = _logger.IsEnabled(LogLevel.Information);
            var (productLine, error) = await _productLineService.GetOrInsertProductLineAsync(request.ProductLine);
            if (!error.IsSuccessful())
                return error;
            int? azureTestCaseId = null;
            if (logInformation)
            {
                azureTestCaseId = await _dbService
                    .Query<TestCase>(testCaseId)
                    .Select(t => (int?)t.AzureTestCaseId)
                    .FirstOrDefaultAsync();
                _logger.LogInformation("Updating impact items for test case {AzureTestCaseId} in product-line {ProductLine}:" +
                    " {FileSignaturesCount} files, {MethodSignaturesCount} methods",
                    azureTestCaseId, productLine,
                    request.CodeSignatures.Where(r => r.Type == CodeSignatureType.File).Count(),
                    request.CodeSignatures.Where(r => r.Type == CodeSignatureType.Method).Count());
            }

            var impactItemsOnDb = (await _dbService
                .GetFilteredAsync(
                    filter: (TestCaseImpactItem tci) =>
                        tci.TestCaseId == testCaseId &&
                        tci.ProductLineId == productLine.Id,
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
                                Path = csToAdd.Path,
                                Signature = csToAdd.Signature,
                                Type = csToAdd.Type == 0 ? CodeSignatureType.File : csToAdd.Type
                            };
                            _dbService.Add(dbCodeSignature);
                        }
                        _dbService.Add(
                            new TestCaseImpactItem()
                            {
                                ProductLineId = productLine.Id,
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
                    "in product-line {ProductLine} - Code signatures: " +
                    "{AddedItemsCount} added, {RecoveredItemsCount} recovered (had been deleted), {DeletedItemsCount} deleted",
                    azureTestCaseId, productLine,
                    codeSignaturesToAdd.Count, recoverCount, deleteCount);

            return ServiceError.NoError;
        }

        public async Task<ServiceError> UpdateTestLastStateAsync(int testCaseId, TestLastStateUpdateRequest lastStateUpdateRequest)
        {
            var (productLine, error) = await _productLineService.GetProductLineAsync(lastStateUpdateRequest.ProductLine);
            if (!error.IsSuccessful())
                return error;

            var testLastState = await _dbService.Query<TestLastState>(tls =>
                    tls.TestCaseId == testCaseId &&
                    tls.ProductLineId == productLine.Id)
                .FirstOrDefaultAsync();
            if (testLastState == null)
            {
                testLastState = new TestLastState()
                {
                    TestCaseId = testCaseId,
                    ProductLineId = productLine.Id
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
                    t.ProductLineId == productLine.Id &&
                    !t.IsDeleted)
                .Any();

            var outcome = lastStateUpdateRequest.Outcome;
            testLastState.LastOutcome = outcome;

            if (lastStateUpdateRequest.DictatedRunReason != null)
            {
                testLastState.ShouldBeRun = true;
                testLastState.RunReason = lastStateUpdateRequest.DictatedRunReason;
            }
            else
            {
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
            }
            testLastState.LastOutcomeDate = DateTime.Now;
            testLastState.LastOutcomeProductBuildInfoId = testRunSessionInfo.BuildInfoId;

            await _dbService.SaveChangesAsync();
            return ServiceError.NoError;
        }

        public async Task<IReadOnlyList<TestLastStateResponse>> GetTestLastStatesAsync(int testCaseId)
        {
            return await _dbService
                .Query<TestLastState>(t => t.TestCaseId == testCaseId)
                .Project()
                .ToListAsync();
        }

        public async Task<(TestLastStateResponse, ServiceError)> DeleteTestLastStateAsync(int testCaseId, ProductLineIdOrKey productLine)
        {
            var (productLineId, error) = await _productLineService.GetProductLineIdAsync(productLine);
            if (!error.IsSuccessful())
                return (null, error);

            var lastState = await _dbService
                .Query<TestLastState>(t =>
                   t.TestCaseId == testCaseId &&
                   t.ProductLineId == productLineId)
                .FirstOrDefaultAsync();
            if (lastState == null)
                return (null, ServiceError.NotFound());
            await _dbService.DeleteAsync(lastState);

            _logger.LogInformation("TestLastState {TestLastState} deleted", lastState);

            return (lastState.ToResponse(), ServiceError.NoError);
        }

        public async Task<(IReadOnlyCollection<TestToRunResponse>, ServiceError)> GetTestsToRun(
            ProductLineIdOrKey productLine, bool allTests = false)
        {
            var (productLineId, error) = await _productLineService.GetProductLineIdAsync(productLine);
            if (!error.IsSuccessful())
                return (null, error);

            var testLastStates = _dbService
                .Query<TestLastState>(tl =>
                    tl.ProductLineId == productLineId);

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

            return (await testsToRun.ToListAsync(), ServiceError.NoError);
        }
    }
}
