using SG.TestRunService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Services
{
    public interface ITestRunSessionService
    {
        Task<TestRunSessionResponse> InsertSessionAsync(TestRunSessionRequest session);
        Task<TestRunSessionResponse> DeleteSessionAsync(int sessionId);
        Task<IReadOnlyList<TestRunSessionResponse>> GetAllSessionsAsync();
        Task<TestRunSessionResponse> GetSessionAsync(int sessionId);
        Task<TestRunResponse> InsertTestRunAsync(int sessionId, TestRunRequest testRunRequest);
        Task<IReadOnlyList<TestRunResponse>> GetSessionTestRunsAsync(int sessionId);
        Task<TestRunResponse> GetTestRunAsync(int testRunId);
    }
}
