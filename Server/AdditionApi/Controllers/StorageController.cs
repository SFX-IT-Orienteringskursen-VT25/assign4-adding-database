using Microsoft.AspNetCore.Mvc;
using assign3_addition_api.Models;
using Microsoft.EntityFrameworkCore;

namespace assign3_addition_api.Controllers
{
    [ApiController]
    [Route("storage")]
    public class StorageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StorageController(AppDbContext context)
        {
            _context = context;
        }

        // POST /storage
        [HttpPost]
        public async Task<IActionResult> SetItem([FromBody] StorageItem item)
        {
            var existing = await _context.StorageItems.FindAsync(item.Key);
            if (existing != null)
            {
                existing.Value = item.Value;
            }
            else
            {
                await _context.StorageItems.AddAsync(item);
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetItem), new { key = item.Key }, item);
        }

        // GET /storage/{key}
        [HttpGet("{key}")]
        public async Task<IActionResult> GetItem(string key)
        {
            var item = await _context.StorageItems.FindAsync(key);
            if (item != null)
            {
                return Ok(item.Value);
            }
            return NotFound();
        }
    }
}
