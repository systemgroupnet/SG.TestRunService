﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Controllers
{
    [Route(RoutConstants.TestCases)]
    [ApiController]
    public class TestCasesController : ControllerBase
    {
        private readonly ITestCaseService _service;

        public TestCasesController(ITestCaseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> Get(string project, string fields = null)
        {
            if (project == null)
                return StatusCode(
                    StatusCodes.Status422UnprocessableEntity,
                    new { message = "Project name should be specified in query ('?project=...')." });
            if (fields == null)
                return Ok(await _service.GetAllAsync(project));

            var fieldNames = fields.Split(',');
            if (fieldNames.Length == 1 && fieldNames[0] == nameof(TestCaseResponse.AzureTestCaseId))
                return Ok(await _service.GetAzureTestCaseIdsAsync(project));
            if(fieldNames.Length == 2 &&
                fieldNames.Contains(nameof(TestCaseResponse.Id)) &&
                fieldNames.Contains(nameof(TestCaseResponse.AzureTestCaseId)))
            {
                return Ok(await _service.GetAllAsync(project, fieldNames));
            }

            return StatusCode(StatusCodes.Status501NotImplemented);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TestCaseResponse>> Get(int id)
        {
            var tc = await _service.GetAsync(id);
            if (tc == null)
                return NotFound();
            return Ok(tc);
        }

        [HttpPost]
        public async Task<ActionResult> Insert(TestCaseRequest testCaseRequest)
        {
            var tc = await _service.InsertAsync(testCaseRequest);
            return CreatedAtAction(nameof(Get), new { id = tc.Id }, tc);
        }
    }
}
