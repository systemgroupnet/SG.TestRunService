using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Services
{
    public interface ITestRunSessionService
    {
        Task<(TestRunSessionResponse, ServiceError)> InsertSessionAsync(TestRunSessionRequest sessionDto);
        Task<TestRunSessionResponse> DeleteSessionAsync(int sessionId);
        Task<IReadOnlyList<TestRunSessionResponse>> GetAllSessionsAsync();
        Task<TestRunSessionResponse> GetSessionAsync(int sessionId);
        Task<TestRunResponse> InsertTestRunAsync(int sessionId, TestRunRequest testRunRequest);
        Task<IReadOnlyCollection<TestRunResponse>> InsertTestRunsAsync(int sessionId, IEnumerable<TestRunRequest> testRunRequests);
        Task<IReadOnlyList<TestRunResponse>> GetSessionTestRunsAsync(int sessionId);
        Task<TestRunResponse> GetTestRunAsync(int testRunId);
        Task<(TestRunSessionResponse, ServiceError)> UpdateSessionAsync(int sessionId, Action<TestRunSessionRequest> sessionUpdater);
        Task<(TestRunResponse, ServiceError)> UpdateTestRunAsync(int sessionId, int testRunId, Action<TestRunRequest> testRunUpdater);
        Task<(TestRunResponse, bool isNew, ServiceError)> ReplaceTestRun(int sessionId, int testRunId, TestRunRequest testRunRequest);
        Task<(TestRunResponse, ServiceError)> DeleteTestRunAsync(int sessionId, int testRunId);
        Task<IReadOnlyCollection<TestRunResponse>> DeleteTestRunsAsync(int sessionId);
    }
}
