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
        Task<(LastImpactUpdateResponse, ServiceError)> GetLastImpactUpdateAsync(ProductLineIdOrKey productLine);
        Task<(PublishImpactChangesResponse, ServiceError)> PublishImpactChangesAsync(PublishImpactChangesRequest request);
        Task<ServiceError> UpdateTestCaseImpactAsync(int testCaseId, TestCaseImpactUpdateRequest request);
        Task<ServiceError> UpdateTestLastStateAsync(int testCaseId, TestLastStateUpdateRequest lastStateUpdateRequest);
        Task<IReadOnlyList<TestLastStateResponse>> GetTestLastStatesAsync(int testCaseId);
        Task<(LastImpactUpdateResponse, ServiceError)> DeleteLastImpactUpdateAsync(ProductLineIdOrKey productLine);
        Task<(TestLastStateResponse, ServiceError)> DeleteTestLastStateAsync(int testCaseId, ProductLineIdOrKey productLine);
        Task<(IReadOnlyCollection<TestToRunResponse>, ServiceError)> GetTestsToRun(ProductLineIdOrKey productLine, bool allTests = false);
    }
}
