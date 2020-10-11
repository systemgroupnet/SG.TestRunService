using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SG.TestRunService.Services;

namespace SG.TestRunService.Controllers
{
    [Route(RouteConstants.ProductLine)]
    [ApiController]
    public class ProductLineController : Controller
    {
        private readonly IProductLineService _productLineService;

        public ProductLineController(IProductLineService productLineService)
        {
            _productLineService = productLineService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var (result, _) = await _productLineService.GetAllProductLines();
            return Ok(result);
        }
    }
}