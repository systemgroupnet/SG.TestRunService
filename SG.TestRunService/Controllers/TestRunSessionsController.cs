﻿using System;
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

        [HttpPatch("{sessionId:int}")]
        public async Task<IActionResult> UpdateSession(int sessionId, JsonPatchDocument<TestRunSessionRequest> patchDocument)
        {
            var (response, error) = await _service.UpdateSessionAsync(sessionId, s => patchDocument.ApplyTo(s, ModelState));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!error.IsSuccessful())
                return error.ToActionResult();
            return Ok(response);
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
            return CreatedAt(sessionId, testRun);
        }

        [HttpPost("{sessionId:int}/runs")]
        public async Task<IActionResult> InsertTestRuns(int sessionId, IEnumerable<TestRunRequest> testRunRequests)
        {
            var testRuns = await _service.InsertTestRunsAsync(sessionId, testRunRequests);
            return CreatedAtAction(nameof(GetAllTestRuns), new { sessionId = sessionId }, testRuns);
        }

        [HttpPatch("{sessionId:int}/runs/{id:int}")]
        public async Task<IActionResult> UpdateTestRun(int sessionId, int id,
            [FromBody]JsonPatchDocument<TestRunRequest> patchDocument)
        {
            var (response, error) = await _service.UpdateTestRunAsync(sessionId, id, r => patchDocument.ApplyTo(r, ModelState));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!error.IsSuccessful())
                return error.ToActionResult();
            return Ok(response);
        }

        [HttpPut("{sessionId:int}/runs/{id:int}")]
        public async Task<IActionResult> ReplaceTestRun(int sessionId, int id, TestRunRequest testRunRequest)
        {
            var (testRunResponse, isNew, error) = await _service.ReplaceTestRun(sessionId, id, testRunRequest);
            if (!error.IsSuccessful())
                return error.ToActionResult();
            if (isNew)
                return CreatedAt(sessionId, testRunResponse);
            else
                return Ok(testRunResponse);
        }

        public CreatedAtActionResult CreatedAt(int sessionId, TestRunResponse testRunResponse)
            => CreatedAtAction(
                    nameof(TestRunsController.GetById),
                    nameof(TestRunsController),
                    new { id = testRunResponse.Id }, testRunResponse);
    }
}

