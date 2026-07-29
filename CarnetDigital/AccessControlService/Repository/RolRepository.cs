using Dapper;
using AccessControlService.Entities;

namespace AccessControlService.Repository
{
    public class RolRepository
    {
        private readonly IDbConnectionFactory _db;

        public RolRepository(IDbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();

            var roles = await conn.QueryAsync<Rol>(
                "SELECT Id, Nombre FROM Rol");

            foreach (var rol in roles)
            {
                var pantallas = await conn.QueryAsync<string>(
                    @"SELECT PantallaId
                      FROM RolPantalla
                      WHERE RolId = @id",
                    new { id = rol.Id });

                rol.Pantallas = pantallas.ToList();
            }

            return roles;
        }

        public async Task<Rol?> GetByIdAsync(string id)
        {
            using var conn = _db.CreateConnection();

            var rol = await conn.QueryFirstOrDefaultAsync<Rol>(
                @"SELECT Id, Nombre
                  FROM Rol
                  WHERE Id = @id",
                new { id });

            if (rol == null)
                return null;

            var pantallas = await conn.QueryAsync<string>(
                @"SELECT PantallaId
                  FROM RolPantalla
                  WHERE RolId = @id",
                new { id });

            rol.Pantallas = pantallas.ToList();

            return rol;
        }

        public async Task<int> CreateAsync(Rol rol)
        {
            using var conn = _db.CreateConnection();

            conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                await conn.ExecuteAsync(
                    @"INSERT INTO Rol (Id, Nombre)
                      VALUES (@Id, @Nombre)",
                    rol,
                    tx);

                foreach (var p in rol.Pantallas)
                {
                    await conn.ExecuteAsync(
                        @"INSERT INTO RolPantalla
                          (RolId, PantallaId)
                          VALUES (@rol, @pantalla)",
                        new
                        {
                            rol = rol.Id,
                            pantalla = p
                        },
                        tx);
                }

                tx.Commit();

                return 1;
            }
            catch
            {
                tx.Rollback();
                return 0;
            }
        }

        public async Task<int> UpdateAsync(Rol rol)
        {
            using var conn = _db.CreateConnection();

            conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                await conn.ExecuteAsync(
                    @"UPDATE Rol
                      SET Nombre = @Nombre
                      WHERE Id = @Id",
                    rol,
                    tx);

                await conn.ExecuteAsync(
                    @"DELETE FROM RolPantalla
                      WHERE RolId = @id",
                    new { id = rol.Id },
                    tx);

                foreach (var p in rol.Pantallas)
                {
                    await conn.ExecuteAsync(
                        @"INSERT INTO RolPantalla
                          (RolId, PantallaId)
                          VALUES (@rol, @pantalla)",
                        new
                        {
                            rol = rol.Id,
                            pantalla = p
                        },
                        tx);
                }

                tx.Commit();

                return 1;
            }
            catch
            {
                tx.Rollback();
                return 0;
            }
        }

        public async Task<int> DeleteAsync(string id)
        {
            using var conn = _db.CreateConnection();

            conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                await conn.ExecuteAsync(
                    @"DELETE FROM RolPantalla
                      WHERE RolId = @id",
                    new { id },
                    tx);

                var rows = await conn.ExecuteAsync(
                    @"DELETE FROM Rol
                      WHERE Id = @id",
                    new { id },
                    tx);

                tx.Commit();

                return rows;
            }
            catch
            {
                tx.Rollback();
                return 0;
            }
        }
    }
}