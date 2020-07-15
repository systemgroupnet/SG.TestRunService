using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using SG.TestRunService.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations.Auxiliary
{
    internal class ImpactedTestUpdater
    {
        private readonly IBaseDbService _dbService;
        private readonly Data.BuildInfo _buildInfo;
        private readonly ILogger<ImpactedTestUpdater> _logger;
        private readonly int _azureBuildDefId;

        public ImpactedTestUpdater(IBaseDbService dbService, Data.BuildInfo testRunSessionBuildInfo, ILogger<ImpactedTestUpdater> logger = null)
        {
            _dbService = dbService;
            _buildInfo = testRunSessionBuildInfo;
            _logger = logger ?? new NullLogger<ImpactedTestUpdater>();
            _azureBuildDefId = _buildInfo.AzureBuildDefinitionId;
        }

        public async Task<PublishImpactChangesResponse> UpdateImpactedTests(IEnumerable<string> changedCodeSignatures)
        {
            _logger.LogInformation("Updating impacted tests for build {@BuildInfo}, with changes {CodeSignatures}", _buildInfo, changedCodeSignatures);

            var impactItems =
                from ii in _dbService.Query<TestCaseImpactItem>()
                where
                    ii.AzureProductBuildDefinitionId == _azureBuildDefId &&
                    !ii.IsDeleted &&
                    changedCodeSignatures.Contains(ii.CodeSignature.Signature)
                select ii;

            var signaturesAndImpactedTests = await
                    (from ii in impactItems
                     join tl in GetTestLastStates() on ii.TestCaseId equals tl.TestCaseId
                     select new
                     {
                         ii.CodeSignature.Signature,
                         ii.TestCaseId,
                         TestLastState = tl,
                         tl.TestCase.AzureTestCaseId
                     })
                    .ToListAsync();

            var testLastStates = signaturesAndImpactedTests
                .Select(s => s.TestLastState)
                .Distinct(new TestLastState.IDEqulityComparer())
                .ToList();

            UpdateLastStateToImpacted(testLastStates);

            var impactedTests = signaturesAndImpactedTests
                .Select(s =>
                    new ImpactedTestResponse()
                    {
                        TestCaseId = s.TestCaseId,
                        AzureTestCaseId = s.AzureTestCaseId
                    })
                .Distinct()
                .ToList();

            var codeSignatureTests = signaturesAndImpactedTests
                .GroupBy(s => s.Signature)
                .ToDictionary(s => s.Key, g => (IReadOnlyList<int>)g.Select(i => i.TestCaseId).ToList().AsReadOnly());

            _logger.LogInformation("Impacted tests for build {@BuildInfo}: {CodeSignatureTests}", _buildInfo, codeSignatureTests);

            return new PublishImpactChangesResponse()
            {
                ImpactedTests = impactedTests,
                CodeSignatureImpactedTestCaseIds = codeSignatureTests
            };
        }

        public async Task UpdateToNoBaseBuild()
        {
            _logger.LogInformation("Update to 'No base build' for build {@BuildInfo}", _buildInfo);

            var testLastStates = await GetTestLastStates().ToListAsync();
            UpdateLastStates(testLastStates, RunReason.NoBaseBuild);
        }

        private IQueryable<TestLastState> GetTestLastStates()
        {
            return _dbService.Query<TestLastState>(tl =>
                  tl.AzureProductBuildDefinitionId == _azureBuildDefId);
        }

        private void UpdateLastStateToImpacted(IReadOnlyCollection<TestLastState> testLastStates)
        {
            UpdateLastStates(testLastStates, RunReason.Impacted);
        }

        private void UpdateLastStates(IReadOnlyCollection<TestLastState> testLastStates, RunReason runReason)
        {
            var now = DateTime.Now;

            _logger.LogInformation("Updating test last states for build {@BuildInfo} - reason: {RunReason}, date/time: {DateTime}, test case ids {TestCaseIds}",
                _buildInfo, runReason, now, testLastStates.Select(tl => tl.TestCaseId));

            foreach (var state in testLastStates)
            {
                state.LastImpactedDate = now;
                state.LastImpactedProductBuildInfo = _buildInfo;
                state.ShouldBeRun = true;
                state.RunReason = runReason;
            }
        }
    }
}
