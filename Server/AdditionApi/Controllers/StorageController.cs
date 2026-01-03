using AdditionApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdditionApi.Controllers;

[ApiController]
[Route("storage")]
public class StorageController : ControllerBase
{
    private readonly IStorageService _storage;
    public StorageController(IStorageService storage) => _storage = storage;

    public record SetValueRequest(object? Value);

    // GET /storage/{key}  -> like localStorage.getItem(key)
    [HttpGet("{key}")]
    public IActionResult Get(string key)
    {
        var value = _storage.GetItem(key);
        return value is null
            ? NotFound(new { error = "Not Found" })           // 404
            : Ok(new { key, value });                          // 200
    }

    // PUT /storage/{key}  -> like localStorage.setItem(key, value)
    [HttpPut("{key}")]
    public IActionResult Put(string key, [FromBody] SetValueRequest body)
    {
        if (body is null || body.Value is null)
            return BadRequest(new { error = "value is required" }); // 400

        var existed = _storage.SetItem(key, body.Value);
        var payload = new { key, value = body.Value, created = !existed };

        return existed
            ? Ok(payload)                                              // 200 (updated)
            : CreatedAtAction(nameof(Get), new { key }, payload);      // 201 (created)
    }
}
