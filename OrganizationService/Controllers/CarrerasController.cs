using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationService.Data;
using OrganizationService.Models;
using OrganizationService.Services;
using System.Text.RegularExpressions;

namespace OrganizationService.Controllers
{
    [ApiController]
    [Route("carreras")]
    public class CarrerasController : ControllerBase
    {
        private readonly OrganizationDb _db;
        private readonly HttpClient _http;
        private readonly AuditClient _audit;

        public CarrerasController(OrganizationDb db, IHttpClientFactory factory, AuditClient audit)
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCarreraRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            
            if (request.Id <= 0)
                return BadRequest(new { mensaje = "Identificador requerido" });


            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Director) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Telefono) ||
                request.InstitucionId <= 0)
            {
                return BadRequest(new { mensaje = "Todos los campos son requeridos" });
            }

            if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest(new { mensaje = "Email inválido" });

            if (!Regex.IsMatch(request.Telefono, @"^\d+$"))
                return BadRequest(new { mensaje = "Teléfono debe ser numérico" });

            using var conn = _db.CreateConnection();

            var existe = await conn.QueryFirstOrDefaultAsync(
            "SELECT 1 FROM Carrera WHERE Id = @id",
            new { id = request.Id });

            if (existe != null)
                return BadRequest(new { mensaje = "Identificador ya existe" });

            await conn.ExecuteAsync(@"               
                INSERT INTO Carrera (Id, Nombre, Director, Email, Telefono, InstitucionId)
                VALUES (@Id, @Nombre, @Director, @Email, @Telefono, @InstitucionId)
                ", request);

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, "Se creó una carrera");

            return Ok(new { mensaje = "Carrera creada" });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCarreraRequest request)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync(@"
                UPDATE Carrera
                SET Nombre=@Nombre, Director=@Director, Email=@Email,
                    Telefono=@Telefono, InstitucionId=@InstitucionId
                WHERE Id=@id",
                new { id, request.Nombre, request.Director, request.Email, request.Telefono, request.InstitucionId });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se actualizó carrera {id}");

            return Ok(new { mensaje = "Actualizada" });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            await conn.ExecuteAsync("DELETE FROM Carrera WHERE Id=@id", new { id });

            var userId = int.Parse(User.FindFirst("uid")!.Value);
            await _audit.Registrar(Request, userId, $"Se eliminó carrera {id}");

            return Ok(new { mensaje = "Eliminada" });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            var data = await conn.QueryAsync("SELECT * FROM Carrera");

            return Ok(data);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!await ValidarToken())
                return Unauthorized(new { mensaje = "Token inválido" });

            using var conn = _db.CreateConnection();

            var data = await conn.QueryFirstOrDefaultAsync(
                "SELECT * FROM Carrera WHERE Id = @id",
                new { id });

            if (data == null)
                return NotFound(new { mensaje = "Carrera no encontrada" });

            return Ok(data);
        }
    }
}
