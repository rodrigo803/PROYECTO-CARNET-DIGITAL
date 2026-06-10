using System.Data;
using Microsoft.Data.SqlClient;

namespace AuditService.Data
{
    public class AuditDb
    {
        private readonly string _connectionString;

        public AuditDb(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("AuditDb")!;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
