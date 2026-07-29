using Dapper;
using AccessControlService.Repository;
using AccessControlService.Entities;

namespace AccessControlService.Repository
{
    public class PantallaRepository
    {
        private readonly IDbConnectionFactory _db;

        public PantallaRepository(IDbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Pantalla>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Pantalla>("SELECT * FROM Pantalla");
        }

        public async Task<Pantalla?> GetByIdAsync(string id)
        {
            using var conn = _db.CreateConnection();

            return await conn.QueryFirstOrDefaultAsync<Pantalla>(
                "SELECT * FROM Pantalla WHERE Id = @id", new { id });
        }

        public async Task<int> CreateAsync(Pantalla p)
        {
            using var conn = _db.CreateConnection();

            return await conn.ExecuteAsync(
                @"INSERT INTO Pantalla (Id, Nombre, Descripcion, Ruta)
                  VALUES (@Id, @Nombre, @Descripcion, @Ruta)", p);
        }

        public async Task<int> UpdateAsync(Pantalla p)
        {
            using var conn = _db.CreateConnection();

            return await conn.ExecuteAsync(
                @"UPDATE Pantalla
                  SET Nombre = @Nombre,
                      Descripcion = @Descripcion,
                      Ruta = @Ruta
                  WHERE Id = @Id", p);
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var conn = _db.CreateConnection();

            return await conn.ExecuteAsync(
                "DELETE FROM Pantalla WHERE Id = @id", new { id });
        }
    }
}
