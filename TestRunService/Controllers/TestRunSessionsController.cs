using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Dto;
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
        public async Task<ActionResult> InsertSession(TestRunSessionDto session)
        {
            await _service.InsertSessionAsync(session);
            return Ok();
        }

        public async Task<ActionResult> DeleteSession(int sessionId)
        {
            await _service.DeleteSessionAsync(sessionId);
            return Ok();
        }
    }
}