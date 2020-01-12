using AutoMapper;
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
        private readonly IMapper _mapper;

        public TestRunSessionService(IEntityDbService<TestRunSession> sessionDbService, IEntityDbService<TestRun> testRunDbService, IMapper mapper)
        {
            _sessionDbService = sessionDbService;
            _testRunDbService = testRunDbService;
            _mapper = mapper;
        }

        public async Task<TestRunSession> InsertSessionAsync(TestRunSessionRequest sessionDto)
        {
            var session = _mapper.Map<TestRunSession>(sessionDto);
            await _sessionDbService.InsertAsync(session);
            return session;
        }

        public async Task<TestRunSessionRequest> DeleteSessionAsync(int sessionId)
        {
            return _mapper.Map<TestRunSessionRequest>(
                await _sessionDbService.DeleteAsync(sessionId));
        }

        public Task<IReadOnlyList<TestRunSessionRequest>> GetAllSessionsAsync()
        {
            return _sessionDbService.GetAllAsync<TestRunSessionRequest>(_mapper);
        }

        public Task<TestRunSessionRequest> GetSessionAsync(int sessionId)
        {
            return _sessionDbService.GetFirstOrDefaultAsync<TestRunSessionRequest>(s => s.Id == sessionId, _mapper);
        }

        public Task<IReadOnlyList<TestRunDto>> GetSessionTestRunsAsync(int sessionId)
        {
            return _testRunDbService.GetFilteredAsync<TestRunDto>(r => r.TestRunSessionId == sessionId, _mapper);
        }
    }
}

