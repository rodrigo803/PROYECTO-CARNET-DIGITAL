using System.Data;
using Microsoft.Data.SqlClient;

namespace CatalogService.Data
{
    public class CatalogDb
    {
        private readonly string _connectionString;

        public CatalogDb(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("CatalogDb")!;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}