using Dapper;
using ParameterService.Entities;

namespace ParameterService.Repository
{
    public class ParametroRepository
    {
        private readonly IDbConnectionFactory _db;

        public ParametroRepository(IDbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Parametro>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Parametro>("SELECT Id, Valor FROM Parametro");
        }

        public async Task<Parametro?> GetByIdAsync(string id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Parametro>(
                "SELECT Id, Valor FROM Parametro WHERE Id = @id",
                new { id });
        }

        public async Task<int> CreateAsync(Parametro p)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteAsync(
                "INSERT INTO Parametro (Id, Valor) VALUES (@Id, @Valor)", p);
        }

        public async Task<int> UpdateAsync(Parametro p)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteAsync(
                "UPDATE Parametro SET Valor = @Valor WHERE Id = @Id", p);
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteAsync(
                "DELETE FROM Parametro WHERE Id = @id", new { id });
        }
    }
}