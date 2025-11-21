[ApiController]
[Route("storage")]
public class StorageController : ControllerBase
{
    private readonly StorageService _storage;

    public StorageController(StorageService storage)
    {
        _storage = storage;
    }

    [HttpPost("{key}")]
    public async Task<IActionResult> SetItem(string key, [FromBody] StorageRequest request)
    {
        if (request?.Value == null)
            return BadRequest("Value is required.");

        await _storage.SetItemAsync(key, request.Value);

        return Ok(new { message = "Stored successfully", key });
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetItem(string key)
    {
        var value = await _storage.GetItemAsync(key);

        if (value == null)
            return NotFound(new { message = "Key not found", key });

        return Ok(new { key, value });
    }
}
