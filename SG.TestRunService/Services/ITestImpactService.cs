using SG.TestRunService.Common.Models;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Services
{
    public interface ITestImpactService
    {
        Task<IReadOnlyList<LastImpactUpdateResponse>> GetLastImpactUpdateAsync();
        Task<LastImpactUpdateResponse> GetLastImpactUpdateAsync(int azureProductBuildDefId);
    }
}
