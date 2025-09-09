using AdditionApi.Models;

namespace AdditionApi.Services
{
    public interface IStorageService
    {
        Task SaveValueAsync(string key, string value);
        Task<string?> GetValueAsync(string key);
        Task<bool> DeleteValueAsync(string key);
        Task<IEnumerable<StorageItem>> GetAllItemsAsync();
    }
}
