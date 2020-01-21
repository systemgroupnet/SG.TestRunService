using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Common.Models;
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
        public async Task<ActionResult<TestRunSessionResponse>> Insert(TestRunSessionRequest sessionRequest)
        {
            var sessionResponse = await _service.InsertSessionAsync(sessionRequest);
            return CreatedAtAction(nameof(GetById), new { sessionId = sessionResponse.Id }, sessionResponse);
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
        public async Task<ActionResult<TestRunSessionResponse>> GetById(int sessionId)
        {
            var session = await _service.GetSessionAsync(sessionId);
            if (session == null)
                return NotFound();
            return session;
        }

        [HttpGet("{sessionId:int}/runs")]
        public async Task<IReadOnlyList<TestRunResponse>> GetAllTestRuns(int sessionId)
        {
            return await _service.GetSessionTestRunsAsync(sessionId);
        }

        [HttpPost("{sessionId:int}/runs")]
        public async Task<IActionResult> InsertTestRun(int sessionId, TestRunRequest testRunRequest)
        {
            var testRun = await _service.InsertTestRunAsync(sessionId, testRunRequest);
            return CreatedAtAction(nameof(TestRunsController.GetById), nameof(TestRunsController), new { id = testRun.Id }, testRun);
        }

        [HttpPatch("{sessionId:int}/runs/{id:int}")]
        public async Task<IActionResult> UpdateTestRun(int sessionId, int id,
            [FromBody]JsonPatchDocument<TestRunRequest> patchDocument)
        {
            var (response, errorCategory) = await _service.UpdateTestRunAsync(sessionId, id, r => patchDocument.ApplyTo(r));
            if (!errorCategory.IsSuccessful())
                return errorCategory.ToActionResult();
            return Ok(response);
        }
    }
}

