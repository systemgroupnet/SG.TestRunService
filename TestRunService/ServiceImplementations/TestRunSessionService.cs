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

        public async Task InsertSessionAsync(TestRunSessionRequest session)
        {
            await _sessionDbService.InsertAsync(_mapper.Map<TestRunSession>(session));
        }
    }
}
