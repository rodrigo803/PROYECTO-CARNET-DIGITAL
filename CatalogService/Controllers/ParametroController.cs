using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CatalogService.Data;
using CatalogService.Models;
using CatalogService.Services;
using System.Text.RegularExpressions;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("parametro")]
    public class ParametroController : ControllerBase
    {
        private readonly CatalogDb _db;
        private readonly HttpClient _http;
        private readonly AuditClient _audit;

        public ParametroController(CatalogDb db, IHttpClientFactory factory, AuditClient audit)
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

        // ✅ VALIDACIÓN CENTRALIZADA
        private bool EsIdValido(string id)
        {
            return Regex.IsMatch(id, @"^[A-Z]{1,10}$");
        }

        // ==========================
        // POST
        // ==========================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateParametroRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Id) ||
                string.IsNullOrWhiteSpace(request.Valor))
                return BadRequest("Campos requeridos");

            if (!EsIdValido(request.Id))
                return BadRequest("Id inválido (solo mayúsculas, máximo 10 caracteres)");

            if (request.Valor.Length > 500)
                return BadRequest("Valor excede 500 caracteres");

            using var conn = _db.CreateConnection();

            var existe = await conn.QueryFirstOrDefaultAsync(
                "SELECT 1 FROM Parametro WHERE Id = @id",
                new { id = request.Id });

            if (existe != null)
                return BadRequest("Identificador ya existe");

            await conn.ExecuteAsync(
                "INSERT INTO Parametro (Id, Valor) VALUES (@Id, @Valor)",
                request);

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se creó parámetro {request.Id}");

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

            var data = await conn.QueryAsync("SELECT * FROM Parametro");

            return Ok(data);
        }

        // ==========================
        // GET BY ID
        // ==========================
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (!await ValidarToken())
                return Unauthorized();

            using var conn = _db.CreateConnection();

            var data = await conn.QueryFirstOrDefaultAsync(
                "SELECT * FROM Parametro WHERE Id = @id",
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
        public async Task<IActionResult> Update(string id, [FromBody] CreateParametroRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Valor))
                return BadRequest("Valor requerido");

            if (request.Valor.Length > 500)
                return BadRequest("Valor excede 500 caracteres");

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(
                "UPDATE Parametro SET Valor=@Valor WHERE Id=@id",
                new { id, request.Valor });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se actualizó parámetro {id}");

            return Ok("Actualizado");
        }

        // ==========================
        // DELETE
        // ==========================
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await ValidarToken())
                return Unauthorized();

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(
                "DELETE FROM Parametro WHERE Id = @id",
                new { id });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se eliminó parámetro {id}");

            return Ok("Eliminado");
        }
    }
}