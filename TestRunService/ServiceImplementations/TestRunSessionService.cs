using SG.TestRunService.Data;
using SG.TestRunService.DbServices;
using SG.TestRunService.Models;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations
{
    public class TestRunSessionService : ITestRunSessionService
    {
        private readonly IEntityDbService<TestRunSession> _sessionDbService;
        private readonly IEntityDbService<TestRun> _testRunDbService;

        public TestRunSessionService(IEntityDbService<TestRunSession> sessionDbService, IEntityDbService<TestRun> testRunDbService)
        {
            _sessionDbService = sessionDbService;
            _testRunDbService = testRunDbService;
        }

        public async Task<TestRunSessionResponse> InsertSessionAsync(TestRunSessionRequest sessionDto)
        {
            var session = sessionDto.ToDataModel();
            await _sessionDbService.InsertAsync(session);
            return TestRunSessionResponse.FromDataModel(session);
        }

        public async Task<TestRunSessionResponse> DeleteSessionAsync(int sessionId)
        {
            return TestRunSessionResponse.FromDataModel(
                await _sessionDbService.DeleteAsync(sessionId));
        }

        public Task<IReadOnlyList<TestRunSessionResponse>> GetAllSessionsAsync()
        {
            return _sessionDbService.GetAllAsync(TestRunSessionResponse.Project);
        }

        public Task<TestRunSessionResponse> GetSessionAsync(int sessionId)
        {
            return _sessionDbService.GetFirstOrDefaultAsync(
                s => s.Id == sessionId,
                TestRunSessionResponse.Project);
        }

        public Task<IReadOnlyList<TestRunResponse>> GetSessionTestRunsAsync(int sessionId)
        {
            return _testRunDbService.GetFilteredAsync(r => r.TestRunSessionId == sessionId, TestRunResponse.Project);
        }

        public Task<TestRunResponse> GetTestRunAsync(int testRunId)
        {
            return _testRunDbService.GetById(testRunId, TestRunResponse.Project);
        }
    }
}

