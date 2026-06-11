using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorizationService.Data;
using AuthorizationService.Models;
using AuthorizationService.Services;
using System.Text.RegularExpressions;

namespace AuthorizationService.Controllers
{
    [ApiController]
    [Route("rol")]
    public class RolController : ControllerBase
    {
        private readonly AuthorizationDb _db;
        private readonly HttpClient _http;
        private readonly AuditClient _audit;

        public RolController(AuthorizationDb db, IHttpClientFactory factory, AuditClient audit)
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

        private bool NombreValido(string nombre)
        {
            return Regex.IsMatch(nombre, @"^[a-zA-Z0-9\s]+$");
        }

        // ==========================
        // POST
        // ==========================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRolRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized();

            if (request.Id <= 0 ||
                string.IsNullOrWhiteSpace(request.Nombre) ||
                !NombreValido(request.Nombre))
            {
                return BadRequest("Datos inválidos");
            }

            using var conn = _db.CreateConnection();

            var existe = await conn.QueryFirstOrDefaultAsync(
                "SELECT 1 FROM Rol WHERE Id = @id",
                new { id = request.Id });

            if (existe != null)
                return BadRequest("ID ya existe");

            //Insertar rol
            await conn.ExecuteAsync(
                "INSERT INTO Rol (Id, Nombre) VALUES (@Id, @Nombre)",
                request);

            //Insertar relación pantallas
            foreach (var pantallaId in request.Pantallas)
            {
                await conn.ExecuteAsync(
                    "INSERT INTO RolPantalla (RolId, PantallaId) VALUES (@RolId, @PantallaId)",
                    new { RolId = request.Id, PantallaId = pantallaId });
            }

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se creó rol {request.Nombre}");

            return Ok("Creado");
        }

        // ==========================
        // GET ALL
        // ==========================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!await ValidarToken())
                return Unauthorized();

            using var conn = _db.CreateConnection();

            var roles = await conn.QueryAsync("SELECT * FROM Rol");

            return Ok(roles);
        }

        // ==========================
        // GET BY ID
        // ==========================
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!await ValidarToken())
                return Unauthorized();

            using var conn = _db.CreateConnection();

            var rol = await conn.QueryFirstOrDefaultAsync(
                "SELECT * FROM Rol WHERE Id = @id",
                new { id });

            if (rol == null)
                return NotFound();

            var pantallas = await conn.QueryAsync<int>(
                "SELECT PantallaId FROM RolPantalla WHERE RolId = @id",
                new { id });

            return Ok(new
            {
                rol,
                pantallas
            });
        }

        // ==========================
        // PUT
        // ==========================
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateRolRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                !NombreValido(request.Nombre))
            {
                return BadRequest("Nombre inválido");
            }

            using var conn = _db.CreateConnection();

            // ✅ actualizar rol
            await conn.ExecuteAsync(
                "UPDATE Rol SET Nombre=@Nombre WHERE Id=@id",
                new { id, request.Nombre });

            // ✅ eliminar relaciones actuales
            await conn.ExecuteAsync(
                "DELETE FROM RolPantalla WHERE RolId = @id",
                new { id });

            // ✅ insertar nuevas relaciones
            foreach (var pantallaId in request.Pantallas)
            {
                await conn.ExecuteAsync(
                    "INSERT INTO RolPantalla (RolId, PantallaId) VALUES (@RolId, @PantallaId)",
                    new { RolId = id, PantallaId = pantallaId });
            }

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se actualizó rol {id}");

            return Ok("Actualizado");
        }

        // ==========================
        // DELETE
        // ==========================
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await ValidarToken())
                return Unauthorized();

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(
                "DELETE FROM RolPantalla WHERE RolId = @id",
                new { id });

            await conn.ExecuteAsync(
                "DELETE FROM Rol WHERE Id = @id",
                new { id });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se eliminó rol {id}");

            return Ok("Eliminado");
        }
    }
}
