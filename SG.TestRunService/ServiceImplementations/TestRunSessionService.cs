using Microsoft.EntityFrameworkCore;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using SG.TestRunService.Infrastructure;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations
{
    public class TestRunSessionService : ITestRunSessionService
    {
        private readonly IBaseDbService _dbService;

        public TestRunSessionService(IBaseDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<TestRunSessionResponse> InsertSessionAsync(TestRunSessionRequest sessionDto)
        {
            var session = sessionDto.ToDataModel();
            await _dbService.InsertAsync(session);
            return session.ToResponse();
        }

        public async Task<TestRunSessionResponse> DeleteSessionAsync(int sessionId)
        {
            var session = await _dbService.DeleteAsync<TestRunSession>(sessionId);
            return session.ToResponse();
        }

        public async Task<IReadOnlyList<TestRunSessionResponse>> GetAllSessionsAsync()
        {
            return await _dbService.Query<TestRunSession>().Project().ToListAsync();
        }

        public Task<TestRunSessionResponse> GetSessionAsync(int sessionId)
        {
            return _dbService.Query<TestRunSession>(sessionId).Project().FirstOrDefaultAsync();
        }

        public async Task<TestRunResponse> InsertTestRunAsync(int sessionId, TestRunRequest testRunRequest)
        {
            var testRun = testRunRequest.ToDataModel(sessionId);
            await _dbService.InsertAsync(testRun);
            return testRun.ToResponse();
        }

        public async Task<IReadOnlyList<TestRunResponse>> GetSessionTestRunsAsync(int sessionId)
        {
            return await _dbService.Query<TestRun>(r => r.TestRunSessionId == sessionId)
                .Project().ToListAsync();
        }

        public Task<TestRunResponse> GetTestRunAsync(int testRunId)
        {
            return _dbService.Query<TestRun>(testRunId).Project().FirstOrDefaultAsync();
        }
    }
}

