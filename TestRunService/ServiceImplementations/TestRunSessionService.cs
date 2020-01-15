using SG.TestRunService.Data;
using SG.TestRunService.Data.DbServices;
using SG.TestRunService.Infrastructure;
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
        private readonly IBaseDbService _baseDbService;

        public TestRunSessionService(IBaseDbService baseDbService)
        {
            _baseDbService = baseDbService;
        }

        public async Task<TestRunSessionResponse> InsertSessionAsync(TestRunSessionRequest sessionDto)
        {
            var session = sessionDto.ToDataModel();
            await _baseDbService.InsertAsync(session);
            return TestRunSessionResponse.FromDataModel(session);
        }

        public async Task<TestRunSessionResponse> DeleteSessionAsync(int sessionId)
        {
            return TestRunSessionResponse.FromDataModel(
                await _baseDbService.DeleteAsync<TestRunSession>(sessionId));
        }

        public Task<IReadOnlyList<TestRunSessionResponse>> GetAllSessionsAsync()
        {
            return _baseDbService.GetAllAsync<TestRunSession, TestRunSessionResponse>(TestRunSessionResponse.Project);
        }

        public Task<TestRunSessionResponse> GetSessionAsync(int sessionId)
        {
            return _baseDbService.GetById<TestRunSession, TestRunSessionResponse>(sessionId, TestRunSessionResponse.Project);
        }

        public async Task<TestRunResponse> InsertTestRunAsync(int sessionId, TestRunRequest testRunRequest)
        {
            var testRun = testRunRequest.ToDataModel(sessionId);
            await _baseDbService.InsertAsync(testRun);
            return TestRunResponse.From(testRun);
        }

        public Task<IReadOnlyList<TestRunResponse>> GetSessionTestRunsAsync(int sessionId)
        {
            return _baseDbService.GetFilteredAsync<TestRun, TestRunResponse>(
                r => r.TestRunSessionId == sessionId, TestRunResponse.Project);
        }

        public Task<TestRunResponse> GetTestRunAsync(int testRunId)
        {
            return _baseDbService.GetById<TestRun, TestRunResponse>(testRunId, TestRunResponse.Project);
        }
    }
}

