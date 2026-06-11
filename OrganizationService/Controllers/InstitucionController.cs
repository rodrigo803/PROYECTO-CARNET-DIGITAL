using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.Data;
using OrganizationService.Models;
using System.Text.RegularExpressions;
using OrganizationService.Services;

namespace OrganizationService.Controllers
{
    [ApiController]
    [Route("institucion")]
    public class InstitucionController : ControllerBase
    {
        private readonly OrganizationDb _db;
        private readonly HttpClient _http;
        private readonly AuditClient _audit;

        public InstitucionController(OrganizationDb db, IHttpClientFactory factory, AuditClient audit)
        {
            _db = db;
            _http = factory.CreateClient();
            _audit = audit;
        }

        // Método reutilizable para validar token con AuthService
        private async Task<bool> ValidarToken()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader))
                return false;

            var requestMsg = new HttpRequestMessage(
                HttpMethod.Get,
                "https://localhost:7014/validate"
            );

            requestMsg.Headers.Add("Authorization", authHeader);

            var response = await _http.SendAsync(requestMsg);

            return response.IsSuccessStatusCode;
        }

        // ==========================
        // POST /institucion
        // ==========================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInstitucionRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Telefono) ||
                string.IsNullOrWhiteSpace(request.Dominios))
            {
                return BadRequest(new { mensaje = "Todos los campos son requeridos" });
            }

            if (request.Id <= 0)
                return BadRequest(new { mensaje = "Identificador requerido" });

            if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest(new { mensaje = "Email inválido" });

            if (!Regex.IsMatch(request.Telefono, @"^\d+$"))
                return BadRequest(new { mensaje = "Teléfono debe ser numérico" });

            using var conn = _db.CreateConnection();
            
            var existe = await conn.QueryFirstOrDefaultAsync(
                "SELECT 1 FROM Institucion WHERE Id = @id",
                new { id = request.Id });

            if (existe != null)
                return BadRequest(new { mensaje = "Identificador ya existe" });


            await conn.ExecuteAsync(@"           
                INSERT INTO Institucion (Id, Nombre, Email, Telefono, Dominios)
                VALUES (@Id, @Nombre, @Email, @Telefono, @Dominios)
                ", request);

            
            var userId = int.Parse(User.FindFirst("uid")!.Value);

            await _audit.Registrar(Request, userId, "Se creó una institución");

            return Ok(new { mensaje = "Institución creada" });
        }

        // ==========================
        // GET /institucion
        // ==========================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            var data = await conn.QueryAsync("SELECT * FROM Institucion");

            var userId = int.Parse(User.FindFirst("uid")!.Value);

            await _audit.Registrar(Request, userId, "Se consultaron todas las instituciones");

            return Ok(data);
        }

        // ==========================
        // GET /institucion/{id}
        // ==========================
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            var data = await conn.QueryFirstOrDefaultAsync(
                "SELECT * FROM Institucion WHERE Id = @id",
                new { id });

            if (data == null)
                return NotFound(new { mensaje = "Institución no encontrada" });

            var userId = int.Parse(User.FindFirst("uid")!.Value);

            await _audit.Registrar(Request, userId, "Se consultó una institución");

            return Ok(data);
        }

        // ==========================
        // PUT /institucion/{id}
        // ==========================
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateInstitucionRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Telefono) ||
                string.IsNullOrWhiteSpace(request.Dominios))
            {
                return BadRequest(new { mensaje = "Todos los campos son requeridos" });
            }

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(@"
                UPDATE Institucion
                SET Nombre = @Nombre,
                    Email = @Email,
                    Telefono = @Telefono,
                    Dominios = @Dominios
                WHERE Id = @id",
                new { id, request.Nombre, request.Email, request.Telefono, request.Dominios });

            var userId = int.Parse(User.FindFirst("uid")!.Value);

            await _audit.Registrar(Request, userId, "Se actualizó una institución");

            return Ok(new { mensaje = "Institución actualizada" });
        }

        // ==========================
        // DELETE /institucion/{id}
        // ==========================
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(
                "DELETE FROM Institucion WHERE Id = @id",
                new { id });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, "Se eliminó una institución");
            return Ok(new { mensaje = "Institución eliminada" });
        }
    }
}