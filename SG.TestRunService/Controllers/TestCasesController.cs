using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SG.TestRunService.Controllers
{
    [Route(RouteConstants.TestCases)]
    [ApiController]
    public class TestCasesController : ControllerBase
    {
        private readonly ITestCaseService _service;

        public TestCasesController(ITestCaseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> Get(string fields = null)
        {
            if (fields == null)
                return Ok(await _service.GetAllAsync());

            var fieldNames = fields.Split(',');
            if (fieldNames.Length == 1 && fieldNames[0] == nameof(TestCaseResponse.AzureTestCaseId))
                return Ok(await _service.GetAzureTestCaseIdsAsync());
            if (fieldNames.Length == 2 &&
                fieldNames.Contains(nameof(TestCaseResponse.Id)) &&
                fieldNames.Contains(nameof(TestCaseResponse.AzureTestCaseId)))
            {
                return Ok(await _service.GetAllAsync(fieldNames));
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
        public async Task<ActionResult> Insert()
        {
            string jsonStr;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                jsonStr = await reader.ReadToEndAsync();
            }
            List<TestCaseRequest> testCaseRequests;
            try
            {
                if (jsonStr.StartsWith("["))
                    testCaseRequests = JsonConvert.DeserializeObject<List<TestCaseRequest>>(jsonStr);
                else if (jsonStr.StartsWith("{"))
                    testCaseRequests = new List<TestCaseRequest>() { JsonConvert.DeserializeObject<TestCaseRequest>(jsonStr) };
                else
                    return BadRequest("Please post a valid JSON object.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var responses = await _service.InsertAsync(testCaseRequests);
            if (responses.Count == 1)
                return CreatedAtAction(nameof(Get), new { id = responses[0].Id }, responses[0]);
            else
                return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<TestCaseResponse>> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            if (response == null)
                return NotFound();
            return Ok(response);
        }
    }
}
