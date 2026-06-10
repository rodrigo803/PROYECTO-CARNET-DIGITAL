using Microservicio.Instituciones.Data;
using Microservicio.Instituciones.DTOs;
using Microservicio.Instituciones.Entities;
using Microservicio.Instituciones.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Instituciones.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InstitucionesController : ControllerBase
    {
        private readonly InstitucionesDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InstitucionesController(
            InstitucionesDbContext context,
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

        // GET: api/Instituciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstitucionDto>>> GetInstituciones()
        {
            var instituciones = await _context.Instituciones
                .Where(i => i.Activo)
                .Include(i => i.Dominios.Where(d => d.Activo))
                .Select(i => new InstitucionDto
                {
                    Id = i.Id,
                    Nombre = i.Nombre,
                    Email = i.Email,
                    Telefono = i.Telefono,
                    Dominios = i.Dominios
                        .Where(d => d.Activo)
                        .Select(d => d.Dominio)
                        .ToList()
                })
                .ToListAsync();

            return Ok(instituciones);
        }

        // GET: api/Instituciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InstitucionDto>> GetInstitucion(int id)
        {
            var institucion = await _context.Instituciones
                .Where(i => i.Id == id && i.Activo)
                .Include(i => i.Dominios.Where(d => d.Activo))
                .Select(i => new InstitucionDto
                {
                    Id = i.Id,
                    Nombre = i.Nombre,
                    Email = i.Email,
                    Telefono = i.Telefono,
                    Dominios = i.Dominios
                        .Where(d => d.Activo)
                        .Select(d => d.Dominio)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (institucion == null)
                return NotFound(new { mensaje = $"No se encontró una institución con ID {id}" });

            return Ok(institucion);
        }

        // POST: api/Instituciones
        [HttpPost]
        public async Task<ActionResult<InstitucionDto>> CrearInstitucion([FromBody] CrearInstitucionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { mensaje = "El email es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Telefono))
                return BadRequest(new { mensaje = "El teléfono es requerido y no puede ser solo espacios en blanco" });

            if (dto.Dominios == null || dto.Dominios.Count == 0)
                return BadRequest(new { mensaje = "Debe especificar al menos un dominio" });

            if (dto.Dominios.Any(d => string.IsNullOrWhiteSpace(d)))
                return BadRequest(new { mensaje = "Los dominios no pueden estar vacíos ni ser solo espacios en blanco" });

            var institucion = new Institucion
            {
                Nombre = dto.Nombre.Trim(),
                Email = dto.Email.Trim(),
                Telefono = dto.Telefono.Trim(),
                Activo = true,
                Dominios = dto.Dominios
                    .Select(d => new InstitucionDominio
                    {
                        Dominio = d.Trim(),
                        Activo = true
                    })
                    .ToList()
            };

            _context.Instituciones.Add(institucion);
            await _context.SaveChangesAsync();

            // Registrar bitácora
            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Creó la institución '{institucion.Nombre}' (ID: {institucion.Id})",
                    token);
            }

            var resultado = new InstitucionDto
            {
                Id = institucion.Id,
                Nombre = institucion.Nombre,
                Email = institucion.Email,
                Telefono = institucion.Telefono,
                Dominios = institucion.Dominios.Select(d => d.Dominio).ToList()
            };

            return CreatedAtAction(nameof(GetInstitucion), new { id = institucion.Id }, resultado);
        }

        // PUT: api/Instituciones/5
        [HttpPut("{id}")]
        public async Task<ActionResult<InstitucionDto>> ActualizarInstitucion(int id, [FromBody] ActualizarInstitucionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { mensaje = "El email es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Telefono))
                return BadRequest(new { mensaje = "El teléfono es requerido y no puede ser solo espacios en blanco" });

            if (dto.Dominios == null || dto.Dominios.Count == 0)
                return BadRequest(new { mensaje = "Debe especificar al menos un dominio" });

            if (dto.Dominios.Any(d => string.IsNullOrWhiteSpace(d)))
                return BadRequest(new { mensaje = "Los dominios no pueden estar vacíos ni ser solo espacios en blanco" });

            var institucion = await _context.Instituciones
                .Include(i => i.Dominios)
                .FirstOrDefaultAsync(i => i.Id == id && i.Activo);

            if (institucion == null)
                return NotFound(new { mensaje = $"No se encontró una institución con ID {id}" });

            institucion.Nombre = dto.Nombre.Trim();
            institucion.Email = dto.Email.Trim();
            institucion.Telefono = dto.Telefono.Trim();

            _context.InstitucionDominios.RemoveRange(institucion.Dominios);

            institucion.Dominios = dto.Dominios
                .Select(d => new InstitucionDominio
                {
                    Dominio = d.Trim(),
                    Activo = true,
                    IdInstitucion = institucion.Id
                })
                .ToList();

            await _context.SaveChangesAsync();

            // Registrar bitácora
            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Modificó la institución '{institucion.Nombre}' (ID: {institucion.Id})",
                    token);
            }

            var resultado = new InstitucionDto
            {
                Id = institucion.Id,
                Nombre = institucion.Nombre,
                Email = institucion.Email,
                Telefono = institucion.Telefono,
                Dominios = institucion.Dominios.Select(d => d.Dominio).ToList()
            };

            return Ok(resultado);
        }

        // DELETE: api/Instituciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarInstitucion(int id)
        {
            var institucion = await _context.Instituciones
                .Include(i => i.Dominios)
                .FirstOrDefaultAsync(i => i.Id == id && i.Activo);

            if (institucion == null)
                return NotFound(new { mensaje = $"No se encontró una institución con ID {id}" });

            institucion.Activo = false;

            foreach (var dominio in institucion.Dominios)
            {
                dominio.Activo = false;
            }

            await _context.SaveChangesAsync();

            // Registrar bitácora
            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Eliminó la institución '{institucion.Nombre}' (ID: {institucion.Id})",
                    token);
            }

            return NoContent();
        }
    }
}