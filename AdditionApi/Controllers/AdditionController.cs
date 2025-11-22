using AdditionApi;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class AdditionController : ControllerBase
{
    private IDictionary<string,string> _storage;

    public AdditionController(IDictionary<string,string> storage)
    {
        _storage = storage;
    }

    [HttpGet("{key}")]
    public IActionResult GetMappings(string key)
    {
        if(_storage.TryGetValue(key, out var value))
        {
            return Ok(value);
        }
        return NotFound(new { Message = $"Key '{key}' not found." });
    }

    [HttpPost]
    public IActionResult PostMappings([FromBody] StorageRecord storageRecord)
    {
         if(_storage.ContainsKey(storageRecord.Key))
    {
        return Conflict(new { Message = $"Key '{storageRecord.Key}' already exists." });
    }
    _storage[storageRecord.Key] = storageRecord.Value;
    return Created($"/addition/{storageRecord.Key}", storageRecord.Value);
    }
}
