using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Models;
using SG.TestRunService.Services;

namespace SG.TestRunService.Controllers
{
    [Route("api/sessions")]
    [ApiController]
    public class TestRunSessionsController : ControllerBase
    {
        readonly ITestRunSessionService _service;

        public TestRunSessionsController(ITestRunSessionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<TestRunSessionResponse>> Insert(TestRunSessionRequest session)
        {
            await _service.InsertSessionAsync(session);
            return CreatedAtAction(nameof(GetById), session.ToDataModel());
        }

        [HttpDelete]
        public async Task<ActionResult<TestRunSessionResponse>> Delete(int sessionId)
        {
            return Ok(await _service.DeleteSessionAsync(sessionId));
        }

        [HttpGet("")]
        public async Task<IReadOnlyList<TestRunSessionResponse>> GetAll()
        {
            return await _service.GetAllSessionsAsync();
        }

        [HttpGet("{sessionId:int}")]
        public async Task<TestRunSessionResponse> GetById(int sessionId)
        {
            return await _service.GetSessionAsync(sessionId);
        }

        [HttpGet("{sessionId:int}/runs")]
        public async Task<IReadOnlyList<TestRunResponse>> GetAllTestRuns(int sessionId)
        {
            return await _service.GetSessionTestRunsAsync(sessionId);
        }

        [HttpPost("{sessionId:int}/runs")]
        public async Task<ActionResult> InsertTestRun(int sessionId, TestRunRequest testRunRequest)
        {
            var testRun = await _service.InsertTestRunAsync(sessionId, testRunRequest);
            return CreatedAtAction(nameof(TestRunsController.GetById), nameof(TestRunsController), new { id = testRun.Id }, testRun);
        }
    }
}

