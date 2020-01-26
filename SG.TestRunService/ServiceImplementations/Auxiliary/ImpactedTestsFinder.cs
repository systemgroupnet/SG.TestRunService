using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using SG.TestRunService.Data.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations.Auxiliary
{
    internal class ImpactedTestsFinder
    {
        private readonly IBaseDbService _dbService;
        private readonly Data.BuildInfo _buildInfo;
        private readonly IConfiguration _configuration;
        private readonly string _project;
        private readonly int _azureBuildDefId;

        public ImpactedTestsFinder(IBaseDbService dbService, IConfiguration configuration, Data.BuildInfo testRunSessionBuildInfo)
        {
            _dbService = dbService;
            _configuration = configuration;
            _buildInfo = testRunSessionBuildInfo;
            _project = _buildInfo.TeamProject;
            _azureBuildDefId = _buildInfo.AzureBuildDefinitionId;
        }

        public async Task<IReadOnlyList<TestToRun>> UpdateAndGetTestsToRun(IReadOnlyList<string> changedCodeSignatures)
        {
            var testImpactQueryMethodConfig = _configuration["testImpact.query"];
            bool runInMemory = testImpactQueryMethodConfig.Contains("memory", StringComparison.OrdinalIgnoreCase);

            var impactedOrAlreadyShouldRun =
                runInMemory
                    ? await FetchToMemeoryAndFindTestsToRun(changedCodeSignatures)
                    : await UseDbQueryToFindTestsToRun(changedCodeSignatures);

            var impactedTestsLastStates = impactedOrAlreadyShouldRun
                .Select(t => t.TestLastState)
                .Where(t => !t.ShouldBeRun)
                .ToList();
            UpdateLastStateToImpacted(impactedTestsLastStates);

            List<TestCase> newTests = await GetNewTestCases(_project);
            var newTestsToRun = AddNewTestsLastStates(newTests);

            return impactedOrAlreadyShouldRun
                .Concat(newTestsToRun)
                .ToList();
        }

        public async Task<IReadOnlyList<TestToRun>> UpdateAndGetAllTestsToRun()
        {
            var currentLastStates = await GetTestLastStatesAsTestToRun();
            foreach (var l in currentLastStates)
                l.RunReason = RunReason.ForceRun;
            List<TestCase> newTests = await GetNewTestCases(_project);
            var newTestsToRun = AddNewTestsLastStates(newTests);
            return currentLastStates.Concat(newTestsToRun).ToList();
        }

        private IQueryable<TestLastState> GetTestLastStates()
        {
            return _dbService.Query<TestLastState>(tl =>
                  tl.AzureProductBuildDefinitionId == _azureBuildDefId);
        }

        private async Task<List<TestToRun>> UseDbQueryToFindTestsToRun(
                IReadOnlyList<string> changedCodeSignatures)
        {
            var impactedTestCases =
                from tc in _dbService.Query<TestCase>()
                where
                    tc.TeamProject == _buildInfo.TeamProject &&
                    _dbService.Query<TestCaseImpactCodeSignature>().Any(tci =>
                       tci.TestCaseId == tc.Id &&
                       tci.AzureProductBuildDefinitionId == _azureBuildDefId &&
                       !tci.IsDeleted &&
                       changedCodeSignatures.Contains(tci.Signature))
                select tc;

            var impactedOrAlreadyShouldRun =
                await (from tls in GetTestLastStates()
                       join it in impactedTestCases on tls.TestCaseId equals it.Id
                       into jit
                       from it in jit.DefaultIfEmpty()
                       where it != null || tls.ShouldBeRun
                       select new TestToRun()
                       {
                           TestLastState = tls,
                           AzureTestCaseId = tls.TestCase.AzureTestCaseId
                       }).ToListAsync();

            return impactedOrAlreadyShouldRun;
        }

        private async Task<List<TestToRun>> FetchToMemeoryAndFindTestsToRun(
                IReadOnlyList<string> changedCodeSignatures)
        {
            var impactedTestsLastStates = new List<TestLastState>();

            var allSignatures = await _dbService
                .Query<TestCaseImpactCodeSignature>(tis =>
                    tis.AzureProductBuildDefinitionId == _azureBuildDefId &&
                    !tis.IsDeleted)
                .Select(tis => new { tis.TestCaseId, tis.Signature })
                .ToListAsync();
            var changeSignaturesSet = new HashSet<string>(changedCodeSignatures);

            HashSet<int> impactedTestCaseIds = new HashSet<int>();
            foreach (var sig in allSignatures)
                if (changeSignaturesSet.Contains(sig.Signature))
                    impactedTestCaseIds.Add(sig.TestCaseId);

            List<TestToRun> allTestLastStates = await GetTestLastStatesAsTestToRun();

            var allTestsToRun = allTestLastStates
                .Where(tls => impactedTestCaseIds.Contains(tls.TestLastState.TestCaseId) || tls.TestLastState.ShouldBeRun)
                .ToList();

            return allTestsToRun;
        }

        private async Task<List<TestToRun>> GetTestLastStatesAsTestToRun()
        {
            return await GetTestLastStates()
                .Select(tls =>
                    new TestToRun()
                    {
                        TestLastState = tls,
                        AzureTestCaseId = tls.TestCase.AzureTestCaseId
                    })
                .ToListAsync();
        }

        private async Task<List<TestCase>> GetNewTestCases(string project)
        {
            var allTestLastStates = GetTestLastStates();
            return await (from tc in _dbService.Query<TestCase>(tc => tc.TeamProject == project)
                          where !allTestLastStates.Any(lt => lt.TestCaseId == tc.Id)
                          select tc).ToListAsync();
        }

        private List<TestToRun> AddNewTestsLastStates(List<TestCase> newTests)
        {
            var lastStates = new List<TestToRun>();
            foreach (var nt in newTests)
            {
                var lastState =
                    new TestLastState()
                    {
                        AzureProductBuildDefinitionId = _azureBuildDefId,
                        TestCase = nt,
                        ProductBuildInfo = _buildInfo,

                        RunReason = RunReason.NotRan,
                        ShouldBeRun = true,
                        UpdateDate = DateTime.Now
                    };
                _dbService.Add(lastState);
                lastStates.Add(
                    new TestToRun()
                    {
                        TestLastState = lastState,
                        AzureTestCaseId = nt.AzureTestCaseId
                    });
            }
            return lastStates;
        }

        private void UpdateLastStateToImpacted(IReadOnlyCollection<TestLastState> testLastStates)
        {
            UpdateLastStates(testLastStates, RunReason.Impacted);
        }

        private void UpdateLastStates(IReadOnlyCollection<TestLastState> testLastStates, RunReason runReason)
        {
            foreach (var state in testLastStates)
            {
                state.UpdateDate = DateTime.Now;
                state.ProductBuildInfo = _buildInfo;
                state.ShouldBeRun = true;
                state.RunReason = runReason;
            }
        }
    }
}
