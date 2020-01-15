using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Models;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Controllers
{
    [Route("api/runs")]
    [ApiController]
    public class TestRunsController : ControllerBase
    {
        readonly ITestRunSessionService _service;

        public TestRunsController(ITestRunSessionService service)
        {
            _service = service;
        }

        [HttpGet("{id:int}")]
        public async Task<TestRunResponse> GetById(int id)
        {
            return await _service.GetTestRunAsync(id);
        }
    }
}

