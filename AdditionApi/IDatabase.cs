namespace AdditionApi;

public interface IDatabase
{
    void Setup();
    void SetItem(string key, string value);
    string? GetItem(string key);
}
