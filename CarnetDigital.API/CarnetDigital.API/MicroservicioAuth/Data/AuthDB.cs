using System.Data;
using Microsoft.Data.SqlClient;

namespace AuthService.Data
{
    public class AuthDb
    {
        private readonly string _connectionString;

        public AuthDb(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("AuthDb")!;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}