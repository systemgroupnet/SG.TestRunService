using Microsoft.EntityFrameworkCore;
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
        private readonly int _azureBuildDefId;

        public ImpactedTestUpdater(IBaseDbService dbService, Data.BuildInfo testRunSessionBuildInfo)
        {
            _dbService = dbService;
            _buildInfo = testRunSessionBuildInfo;
            _azureBuildDefId = _buildInfo.AzureBuildDefinitionId;
        }

        public async Task<PublishImpactChangesResponse> UpdateImpactedTests(IEnumerable<string> changedCodeSignatures)
        {
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

            return new PublishImpactChangesResponse()
            {
                ImpactedTests = impactedTests,
                CodeSignatureImpactedTestCaseIds = codeSignatureTests
            };
        }

        public async Task UpdateToNoBaseBuild()
        {
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
