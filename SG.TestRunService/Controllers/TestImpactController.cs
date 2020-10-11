using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SG.TestRunService.Controllers
{
    [Route(RouteConstants.Impact)]
    [ApiController]
    public class TestImpactController : ControllerBase
    {
        private readonly ITestImpactService _service;
        private readonly IRetryFacility _retryFacility;

        public TestImpactController(ITestImpactService service, IRetryFacility retryFacility)
        {
            _service = service;
            _retryFacility = retryFacility;
        }

        [HttpGet("lastUpdate")]
        public async Task<IActionResult> GetLastUpdate(string productLineKey, int? productLineId)
        {
            if ((productLineKey != null) && (productLineId != null))
                return BadRequest($"Please specify only '{nameof(productLineKey)}' or '{nameof(productLineId)}'.");

            if ((productLineKey == null) && (productLineId == null))
                return Ok(await _service.GetLastImpactUpdatesAsync());

            var (response, error) = await _service.GetLastImpactUpdateAsync(
                new ProductLineIdOrKey()
                {
                    Id = productLineId,
                    Key = productLineKey
                });

            if (!error.IsSuccessful() && error.Category != ServiceErrorCategory.NotFound)
                return error.ToActionResult();

            return Ok(response);
        }

        [HttpDelete("lastUpdate")]
        public async Task<IActionResult> DeleteLastUpdate(string productLineKey, int? productLineId)
        {
            var (response, error) = await _service.DeleteLastImpactUpdateAsync(
                new ProductLineIdOrKey()
                {
                    Id = productLineId,
                    Key = productLineKey
                });
            if (!error.IsSuccessful())
                return error.ToActionResult();
            return Ok(response);
        }

        [HttpPost("changes")]
        public async Task<IActionResult> PublichChanges(PublishImpactChangesRequest request)
        {
            var (response, error) =
                await _retryFacility.RetryAsync(
                    operationName: "PublishChanges",
                    action: () => _service.PublishImpactChangesAsync(request));

            if (!error.IsSuccessful())
                return error.ToActionResult();
            return Ok(response);
        }

        [HttpGet("testsToRun")]
        public async Task<IActionResult> GetTestsToRun(
            string productLineKey, int? productLineId, bool? allTests)
        {
            var (response, error) =
                await _retryFacility.RetryAsync(
                    operationName: nameof(GetTestsToRun),
                    action: () => _service.GetTestsToRun(
                        new ProductLineIdOrKey()
                        {
                            Key = productLineKey,
                            Id = productLineId
                        },
                        allTests ?? false));

            if (!error.IsSuccessful())
                return error.ToActionResult();
            return Ok(response);
        }

        [HttpPost("testrun/{testCaseId:int}")]
        public async Task<IActionResult> UpdateTestCaseImpact(int testCaseId, TestCaseImpactUpdateRequest request)
        {
            var error = await _retryFacility.RetryAsync(
                operationName: nameof(UpdateTestCaseImpact),
                action: () => _service.UpdateTestCaseImpactAsync(testCaseId, request));
            return error.ToActionResult();
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
            return error.ToActionResult();
        }

        [HttpDelete("lastState/{testCaseId:int}")]
        public async Task<IActionResult> DeleteTestLastState(int testCaseId, string productLineKey, int? productLineId)
        {
            var (response, error) =
                await _service.DeleteTestLastStateAsync(
                    testCaseId,
                    new ProductLineIdOrKey()
                    {
                        Key = productLineKey,
                        Id = productLineId
                    });
            if (!error.IsSuccessful())
                return error.ToActionResult();
            return Ok(response);
        }


        [HttpGet("lineImpactStats")]
        public async Task<IActionResult> GetProdutionLineMethodImpactStats(string productLineKey, int? productLineId)
        {
            var (response, error) =
                await _service.GetProductLineMathodImpactStats(
                    new ProductLineIdOrKey
                    {
                        Key = productLineKey,
                        Id = productLineId
                    });
            if (!error.IsSuccessful())
                return error.ToActionResult();

            return Ok(response);
        }
    }
}
