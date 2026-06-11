using System.Data;
using Microsoft.Data.SqlClient;

namespace OrganizationService.Data
{
    public class OrganizationDb
    {
        private readonly string _connectionString;

        public OrganizationDb(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("OrganizationDb")!;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
