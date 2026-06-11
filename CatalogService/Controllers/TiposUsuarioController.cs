using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CatalogService.Data;
using CatalogService.Models;
using CatalogService.Services;

namespace CatalogService.Controllers
{
    [ApiController]
    [Route("tiposusuario")]
    public class TiposUsuarioController : ControllerBase
    {
        private readonly CatalogDb _db;
        private readonly HttpClient _http;
        private readonly AuditClient _audit;

        public TiposUsuarioController(CatalogDb db, IHttpClientFactory factory, AuditClient audit)
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
        // POST
        // ==========================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTipoUsuarioRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized();

            if (request.Id <= 0 || string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest("Datos inválidos");

            using var conn = _db.CreateConnection();

            var existe = await conn.QueryFirstOrDefaultAsync(
                "SELECT 1 FROM TipoUsuario WHERE Id = @id",
                new { id = request.Id });

            if (existe != null)
                return BadRequest("ID ya existe");

            await conn.ExecuteAsync(
                "INSERT INTO TipoUsuario (Id, Nombre) VALUES (@Id, @Nombre)",
                request);

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, "Se creó un tipo de usuario");

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

            var data = await conn.QueryAsync("SELECT * FROM TipoUsuario");

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
                "SELECT * FROM TipoUsuario WHERE Id = @id",
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
        public async Task<IActionResult> Update(int id, [FromBody] CreateTipoUsuarioRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Nombre))
                return BadRequest("Nombre requerido");

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(
                "UPDATE TipoUsuario SET Nombre=@Nombre WHERE Id=@id",
                new { id, request.Nombre });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se actualizó tipo de usuario {id}");

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
                "DELETE FROM TipoUsuario WHERE Id = @id",
                new { id });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se eliminó tipo de usuario {id}");

            return Ok("Eliminado");
        }
    }
}