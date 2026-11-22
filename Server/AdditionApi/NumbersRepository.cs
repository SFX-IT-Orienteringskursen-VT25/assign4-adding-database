using Microsoft.Data.SqlClient;

public class NumbersRepository
{
    private readonly string _connectionString;

    public NumbersRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int[]> GetNumbersAsync()
    {
        var numbers = new List<int>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "SELECT Value FROM Numbers ORDER BY Id";

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            numbers.Add(reader.GetInt32(0));
        }

        return numbers.ToArray();
    }
    public async Task SaveNumbersAsync(int[] numbers)
    {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var transaction =
                (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                const string deleteSql = "DELETE FROM Numbers";

                await using (var deleteCommand = new SqlCommand(deleteSql, connection, transaction))
                {
                    await deleteCommand.ExecuteNonQueryAsync();
                }

                const string insertSql = "INSERT INTO Numbers (Value) VALUES (@value)";

                foreach (int number in numbers)
                {
                    await using var insertCommand =
                        new SqlCommand(insertSql, connection, transaction);

                    insertCommand.Parameters.AddWithValue("@value", number);

                    await insertCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
    }
}