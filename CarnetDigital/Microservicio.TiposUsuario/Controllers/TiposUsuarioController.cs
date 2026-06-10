using Microservicio.TiposUsuario.Data;
using Microservicio.TiposUsuario.DTOs;
using Microservicio.TiposUsuario.Entities;
using Microservicio.TiposUsuario.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.TiposUsuario.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TiposUsuarioController : ControllerBase
    {
        private readonly TiposUsuarioDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TiposUsuarioController(
            TiposUsuarioDbContext context,
            IBitacoraService bitacoraService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _bitacoraService = bitacoraService;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetUsuarioId()
        {
            var uidClaim = User.FindFirst("uid");
            if (uidClaim == null) return null;
            if (int.TryParse(uidClaim.Value, out int uid)) return uid;
            return null;
        }

        private string? GetToken()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ")) return null;
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoUsuarioDto>>> GetTiposUsuario()
        {
            var tipos = await _context.TiposUsuario
                .Where(t => t.Activo)
                .Select(t => new TipoUsuarioDto { Id = t.Id, Nombre = t.Nombre })
                .ToListAsync();
            return Ok(tipos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TipoUsuarioDto>> GetTipoUsuario(int id)
        {
            var tipo = await _context.TiposUsuario
                .Where(t => t.Id == id && t.Activo)
                .Select(t => new TipoUsuarioDto { Id = t.Id, Nombre = t.Nombre })
                .FirstOrDefaultAsync();

            if (tipo == null)
                return NotFound(new { mensaje = $"No se encontró un tipo de usuario con ID {id}" });

            return Ok(tipo);
        }

        [HttpPost]
        public async Task<ActionResult<TipoUsuarioDto>> CrearTipoUsuario([FromBody] CrearTipoUsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            var tipo = new TipoUsuario { Nombre = dto.Nombre.Trim(), Activo = true };
            _context.TiposUsuario.Add(tipo);
            await _context.SaveChangesAsync();

            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Creó el tipo de usuario '{tipo.Nombre}' (ID: {tipo.Id})",
                    token);
            }

            return CreatedAtAction(nameof(GetTipoUsuario), new { id = tipo.Id },
                new TipoUsuarioDto { Id = tipo.Id, Nombre = tipo.Nombre });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TipoUsuarioDto>> ActualizarTipoUsuario(int id, [FromBody] ActualizarTipoUsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            var tipo = await _context.TiposUsuario.FirstOrDefaultAsync(t => t.Id == id && t.Activo);
            if (tipo == null)
                return NotFound(new { mensaje = $"No se encontró un tipo de usuario con ID {id}" });

            tipo.Nombre = dto.Nombre.Trim();
            await _context.SaveChangesAsync();

            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Modificó el tipo de usuario '{tipo.Nombre}' (ID: {tipo.Id})",
                    token);
            }

            return Ok(new TipoUsuarioDto { Id = tipo.Id, Nombre = tipo.Nombre });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTipoUsuario(int id)
        {
            var tipo = await _context.TiposUsuario.FirstOrDefaultAsync(t => t.Id == id && t.Activo);
            if (tipo == null)
                return NotFound(new { mensaje = $"No se encontró un tipo de usuario con ID {id}" });

            tipo.Activo = false;
            await _context.SaveChangesAsync();

            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Eliminó el tipo de usuario '{tipo.Nombre}' (ID: {tipo.Id})",
                    token);
            }

            return NoContent();
        }
    }
}