using Microservicio.Areas.Data;
using Microservicio.Areas.DTOs;
using Microservicio.Areas.Entities;
using Microservicio.Areas.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Areas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AreasController : ControllerBase
    {
        private readonly AreasDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IInstitucionesValidator _institucionesValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AreasController(
            AreasDbContext context,
            IBitacoraService bitacoraService,
            IInstitucionesValidator institucionesValidator,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _bitacoraService = bitacoraService;
            _institucionesValidator = institucionesValidator;
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

        // GET: api/Areas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaDto>>> GetAreas()
        {
            var token = GetToken();
            if (token == null)
                return Unauthorized();

            var areas = await _context.AreasTrabajo
                .Where(a => a.Activo)
                .ToListAsync();

            var resultado = new List<AreaDto>();

            foreach (var area in areas)
            {
                var institucion = await _institucionesValidator.ObtenerInstitucionAsync(area.IdInstitucion, token);

                resultado.Add(new AreaDto
                {
                    Id = area.Id,
                    Nombre = area.Nombre,
                    IdInstitucion = area.IdInstitucion,
                    NombreInstitucion = institucion?.Nombre ?? "(Institución no disponible)"
                });
            }

            return Ok(resultado);
        }

        // GET: api/Areas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AreaDto>> GetArea(int id)
        {
            var token = GetToken();
            if (token == null)
                return Unauthorized();

            var area = await _context.AreasTrabajo
                .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

            if (area == null)
                return NotFound(new { mensaje = $"No se encontró un área de trabajo con ID {id}" });

            var institucion = await _institucionesValidator.ObtenerInstitucionAsync(area.IdInstitucion, token);

            return Ok(new AreaDto
            {
                Id = area.Id,
                Nombre = area.Nombre,
                IdInstitucion = area.IdInstitucion,
                NombreInstitucion = institucion?.Nombre ?? "(Institución no disponible)"
            });
        }

        // POST: api/Areas
        [HttpPost]
        public async Task<ActionResult<AreaDto>> CrearArea([FromBody] CrearAreaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            var token = GetToken();
            if (token == null)
                return Unauthorized();

            var institucion = await _institucionesValidator.ObtenerInstitucionAsync(dto.IdInstitucion, token);
            if (institucion == null)
                return BadRequest(new { mensaje = $"No existe una institución activa con ID {dto.IdInstitucion}" });

            var area = new AreaTrabajo
            {
                Nombre = dto.Nombre.Trim(),
                IdInstitucion = dto.IdInstitucion,
                Activo = true
            };

            _context.AreasTrabajo.Add(area);
            await _context.SaveChangesAsync();

            // Bitácora
            var usuarioId = GetUsuarioId();
            if (usuarioId.HasValue)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Creó el área de trabajo '{area.Nombre}' (ID: {area.Id})",
                    token);
            }

            return CreatedAtAction(nameof(GetArea), new { id = area.Id }, new AreaDto
            {
                Id = area.Id,
                Nombre = area.Nombre,
                IdInstitucion = area.IdInstitucion,
                NombreInstitucion = institucion.Nombre
            });
        }

        // PUT: api/Areas/5
        [HttpPut("{id}")]
        public async Task<ActionResult<AreaDto>> ActualizarArea(int id, [FromBody] ActualizarAreaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            var token = GetToken();
            if (token == null)
                return Unauthorized();

            var area = await _context.AreasTrabajo
                .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

            if (area == null)
                return NotFound(new { mensaje = $"No se encontró un área de trabajo con ID {id}" });

            var institucion = await _institucionesValidator.ObtenerInstitucionAsync(dto.IdInstitucion, token);
            if (institucion == null)
                return BadRequest(new { mensaje = $"No existe una institución activa con ID {dto.IdInstitucion}" });

            area.Nombre = dto.Nombre.Trim();
            area.IdInstitucion = dto.IdInstitucion;

            await _context.SaveChangesAsync();

            // Bitácora
            var usuarioId = GetUsuarioId();
            if (usuarioId.HasValue)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Modificó el área de trabajo '{area.Nombre}' (ID: {area.Id})",
                    token);
            }

            return Ok(new AreaDto
            {
                Id = area.Id,
                Nombre = area.Nombre,
                IdInstitucion = area.IdInstitucion,
                NombreInstitucion = institucion.Nombre
            });
        }

        // DELETE: api/Areas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarArea(int id)
        {
            var area = await _context.AreasTrabajo
                .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

            if (area == null)
                return NotFound(new { mensaje = $"No se encontró un área de trabajo con ID {id}" });

            area.Activo = false;
            await _context.SaveChangesAsync();

            // Bitácora
            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Eliminó el área de trabajo '{area.Nombre}' (ID: {area.Id})",
                    token);
            }

            return NoContent();
        }
    }
}