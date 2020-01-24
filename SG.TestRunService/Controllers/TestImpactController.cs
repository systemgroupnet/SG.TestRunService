using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Controllers
{
    [Route(RoutConstants.Impact)]
    [ApiController]
    public class TestImpactController : ControllerBase
    {
        private readonly ITestImpactService _service;

        public TestImpactController(ITestImpactService service)
        {
            _service = service;
        }

        [HttpGet("lastUpdate")]
        public Task<IReadOnlyList<LastImpactUpdateResponse>> GetLastUpdates()
        {
            return _service.GetLastImpactUpdatesAsync();
        }

        [HttpGet("lastUpdate/{azureProductBuildDefId:int}")]
        public Task<LastImpactUpdateResponse> GetLastUpdate(int azureProductBuildDefId)
        {
            return _service.GetLastImpactUpdateAsync(azureProductBuildDefId);
        }

        [HttpPost("changes")]
        public async Task<PublishImpactChangesResponse> PublichChanges(PublishImpactChangesRequest request)
        {
            var testsToRun = await _service.PublishImpactChangesAsync(request);
            return new PublishImpactChangesResponse()
            {
                TestsToRun = testsToRun,
                TestsToRunCount = testsToRun.Count
            };
        }
    }
}
