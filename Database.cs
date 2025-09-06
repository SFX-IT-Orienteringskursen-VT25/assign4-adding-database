using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace AdditionApi
{
    public class SqlDatabase
    {
        private readonly string _connectionString;

        public SqlDatabase(IConfiguration configuration, SqlCredential sqlCredential)
        {
            var baseConn = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

            var builder = new SqlConnectionStringBuilder(baseConn);

            if (!string.IsNullOrWhiteSpace(sqlCredential?.UserId) &&
                !string.IsNullOrWhiteSpace(sqlCredential.Password))
            {
                builder.UserID = sqlCredential.UserId;
                builder.Password = sqlCredential.Password;
                builder.IntegratedSecurity = false;
            }
            else
            {
                builder.IntegratedSecurity = true;
            }

            _connectionString = builder.ToString();
        }

        public string GetConnectionString() => _connectionString;
    }
}
