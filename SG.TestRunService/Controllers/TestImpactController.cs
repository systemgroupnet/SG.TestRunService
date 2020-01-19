using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Controllers
{
    [Route("api/impact")]
    [ApiController]
    public class TestImpactController : ControllerBase
    {
        private readonly ITestImpactService _service;

        public TestImpactController(ITestImpactService service)
        {
            _service = service;
        }

        [HttpGet("lastUpdate")]
        public Task<IReadOnlyList<LastImpactUpdateResponse>> GetLastUpdate()
        {
            return _service.GetLastImpactUpdateAsync();
        }

        [HttpGet("lastUpdate/{azureProductBuildDefId:int}")]
        public Task<LastImpactUpdateResponse> GetLastUpdate(int azureProductBuildDefId)
        {
            return _service.GetLastImpactUpdateAsync(azureProductBuildDefId);
        }
    }
}
