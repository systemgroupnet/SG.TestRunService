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
        public async Task<ActionResult> Insert(TestRunSessionRequest session)
        {
            await _service.InsertSessionAsync(session);
            return CreatedAtAction(nameof(GetById), session.);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int sessionId)
        {
            return Ok(await _service.DeleteSessionAsync(sessionId));
        }

        [HttpGet("")]
        public async Task<ActionResult> GetAll()
        {
            return new JsonResult(await _service.GetAllSessionsAsync());
        }

        [HttpGet("{sessionId:int}")]
        public async Task<ActionResult> GetById(int sessionId)
        {
            return new JsonResult(await _service.GetSessionAsync(sessionId));
        }

        [HttpGet("{sessionId:int}/runs")]
        public async Task<ActionResult> GetAllTestRuns(int sessionId)
        {
            return new JsonResult(await _service.GetSessionTestRunsAsync(sessionId));
        }

        [HttpPost("{sessionId:int}/runs")]
        public async Task<ActionResult> InsertTestRun(int sessionId, TestRunDto testRun)
        {

            return Created();
        }
    }
}