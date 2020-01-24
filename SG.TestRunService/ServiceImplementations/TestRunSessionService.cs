using Microsoft.EntityFrameworkCore;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using SG.TestRunService.Data.Services;
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
            var build = await _dbService.Query<Data.BuildInfo>(b =>
                   b.AzureBuildDefinitionId == sessionDto.ProductBuild.AzureBuildDefinitionId &&
                   b.AzureBuildId == sessionDto.ProductBuild.AzureBuildId)
                .FirstOrDefaultAsync();
            if(build == null)
            {
                build = sessionDto.ProductBuild.ToDataModel();
                _dbService.Add(build);
            }
            var session = sessionDto.ToDataModel();
            session.ProductBuildInfo = build;
            _dbService.Add(session);
            await _dbService.SaveChangesAsync();
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

        public async Task<(TestRunSessionResponse, ServiceError)> UpdateSessionAsync(int sessionId, Action<TestRunSessionRequest> sessionUpdater)
        {
            var session = _dbService.Query<TestRunSession>(sessionId).FirstOrDefault();
            if (session == null)
                return (null, ServiceError.NotFound($"No TestRunSession with Id of {sessionId} found."));
            var sessionRequest = session.ToRequest();
            sessionUpdater(sessionRequest);
            await _dbService.SaveChangesAsync();
            return (session.ToResponse(), null);
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

        public async Task<(TestRunResponse, ServiceError)> UpdateTestRunAsync(
            int sessionId, int testRunId, Action<TestRunRequest> testRunUpdater)
        {
            var testRun = _dbService.Query<TestRun>(testRunId).FirstOrDefault();
            if (testRun == null)
                return (null, ServiceError.NotFound($"No TestRun with Id of {testRunId} found."));
            if(testRun.TestRunSessionId != sessionId)
            {
                return (null, ServiceError.NotFound($"TestRun with Id of {testRunId} does not belong to session {sessionId}."));
            }
            var testRunRequest = testRun.ToRequest();
            testRunUpdater(testRunRequest);
            testRunRequest.Update(testRun);
            await _dbService.SaveChangesAsync();
            return (testRun.ToResponse(), null);
        }

        public async Task<(TestRunResponse, bool isNew, ServiceError)> ReplaceTestRun(
            int sessionId, int testRunId, TestRunRequest testRunRequest)
        {
            var testRun = _dbService.Query<TestRun>(testRunId).FirstOrDefault();
            bool isNew;
            if (testRun == null)
            {
                isNew = true;
                testRun = testRunRequest.ToDataModel(sessionId);
                _dbService.Add(testRun);
            }
            else
            {
                isNew = false;
                if (testRun.TestRunSessionId != sessionId)
                    return (null, false, ServiceError.NotFound($"TestRun with Id of {testRunId} does not belong to session {sessionId}."));
                testRunRequest.Update(testRun);
            }
            await _dbService.SaveChangesAsync();
            return (testRun.ToResponse(), isNew, null);
        }
    }
}

