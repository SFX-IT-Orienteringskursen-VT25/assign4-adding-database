using Microsoft.AspNetCore.Mvc;
using AdditionApi.Data;
using AdditionApi.Models;

namespace AdditionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumbersController : ControllerBase
    {
        private readonly NumbersContext _context;

        public NumbersController(NumbersContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetNumbers() => Ok(_context.Numbers.ToList());

        [HttpPost]
        public IActionResult AddNumber([FromBody] Number number)
        {
            _context.Numbers.Add(number);
            _context.SaveChanges();
            return Ok(number);
        }
    }
}
