﻿using Microsoft.EntityFrameworkCore;
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
    internal class TestLastStateUpdater
    {
        private readonly IBaseDbService _dbService;
        private readonly Data.BuildInfo _buildInfo;
        private readonly IConfiguration _configuration;
        private readonly string _project;
        private readonly int _azureBuildDefId;

        public TestLastStateUpdater(IBaseDbService dbService, IConfiguration configuration, Data.BuildInfo testRunSessionBuildInfo)
        {
            _dbService = dbService;
            _configuration = configuration;
            _buildInfo = testRunSessionBuildInfo;
            _project = _buildInfo.TeamProject;
            _azureBuildDefId = _buildInfo.AzureBuildDefinitionId;
        }

        public async Task<List<TestToRun>> UpdateAndGetTestsToRun(IReadOnlyList<string> changedCodeSignatures)
        {
            var allTestLastStatesQuery =
                _dbService.Query<TestLastState>(tl =>
                  tl.AzureProductBuildDefinitionId == _azureBuildDefId);

            var testImpactQueryMethodConfig = _configuration["testImpact.query"];
            bool runInMemory = testImpactQueryMethodConfig.Contains("memory", StringComparison.OrdinalIgnoreCase);

            var impactedOrAlreadyShouldRun =
                runInMemory
                    ? await FetchToMemeoryAndFindTestsToRun(changedCodeSignatures, allTestLastStatesQuery)
                    : await UseDbQueryToFindTestsToRun(changedCodeSignatures, allTestLastStatesQuery);

            var impactedTestsLastStates = impactedOrAlreadyShouldRun
                .Select(t => t.TestLastState)
                .Where(t => !t.ShouldBeRun)
                .ToList();
            UpdateLastStateToImpacted(impactedTestsLastStates);

            List<TestCase> newTests = await GetNewTestCases(_project, allTestLastStatesQuery);
            var newTestsToRun = AddNewTestsLastStates(newTests);

            return impactedOrAlreadyShouldRun
                .Concat(newTestsToRun)
                .ToList();
        }

        private async Task<List<TestToRun>> UseDbQueryToFindTestsToRun(
                IReadOnlyList<string> changedCodeSignatures, IQueryable<TestLastState> allTestLastStatesQuery)
        {
            var impactedTestCases =
                from tc in _dbService.Query<TestCase>()
                where
                    tc.TeamProject == _buildInfo.TeamProject &&
                    _dbService.Query<TestCaseImpactCodeSignature>().Any(tci =>
                       tci.TestCaseId == tc.Id &&
                       (tci.AzureProductBuildDefinitionId == _azureBuildDefId ||
                       changedCodeSignatures.Contains(tci.Signature)))
                select tc;

            var impactedOrAlreadyShouldRun =
                await (from tls in allTestLastStatesQuery
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
                IReadOnlyList<string> changedCodeSignatures, IQueryable<TestLastState> allTestLastStatesQuery)
        {
            var impactedTestsLastStates = new List<TestLastState>();

            var allSignatures = await _dbService
                .Query<TestCaseImpactCodeSignature>(tis =>
                    tis.AzureProductBuildDefinitionId == _azureBuildDefId)
                .Select(tis => new { tis.TestCaseId, tis.Signature })
                .ToListAsync();
            var changeSignaturesSet = new HashSet<string>(changedCodeSignatures);

            HashSet<int> impactedTestCaseIds = new HashSet<int>();
            foreach (var sig in allSignatures)
                if (changeSignaturesSet.Contains(sig.Signature))
                    impactedTestCaseIds.Add(sig.TestCaseId);

            var allTestLastStates = await allTestLastStatesQuery
                .Select(tls =>
                    new TestToRun()
                    {
                        TestLastState = tls,
                        AzureTestCaseId = tls.TestCase.AzureTestCaseId
                    })
                .ToListAsync();

            var allTestsToRun = allTestLastStates
                .Where(tls => impactedTestCaseIds.Contains(tls.TestLastState.TestCaseId) || tls.TestLastState.ShouldBeRun)
                .ToList();

            return allTestsToRun;
        }

        private async Task<List<TestCase>> GetNewTestCases(string project, IQueryable<TestLastState> allTestLastStates)
        {
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

        private void UpdateLastStateToImpacted(List<TestLastState> testLastStates)
        {
            foreach (var state in testLastStates)
            {
                state.UpdateDate = DateTime.Now;
                state.ProductBuildInfo = _buildInfo;
                state.ShouldBeRun = true;
                state.RunReason = RunReason.Impacted;
            }
        }
    }
}