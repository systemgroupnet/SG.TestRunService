using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using SG.TestRunService.Data.Services;
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

        public TestImpactService(IBaseDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IReadOnlyList<LastImpactUpdateResponse>> GetLastImpactUpdateAsync()
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
    }
}
