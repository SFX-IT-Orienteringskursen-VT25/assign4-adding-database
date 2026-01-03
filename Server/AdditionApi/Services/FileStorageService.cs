using System.Text.Json;

namespace AdditionApi.Services;

public class FileStorageService : IStorageService
{
    private readonly string _dbPath;
    private readonly object _lockObj = new();

    public FileStorageService(string contentRoot)
    {
        var dataDir = Path.Combine(contentRoot, "..", "data");
        Directory.CreateDirectory(dataDir);
        _dbPath = Path.Combine(dataDir, "storage.json");
        if (!File.Exists(_dbPath)) File.WriteAllText(_dbPath, "{}");
    }

    private Dictionary<string, object> ReadAll()
    {
        lock (_lockObj)
        {
            var raw = File.ReadAllText(_dbPath);
            return string.IsNullOrWhiteSpace(raw)
                ? new Dictionary<string, object>()
                : JsonSerializer.Deserialize<Dictionary<string, object>>(raw)
                  ?? new Dictionary<string, object>();
        }
    }

    private void WriteAll(Dictionary<string, object> map)
    {
        lock (_lockObj)
        {
            var json = JsonSerializer.Serialize(map, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_dbPath, json);
        }
    }

    public object? GetItem(string key)
    {
        var db = ReadAll();
        return db.TryGetValue(key, out var value) ? value : null;
    }

    // Return true if key existed (update), false if newly created
    public bool SetItem(string key, object value)
    {
        var db = ReadAll();
        var existed = db.ContainsKey(key);
        db[key] = value!;
        WriteAll(db);
        return existed;
    }
}
