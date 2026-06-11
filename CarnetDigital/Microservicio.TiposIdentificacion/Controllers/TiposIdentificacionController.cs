using Microservicio.TiposIdentificacion.Data;
using Microservicio.TiposIdentificacion.DTOs;
using Microservicio.TiposIdentificacion.Entities;
using Microservicio.TiposIdentificacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.TiposIdentificacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TiposIdentificacionController : ControllerBase
    {
        private readonly TiposIdentificacionDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TiposIdentificacionController(
            TiposIdentificacionDbContext context,
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
        public async Task<ActionResult<IEnumerable<TipoIdentificacionDto>>> GetTiposIdentificacion()
        {
            var tipos = await _context.TiposIdentificacion
                .Where(t => t.Activo)
                .Select(t => new TipoIdentificacionDto { Id = t.Id, Nombre = t.Nombre })
                .ToListAsync();
            return Ok(tipos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TipoIdentificacionDto>> GetTipoIdentificacion(int id)
        {
            var tipo = await _context.TiposIdentificacion
                .Where(t => t.Id == id && t.Activo)
                .Select(t => new TipoIdentificacionDto { Id = t.Id, Nombre = t.Nombre })
                .FirstOrDefaultAsync();

            if (tipo == null)
                return NotFound(new { mensaje = $"No se encontró un tipo de identificación con ID {id}" });

            return Ok(tipo);
        }

        [HttpPost]
        public async Task<ActionResult<TipoIdentificacionDto>> CrearTipoIdentificacion([FromBody] CrearTipoIdentificacionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            var tipo = new TipoIdentificacion { Nombre = dto.Nombre.Trim(), Activo = true };
            _context.TiposIdentificacion.Add(tipo);
            await _context.SaveChangesAsync();

            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Creó el tipo de identificación '{tipo.Nombre}' (ID: {tipo.Id})",
                    token);
            }

            return CreatedAtAction(nameof(GetTipoIdentificacion), new { id = tipo.Id },
                new TipoIdentificacionDto { Id = tipo.Id, Nombre = tipo.Nombre });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TipoIdentificacionDto>> ActualizarTipoIdentificacion(int id, [FromBody] ActualizarTipoIdentificacionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            var tipo = await _context.TiposIdentificacion.FirstOrDefaultAsync(t => t.Id == id && t.Activo);
            if (tipo == null)
                return NotFound(new { mensaje = $"No se encontró un tipo de identificación con ID {id}" });

            tipo.Nombre = dto.Nombre.Trim();
            await _context.SaveChangesAsync();

            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Modificó el tipo de identificación '{tipo.Nombre}' (ID: {tipo.Id})",
                    token);
            }

            return Ok(new TipoIdentificacionDto { Id = tipo.Id, Nombre = tipo.Nombre });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTipoIdentificacion(int id)
        {
            var tipo = await _context.TiposIdentificacion.FirstOrDefaultAsync(t => t.Id == id && t.Activo);
            if (tipo == null)
                return NotFound(new { mensaje = $"No se encontró un tipo de identificación con ID {id}" });

            tipo.Activo = false;
            await _context.SaveChangesAsync();

            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Eliminó el tipo de identificación '{tipo.Nombre}' (ID: {tipo.Id})",
                    token);
            }

            return NoContent();
        }
    }
}