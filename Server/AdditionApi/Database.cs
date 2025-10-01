using Microsoft.Data.SqlClient;

namespace SetupMssqlExample;

public static class Database
{
    private const string TableName = "Numbers";
    private const string DbName = "NumbersDb";

    public static void Setup()
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
                [Id] INT PRIMARY KEY IDENTITY,
                [Value] INT NOT NULL
            );
        END";
        createTableCommand.ExecuteNonQuery();
    }

    public static List<int> GetNumbers()
    {
        using var sqlConnection = CreateConnection();
        using var command = sqlConnection.CreateCommand();
        command.CommandText = $@"USE {DbName}; SELECT [Value] FROM {TableName};";

        var numbers = new List<int>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            numbers.Add((int)reader["Value"]);
        }

        return numbers;
    }

    public static void SaveNumbers(List<int> numbers)
    {
        using var sqlConnection = CreateConnection();

        // First clear table
        using (var deleteCommand = sqlConnection.CreateCommand())
        {
            deleteCommand.CommandText = $@"USE {DbName}; DELETE FROM {TableName};";
            deleteCommand.ExecuteNonQuery();
        }

        // Insert new numbers
        foreach (var num in numbers)
        {
            using var insertCommand = sqlConnection.CreateCommand();
            insertCommand.CommandText = $@"USE {DbName}; INSERT INTO {TableName} ([Value]) VALUES (@val);";
            insertCommand.Parameters.AddWithValue("@val", num);
            insertCommand.ExecuteNonQuery();
        }
    }

    private static SqlConnection CreateConnection()
    {
        var sqlConnection = new SqlConnection(
            $"Server=localhost,1433;Database=master;User Id=sa;Password={SqlCredentials.Password};TrustServerCertificate=True;"
        );
        sqlConnection.Open();
        return sqlConnection;
    }
}
