using AuditService.Data;
using AuditService.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuditService.Controllers
{
    [ApiController]
    [Route("bitacora")]
    public class BitacoraController : ControllerBase
    {
        private readonly AuditDb _db;

        public BitacoraController(AuditDb db)
        {
            _db = db;
        }

        // ==========================
        // POST /bitacora
        // ==========================
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBitacoraRequest request)
        {
            // Cambié <= 0 por < 0 para permitir que el Sistema guarde logs con ID 0
            if (request.UsuarioId < 0 || string.IsNullOrWhiteSpace(request.Descripcion))
            {
                return BadRequest(new { mensaje = "Datos inválidos" });
            }

            using var conn = _db.CreateConnection();

            // Usamos directamente el UsuarioId que nos manda el otro microservicio
            await conn.ExecuteAsync(@"
                INSERT INTO Bitacora (UsuarioId, Descripcion)
                VALUES (@UserId, @Descripcion)",
                new
                {
                    UserId = request.UsuarioId,
                    Descripcion = request.Descripcion
                });

            return Ok(new { mensaje = "Registro guardado en bitácora" });
        }

        // ==========================
        // GET /bitacora (Web15: filtros + paginación)
        // ==========================
        // La HU pide filtrar por "usuario" y "acción", pero la tabla Bitacora solo tiene
        // UsuarioId (int) y Descripcion (texto libre) - no hay un campo de usuario (nombre)
        // ni de acción categorizada. Por eso "usuario" se filtra por UsuarioId exacto y
        // "acción" se filtra con LIKE sobre Descripcion.
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get(
            DateTime? fecha,
            int? usuarioId,
            string? descripcion,
            int page = 1,
            int pageSize = 15)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 15;

            using var conn = _db.CreateConnection();

            var where = new List<string>();
            var parameters = new DynamicParameters();

            if (fecha.HasValue)
            {
                where.Add("CAST(Fecha AS DATE) = @Fecha");
                parameters.Add("Fecha", fecha.Value.Date);
            }

            if (usuarioId.HasValue)
            {
                where.Add("UsuarioId = @UsuarioId");
                parameters.Add("UsuarioId", usuarioId.Value);
            }

            if (!string.IsNullOrWhiteSpace(descripcion))
            {
                where.Add("Descripcion LIKE @Descripcion");
                parameters.Add("Descripcion", $"%{descripcion}%");
            }

            var whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : string.Empty;

            var total = await conn.ExecuteScalarAsync<int>(
                $"SELECT COUNT(*) FROM Bitacora {whereClause}",
                parameters);

            parameters.Add("Offset", (page - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var items = await conn.QueryAsync<BitacoraDto>($@"
                SELECT * FROM Bitacora
                {whereClause}
                ORDER BY Fecha DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
                parameters);

            return Ok(new PagedBitacoraResult
            {
                Items = items.ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            });
        }
    }
}