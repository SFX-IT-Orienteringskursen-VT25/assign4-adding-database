using Microsoft.Data.SqlClient;

namespace AdditionApi;

public static class Database
{
    private const string TableName = "MyTable";
    private const string DbName = "MyDatabase";

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
                [Value] VARCHAR(50) NOT NULL
            );
        END";
        createTableCommand.ExecuteNonQuery();
    }

    public static void InsertValue(string value)
    {
        using var sqlConnection = CreateConnection();
        using var insertCommand = sqlConnection.CreateCommand();
        insertCommand.CommandText = $@"
        USE {DbName};
        INSERT INTO {TableName} ([Value]) VALUES (@value);";
        insertCommand.Parameters.AddWithValue("@value", value);
        insertCommand.ExecuteNonQuery();

        //INSERT INTO dbo.MyTable ([Value])
        //VALUES ('First'), ('Second'), ('Third');
    }

    public static void Select()
    {
        using var sqlConnection = CreateConnection();
        using var insertCommand = sqlConnection.CreateCommand();
        insertCommand.CommandText = $@"
        USE {DbName};
        SELECT * FROM {TableName};";
        using var reader = insertCommand.ExecuteReader();

        var rowsInDb = new List<string>();
        while (reader.Read())
        {
            var id = reader["Id"].ToString();
            var value = reader["Value"].ToString();
            rowsInDb.Add($"{id}: {value}");
        }

        Console.WriteLine("Rows in database: " + rowsInDb.Count);
    }

    public static List<(int Id, string Value)> GetAllRows()
    {
        using var sqlConnection = CreateConnection();
        using var cmd = sqlConnection.CreateCommand();
        cmd.CommandText = $@"
        USE [{DbName}];
        SELECT [Id], [Value]
        FROM [{TableName}]
        ORDER BY [Id];";

        using var rdr = cmd.ExecuteReader();
        var rows = new List<(int, string)>();
        while (rdr.Read())
            rows.Add((rdr.GetInt32(0), rdr.GetString(1)));
        return rows;
    }


    public static void DeleteAll()
    {
        using var sqlConnection = CreateConnection();
        using var insertCommand = sqlConnection.CreateCommand();
        insertCommand.CommandText = $@"
        USE {DbName};
        DELETE FROM {TableName};";
        insertCommand.ExecuteNonQuery();
    }

    private static SqlConnection CreateConnection()
    {
        var sqlConnection = new SqlConnection($"Server=localhost,1433;Database={DbName};User Id=sa;Password={SqlCredentials.Password};TrustServerCertificate=True;");
        sqlConnection.Open();

        return sqlConnection;
    }
}