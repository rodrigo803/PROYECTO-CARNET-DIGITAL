using System.Data;
using Microsoft.Data.SqlClient;

namespace UserService.Data
{
    public class OrganizationDb
    {
        private readonly string _connectionString;

        public UserDb(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("UserDb")!;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
