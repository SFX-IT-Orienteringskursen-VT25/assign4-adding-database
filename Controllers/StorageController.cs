using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace AdditionApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly SqlDatabase _database;

        public StorageController(SqlDatabase database)
        {
            _database = database;
        }

        [HttpPost("SaveCalculation")]
        public IActionResult SaveCalculation([FromBody] Calculation sqlCalculation)
        {
            try
            {
                using var conn = new SqlConnection(_database.GetConnectionString());
                conn.Open();

                const string query = @"
INSERT INTO CalculationStorage (QT, VOL, TMP, Result)
VALUES (@QT, @VOL, @TMP, @Result);";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@QT", sqlCalculation.QT);
                cmd.Parameters.AddWithValue("@VOL", sqlCalculation.VOL);
                cmd.Parameters.AddWithValue("@TMP", sqlCalculation.TMP);
                cmd.Parameters.AddWithValue("@Result", sqlCalculation.Result);

                cmd.ExecuteNonQuery();
                return Ok("Calculation saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving calculation: {ex.Message}");
            }
        }

        [HttpGet("GetCalculations")]
        public IActionResult GetCalculations()
        {
            try
            {
                using var conn = new SqlConnection(_database.GetConnectionString());
                conn.Open();

                const string query = @"
SELECT TOP 10 Id, QT, VOL, TMP, Result
FROM CalculationStorage
ORDER BY Id DESC;";

                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                var items = new List<object>();
                while (reader.Read())
                {
                    items.Add(new
                    {
                        Id = reader.GetInt32(0),
                        Operand1 = reader.GetDouble(1),
                        Operand2 = reader.GetDouble(2),
                        Operation = reader.GetString(3),
                        Result = reader.GetDouble(4)
                    });
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving calculations: {ex.Message}");
            }
        }
    }
}
