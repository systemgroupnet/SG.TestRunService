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
        Task<IReadOnlyList<LastImpactUpdateResponse>> GetLastImpactUpdatesAsync();
        Task<LastImpactUpdateResponse> GetLastImpactUpdateAsync(int azureProductBuildDefId);
        Task<IReadOnlyList<TestToRunResponse>> PublishImpactChangesAsync(PublishImpactChangesRequest request);
        Task UpdateTestCaseImpactAsync(int testCaseId, TestCaseImpactUpdateRequest request);
        Task<ServiceError> UpdateTestLastStateAsync(int testCaseId, TestLastStateUpdateRequest lastStateUpdateRequest);
        Task<IReadOnlyList<TestLastStateResponse>> GetTestLastStatesAsync(int testCaseId);
        Task<LastImpactUpdateResponse> DeleteLastImpactUpdateAsync(int azureProductBuildDefId);
        Task<TestLastStateResponse> DeleteTestLastStateAsync(int testCaseId, int azureProductBuildDefId);
    }
}
