using SG.TestRunService.Data;
using SG.TestRunService.DbServices;
using SG.TestRunService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Services
{
    public interface ITestRunSessionService
    {
        Task InsertSessionAsync(TestRunSessionRequest session);
        Task<TestRunSessionRequest> DeleteSessionAsync(int sessionId);
        Task<IReadOnlyList<TestRunSessionRequest>> GetAllSessionsAsync();
        Task<TestRunSessionRequest> GetSessionAsync(int sessionId);
        Task<IReadOnlyList<TestRunDto>> GetSessionTestRunsAsync(int sessionId);
    }
}
