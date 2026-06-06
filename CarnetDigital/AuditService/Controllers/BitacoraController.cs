using AuditService.Data;
using AuditService.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBitacoraRequest request)
        {
            if (request.UsuarioId <= 0 ||
                string.IsNullOrWhiteSpace(request.Descripcion))
            {
                return BadRequest(new { mensaje = "Datos inválidos" });
            }

            //Usuario desde token
            var userIdClaim = User.FindFirst("uid");

            if (userIdClaim == null)
                return Unauthorized();

            int userIdFromToken = int.Parse(userIdClaim.Value);

            //Validar coincidencia
            if (userIdFromToken != request.UsuarioId)
            {
                return Unauthorized(new { mensaje = "Usuario no coincide con token" });
            }

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(@"
                INSERT INTO Bitacora (UsuarioId, Descripcion)
                VALUES (@UserId, @Descripcion)",
                new
                {
                    UserId = userIdFromToken,
                    Descripcion = request.Descripcion
                });

            return Ok(new { mensaje = "Registro guardado" });
        }

        // ==========================
        // GET /bitacora
        // ==========================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var conn = _db.CreateConnection();

            var data = await conn.QueryAsync("SELECT * FROM Bitacora ORDER BY Fecha DESC");

            return Ok(data);
        }
    }
}