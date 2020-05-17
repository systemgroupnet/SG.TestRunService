using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Controllers
{
    [Route(RouteConstants.Impact)]
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

        [HttpDelete("lastUpdate/{azureProductBuildDefId:int}")]
        public async Task<IActionResult> DeleteLastUpdate(int azureProductBuildDefId)
        {
            var response = await _service.DeleteLastImpactUpdateAsync(azureProductBuildDefId);
            if (response == null)
                return NotFound();
            return Ok(response);
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

        [HttpPost("testrun/{testCaseId:int}")]
        public async Task<ActionResult> UpdateTestCaseImpact(int testCaseId, TestCaseImpactUpdateRequest request)
        {
            await _service.UpdateTestCaseImpactAsync(testCaseId, request);
            return Ok();
        }

        [HttpGet("lastState/{testCaseId:int}")]
        public async Task<ActionResult<IReadOnlyCollection<TestLastStateResponse>>> GetTestLastStates(int testCaseId)
        {
            var result = await _service.GetTestLastStatesAsync(testCaseId);
            if (result.Count == 0)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("lastState/{testCaseId:int}")]
        public async Task<IActionResult> UpdateTestLastState(int testCaseId, TestLastStateUpdateRequest request)
        {
            var error = await _service.UpdateTestLastStateAsync(testCaseId, request);
            if (!error.IsSuccessful())
                return error.ToActionResult();
            return Ok();
        }

        [HttpDelete("lastState/{testCaseId:int}")]
        public async Task<IActionResult> DeleteTestLastState(int testCaseId, int azureProductBuildDefId)
        {
            var response = await _service.DeleteTestLastStateAsync(testCaseId, azureProductBuildDefId);
            if (response == null)
                return NotFound();
            return Ok(response);
        }
    }
}
