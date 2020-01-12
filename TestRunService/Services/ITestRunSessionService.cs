using SG.TestRunService.Data;
using SG.TestRunService.DbServices;
using SG.TestRunService.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Services
{
    public interface ITestRunSessionService
    {
        Task InsertSessionAsync(TestRunSessionDto session);
        Task<TestRunSession> DeleteSessionAsync(int sessionId);
    }
}
