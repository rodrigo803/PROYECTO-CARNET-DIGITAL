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
        // GET /bitacora
        // ==========================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()  
        {
            using var conn = _db.CreateConnection();

            var data = await conn.QueryAsync("SELECT * FROM Bitacora ORDER BY Fecha DESC");

            return Ok(data);
        }
    }
}