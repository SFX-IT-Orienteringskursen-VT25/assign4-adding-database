using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using AdditionApi.Models;

namespace AdditionApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculationController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var results = new List<string> { "example1", "example2" };
            return results;
        }
    }
}
