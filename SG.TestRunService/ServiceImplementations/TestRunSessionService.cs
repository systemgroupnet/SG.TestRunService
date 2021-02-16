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
        private readonly IProductLineService _productLineService;

        public TestRunSessionService(IBaseDbService dbService, IProductLineService productLineService)
        {
            _dbService = dbService;
            _productLineService = productLineService;
        }

        public async Task<(TestRunSessionResponse, ServiceError)> InsertSessionAsync(TestRunSessionRequest sessionDto)
        {
            var build = await _dbService.Query<Data.BuildInfo>(b =>
                   b.AzureBuildDefinitionId == sessionDto.ProductBuild.AzureBuildDefinitionId &&
                   b.AzureBuildId == sessionDto.ProductBuild.AzureBuildId)
                .FirstOrDefaultAsync();
            if (build == null)
            {
                build = sessionDto.ProductBuild.ToDataModel();
                _dbService.Add(build);
            }
            var (productLine, error) = await _productLineService.GetOrInsertProductLineAsync(sessionDto.ProductLine);

            if (!error.IsSuccessful())
                return (null, error);

            sessionDto.ProductLine = productLine.ToDto();
            var session = sessionDto.ToDataModel();
            session.ProductBuildInfo = build;
            _dbService.Add(session);
            await _dbService.SaveChangesAsync();
            var response = session.ToResponse();
            response.ProductLine = sessionDto.ProductLine;
            return (response, ServiceError.NoError);
        }

        public async Task<TestRunSessionResponse> DeleteSessionAsync(int sessionId)
        {
            var session = await _dbService.DeleteAsync<TestRunSession>(sessionId);
            if (session == null)
                return null;
            return session.ToResponse();
        }

        public async Task<IReadOnlyList<TestRunSessionResponse>> GetAllSessionsAsync()
        {
            return await _dbService.Query<TestRunSession>().MaterializeAllAsync();
        }

        public async Task<IReadOnlyList<TestRunSessionResponse>> GetAllSessionsAsync(SessionFilterRequest sessionFilter)
        {
            var query = _dbService.Query<TestRunSession>();


            query = sessionFilter.StartedBefore.HasValue ?
                    query.Where(x => x.StartTime < sessionFilter.StartedBefore.Value) : query;

            query = sessionFilter.StartedAfter.HasValue ?
                    query.Where(x =>  x.StartTime > sessionFilter.StartedAfter.Value) : query;

            query = sessionFilter.CompletedBefore.HasValue ?
                    query.Where(x => x.FinishTime.HasValue && x.FinishTime < sessionFilter.CompletedBefore.Value) : query;

            query = sessionFilter.CompletedAfter.HasValue ?
                    query.Where(x => !x.FinishTime.HasValue || x.FinishTime > sessionFilter.CompletedAfter.Value) : query;

            var result = await query.MaterializeAllAsync();

            var modifiedResult = sessionFilter.Skip.HasValue ? result.SkipLast(sessionFilter.Skip.Value) : result;

            modifiedResult = sessionFilter.Top.HasValue ? modifiedResult.TakeLast(sessionFilter.Top.Value) : modifiedResult;


            modifiedResult = string.IsNullOrWhiteSpace(sessionFilter.ProjectName) ? modifiedResult :
                modifiedResult.Where(x => string.Equals(x.ProductBuild.TeamProject, sessionFilter.ProjectName, StringComparison.OrdinalIgnoreCase));

            return modifiedResult.ToList();
        }

        public Task<TestRunSessionResponse> GetSessionAsync(int sessionId)
        {
            return _dbService.Query<TestRunSession>(sessionId).MaterializeFirstOrDefaultAsync();
        }

        public async Task<(TestRunSessionResponse, ServiceError)> UpdateSessionAsync(int sessionId, Action<TestRunSessionRequest> sessionUpdater)
        {
            var session = await _dbService.Query<TestRunSession>(sessionId).Include(s => s.ExtraData).FirstOrDefaultAsync();
            if (session == null)
                return (null, ServiceError.NotFound($"No TestRunSession with Id of {sessionId} found."));
            var sessionRequest = session.ToRequest();
            sessionUpdater(sessionRequest);
            sessionRequest.Update(session);
            await _dbService.SaveChangesAsync();
            return (session.ToResponse(), null);
        }

        public async Task<TestRunResponse> InsertTestRunAsync(int sessionId, TestRunRequest testRunRequest)
        {
            var testRun = testRunRequest.ToDataModel(sessionId);
            await _dbService.InsertAsync(testRun);
            return testRun.ToResponse();
        }

        public async Task<IReadOnlyCollection<TestRunResponse>> InsertTestRunsAsync(
            int sessionId, IEnumerable<TestRunRequest> testRunRequests)
        {
            var testRuns = testRunRequests.Select(t => t.ToDataModel(sessionId)).ToList();
            await _dbService.InsertAsync(testRuns);
            return testRuns.Select(t => t.ToResponse()).ToList();
        }

        public async Task<IReadOnlyList<TestRunResponse>> GetSessionTestRunsAsync(int sessionId)
        {
            return await _dbService.Query<TestRun>(r => r.TestRunSessionId == sessionId)
                .MaterializeAllAsync();
        }

        public Task<TestRunResponse> GetTestRunAsync(int testRunId)
        {
            return _dbService.Query<TestRun>(testRunId).MaterializeFirstOrDefaultAsync();
        }

        public async Task<(TestRunResponse, ServiceError)> UpdateTestRunAsync(
            int sessionId, int testRunId, Action<TestRunRequest> testRunUpdater)
        {
            var testRun = await _dbService.Query<TestRun>(testRunId).Include(r => r.ExtraData).FirstOrDefaultAsync();
            if (testRun == null)
                return (null, ServiceError.NotFound($"No TestRun with Id of {testRunId} found."));
            if (testRun.TestRunSessionId != sessionId)
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
            var testRun = await _dbService.Query<TestRun>(testRunId).Include(r => r.ExtraData).FirstOrDefaultAsync();
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

        public async Task<(TestRunResponse, ServiceError)> DeleteTestRunAsync(int sessionId, int testRunId)
        {
            var testRun = await _dbService.Query<TestRun>(testRunId).FirstOrDefaultAsync();
            if (testRun == null)
                return (null, ServiceError.NotFound(null));
            if (testRun.TestRunSessionId != sessionId)
                return (null, ServiceError.NotFound($"TestRun with Id of {testRunId} does not belong to session {sessionId}."));
            await _dbService.DeleteAsync(testRun);
            return (testRun.ToResponse(), ServiceError.NoError);
        }

        public async Task<IReadOnlyCollection<TestRunResponse>> DeleteTestRunsAsync(int sessionId)
        {
            var testRuns = await _dbService
                .Query<TestRun>(tr => tr.TestRunSessionId == sessionId)
                .ToListAsync();
            foreach (var tr in testRuns)
                _dbService.Remove(tr);
            await _dbService.SaveChangesAsync();
            return testRuns.Select(tr => tr.ToResponse()).ToList();
        }


    }


}

