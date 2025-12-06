using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace PersistentNumbers;

public static class Database
{
    private const string TableName = "Numbers";
    private const string DbName = "PersistentNumbers";

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
        using var cmd = sqlConnection.CreateCommand();
        cmd.CommandText = $@"
        USE {DbName};
        INSERT INTO {TableName} ([Value]) VALUES (@value);";
        cmd.Parameters.AddWithValue("@value", value);
        cmd.ExecuteNonQuery();
    }

   public static List<int> SelectNumbers()
{
    var numbers = new List<int>();

    using var sqlConnection = CreateConnection();
    using var cmd = sqlConnection.CreateCommand();
    cmd.CommandText = $@"
        USE {DbName};
        SELECT [Value] FROM {TableName};";

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        if (int.TryParse(reader["Value"].ToString(), out int num))
        {
            numbers.Add(num);
        }
    }

    return numbers;
}

    public static void DeleteAll()
    {
        using var sqlConnection = CreateConnection();
        using var cmd = sqlConnection.CreateCommand();
        cmd.CommandText = $@"
        USE {DbName};
        DELETE FROM {TableName};";
        cmd.ExecuteNonQuery();
    }

    private static SqlConnection CreateConnection()
    {
        var sqlConnection = new SqlConnection(
            $"Server=localhost,1433;Database=master;User Id=sa;Password={SqlCredentials.Password};TrustServerCertificate=True;");
        sqlConnection.Open();
        return sqlConnection;
    }
}
