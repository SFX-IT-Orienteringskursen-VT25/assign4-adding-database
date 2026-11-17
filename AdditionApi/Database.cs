using Microsoft.Data.SqlClient;

namespace AdditionApi;

public class Database : IDatabase
{
    private const string TableName = "StorageItems";
    private const string DbName = "AdditionDatabase";

    public void Setup()
    {
        using var sqlConnection = CreateConnection();
        using var createDbCommand = sqlConnection.CreateCommand();
        createDbCommand.CommandText = $"IF DB_ID('{DbName}') IS NULL CREATE DATABASE {DbName};";
        createDbCommand.ExecuteNonQuery();

        using var createTableCommand = sqlConnection.CreateCommand();
        createTableCommand.CommandText = $@"
        USE {DbName};
        IF OBJECT_ID(N'{TableName}', N'U') IS NULL
        BEGIN
            CREATE TABLE {TableName} (
                [Key] NVARCHAR(200) PRIMARY KEY,
                [Value] NVARCHAR(MAX) NOT NULL
            );
        END";
        createTableCommand.ExecuteNonQuery();
    }

    // localStorage.setItem: insert the key, or overwrite the value if the key already exists
    public void SetItem(string key, string value)
    {
        using var sqlConnection = CreateConnection();
        using var command = sqlConnection.CreateCommand();
        command.CommandText = $@"
        USE {DbName};
        MERGE {TableName} WITH (HOLDLOCK) AS target
        USING (SELECT @key AS [Key], @value AS [Value]) AS source
            ON target.[Key] = source.[Key]
        WHEN MATCHED THEN UPDATE SET [Value] = source.[Value]
        WHEN NOT MATCHED THEN INSERT ([Key], [Value]) VALUES (source.[Key], source.[Value]);";
        command.Parameters.AddWithValue("@key", key);
        command.Parameters.AddWithValue("@value", value);
        command.ExecuteNonQuery();
    }

    // localStorage.getItem: returns null when the key does not exist
    public string? GetItem(string key)
    {
        using var sqlConnection = CreateConnection();
        using var command = sqlConnection.CreateCommand();
        command.CommandText = $@"
        USE {DbName};
        SELECT [Value] FROM {TableName} WHERE [Key] = @key;";
        command.Parameters.AddWithValue("@key", key);

        return command.ExecuteScalar() as string;
    }

    private static SqlConnection CreateConnection()
    {
        var sqlConnection = new SqlConnection($"Server=localhost,1433;Database=master;User Id=sa;Password={SqlCredentials.Password};TrustServerCertificate=True;");
        sqlConnection.Open();

        return sqlConnection;
    }
}
