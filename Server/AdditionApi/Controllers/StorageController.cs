using System.Text.Json;
using AdditionApi.Data;
using AdditionApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdditionApi.Controllers;

[ApiController]
[Route("storage")]
public class StorageController : ControllerBase
{
    private readonly AppDbContext _db;

    public StorageController(AppDbContext db) => _db = db;

    // GET /storage/{key}
    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key)
    {
        var item = await _db.StoredItems.FindAsync(key);
        if (item is null) return NotFound(new { error = "Not Found" });

        var doc = JsonDocument.Parse(item.ValueJson);
        return Ok(new { key = item.Key, value = doc.RootElement.Clone() });
    }

    public record SetRequest(JsonElement value);

    // PUT /storage/{key}   body: { "value": ... }
    [HttpPut("{key}")]
    public async Task<IActionResult> Put(string key, [FromBody] SetRequest req)
    {
        if (req.value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return BadRequest(new { error = "value is required" });

        var json = req.value.GetRawText();
        var existing = await _db.StoredItems.FindAsync(key);

        if (existing is null)
        {
            _db.StoredItems.Add(new StoredItem { Key = key, ValueJson = json });
            await _db.SaveChangesAsync();
            return Created($"/storage/{key}", new { key, value = req.value, created = true });
        }

        existing.ValueJson = json;
        await _db.SaveChangesAsync();
        return Ok(new { key, value = req.value, created = false });
    }
}
