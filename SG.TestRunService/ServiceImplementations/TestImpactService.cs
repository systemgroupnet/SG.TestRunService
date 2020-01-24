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

            var tlsUpdater = new TestLastStateUpdater(_dbService, _configuration, testRunSessionBuildInfo);
            var testLastStatesToRun = await tlsUpdater.UpdateAndGetTestsToRun(request.Changes.Select(c => c.Signature).ToList());

            var response = testLastStatesToRun
                .Select(t =>
                    new TestToRunResponse()
                    {
                        Id = t.TestLastState.TestCaseId,
                        AzureTestCaseId = t.AzureTestCaseId,
                        RunReason = t.TestLastState.RunReason.Value
                    })
                .ToList();
            return response;
        }
    }
}
