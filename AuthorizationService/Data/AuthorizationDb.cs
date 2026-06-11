using System.Data;
using Microsoft.Data.SqlClient;

namespace AuthorizationService.Data
{
    public class AuthorizationDb
    {
        private readonly string _connectionString;

        public AuthorizationDb(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("AuthorizationDb")!;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}