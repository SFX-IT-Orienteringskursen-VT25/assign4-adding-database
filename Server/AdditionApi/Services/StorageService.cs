using Microsoft.EntityFrameworkCore;
using AdditionApi.Models;
using AdditionApi.Data;

namespace AdditionApi.Services
{
    public class StorageService : IStorageService
    {
        private readonly YourDbContext _context;

        public StorageService(YourDbContext context)
        {
            _context = context;
        }

        public async Task SaveValueAsync(string key, string value)
        {
            var item = await _context.StorageItems.FirstOrDefaultAsync(i => i.Key == key);
            
            if (item != null)
            {
                // Update existing item
                item.Value = value;
                item.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new item
                item = new StorageItem 
                { 
                    Key = key, 
                    Value = value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.StorageItems.AddAsync(item);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetValueAsync(string key)
        {
            var item = await _context.StorageItems.FirstOrDefaultAsync(i => i.Key == key);
            return item?.Value;
        }

        public async Task<bool> DeleteValueAsync(string key)
        {
            var item = await _context.StorageItems.FirstOrDefaultAsync(i => i.Key == key);
            if (item == null) return false;

            _context.StorageItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StorageItem>> GetAllItemsAsync()
        {
            return await _context.StorageItems.ToListAsync();
        }
    }
}