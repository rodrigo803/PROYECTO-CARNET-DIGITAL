using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.Data;
using OrganizationService.Models;
using OrganizationService.Services;

namespace OrganizationService.Controllers
{
    [ApiController]
    [Route("areas")]
    public class AreasController : ControllerBase
    {
        private readonly OrganizationDb _db;
        private readonly HttpClient _http;
        private readonly AuditClient _audit;

        public AreasController(OrganizationDb db, IHttpClientFactory factory, AuditClient audit)
        {
            _db = db;
            _http = factory.CreateClient();
            _audit = audit;
        }

        private async Task<bool> ValidarToken()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader))
                return false;

            var requestMsg = new HttpRequestMessage(
                HttpMethod.Get,
                "https://localhost:7181/validate"
            );

            requestMsg.Headers.Add("Authorization", authHeader);

            var response = await _http.SendAsync(requestMsg);

            return response.IsSuccessStatusCode;
        }

        // ==========================
        // POST /areas
        // ==========================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAreaRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            if (request.Id <= 0 ||
                string.IsNullOrWhiteSpace(request.Nombre) ||
                request.InstitucionId <= 0)
            {
                return BadRequest(new { mensaje = "Todos los campos son requeridos" });
            }

            using var conn = _db.CreateConnection();

            // ✅ validar duplicado
            var existe = await conn.QueryFirstOrDefaultAsync(
                "SELECT 1 FROM Area WHERE Id = @id",
                new { id = request.Id });

            if (existe != null)
                return BadRequest(new { mensaje = "Identificador ya existe" });

            await conn.ExecuteAsync(@"
                INSERT INTO Area (Id, Nombre, InstitucionId)
                VALUES (@Id, @Nombre, @InstitucionId)", request);

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, "Se creó un área");

            return Ok(new { mensaje = "Área creada" });
        }

        // ==========================
        // GET /areas
        // ==========================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            var data = await conn.QueryAsync("SELECT * FROM Area");

            return Ok(data);
        }

        // ==========================
        // GET /areas/{id}
        // ==========================
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            var data = await conn.QueryFirstOrDefaultAsync(
                "SELECT * FROM Area WHERE Id = @id",
                new { id });

            if (data == null)
                return NotFound(new { mensaje = "Área no encontrada" });

            return Ok(data);
        }

        // ==========================
        // PUT /areas/{id}
        // ==========================
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateAreaRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            if (request.Id <= 0 ||
                string.IsNullOrWhiteSpace(request.Nombre) ||
                request.InstitucionId <= 0)
            {
                return BadRequest(new { mensaje = "Todos los campos son requeridos" });
            }

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(@"
                UPDATE Area
                SET Nombre = @Nombre,
                    InstitucionId = @InstitucionId
                WHERE Id = @id",
                new { id, request.Nombre, request.InstitucionId });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se actualizó área {id}");

            return Ok(new { mensaje = "Área actualizada" });
        }

        // ==========================
        // DELETE /areas/{id}
        // ==========================
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(
                "DELETE FROM Area WHERE Id = @id",
                new { id });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se eliminó área {id}");

            return Ok(new { mensaje = "Área eliminada" });
        }
    }
}