using Microsoft.EntityFrameworkCore;
using AdditionApi.Models;

public class Storage
{
    public async Task SaveValueAsync(string key, string value, YourDbContext db)
    {
        var item = await db.StorageItems.FirstOrDefaultAsync(i => i.Key == key);
        if (item != null)
            item.Value = value;
        else
            await db.StorageItems.AddAsync(new StorageItem { Key = key, Value = value });

        await db.SaveChangesAsync();
    }

    public async Task<string?> GetValueAsync(string key, YourDbContext db)
    {
        var item = await db.StorageItems.FirstOrDefaultAsync(i => i.Key == key);
        return item?.Value;
    }
}
