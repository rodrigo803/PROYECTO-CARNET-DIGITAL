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
    [Route("pantallas")]
    public class PantallasController : ControllerBase
    {
        private readonly AuthorizationDb _db;
        private readonly HttpClient _http;
        private readonly AuditClient _audit;

        public PantallasController(AuthorizationDb db, IHttpClientFactory factory, AuditClient audit)
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

        // ✅ Validación nombre y descripción
        private bool TextoValido(string texto)
        {
            return Regex.IsMatch(texto, @"^[a-zA-Z0-9\s]+$");
        }

        // ==========================
        // POST
        // ==========================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePantallaRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized();

            if (request.Id <= 0 ||
                string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Descripcion) ||
                string.IsNullOrWhiteSpace(request.Ruta))
            {
                return BadRequest("Datos inválidos");
            }

            if (!TextoValido(request.Nombre) || !TextoValido(request.Descripcion))
                return BadRequest("Nombre y descripción solo permiten letras, números y espacios");

            using var conn = _db.CreateConnection();

            var existe = await conn.QueryFirstOrDefaultAsync(
                "SELECT 1 FROM Pantalla WHERE Id = @id",
                new { id = request.Id });

            if (existe != null)
                return BadRequest("ID ya existe");

            await conn.ExecuteAsync(@"
                INSERT INTO Pantalla (Id, Nombre, Descripcion, Ruta)
                VALUES (@Id, @Nombre, @Descripcion, @Ruta)", request);

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se creó pantalla {request.Nombre}");

            return Ok("Creada");
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

            var data = await conn.QueryAsync("SELECT * FROM Pantalla");

            return Ok(data);
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

            var data = await conn.QueryFirstOrDefaultAsync(
                "SELECT * FROM Pantalla WHERE Id = @id",
                new { id });

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // ==========================
        // PUT
        // ==========================
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePantallaRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized();

            if (!TextoValido(request.Nombre) || !TextoValido(request.Descripcion))
                return BadRequest("Texto inválido");

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(@"
                UPDATE Pantalla
                SET Nombre=@Nombre,
                    Descripcion=@Descripcion,
                    Ruta=@Ruta
                WHERE Id=@id",
                new { id, request.Nombre, request.Descripcion, request.Ruta });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se actualizó pantalla {id}");

            return Ok("Actualizada");
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
                "DELETE FROM Pantalla WHERE Id = @id",
                new { id });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se eliminó pantalla {id}");

            return Ok("Eliminada");
        }
    }
}