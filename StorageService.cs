using Microsoft.EntityFrameworkCore;

public class StorageService
{
    private readonly StorageDbContext _db;

    public StorageService(StorageDbContext db)
    {
        _db = db;
    }

    public async Task SetItemAsync(string key, string value)
    {
        var item = await _db.StorageItems.FindAsync(key);

        if (item == null)
        {
            item = new StorageItem { Key = key, Value = value };
            _db.StorageItems.Add(item);
        }
        else
        {
            item.Value = value;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<string?> GetItemAsync(string key)
    {
        var item = await _db.StorageItems.FindAsync(key);
        return item?.Value;
    }
}
