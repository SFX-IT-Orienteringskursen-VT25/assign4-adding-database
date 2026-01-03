namespace AdditionApi.Services;

public interface IStorageService
{
    object? GetItem(string key);
    /// <returns>true if key existed before, false if new</returns>
    bool SetItem(string key, object value);
}

