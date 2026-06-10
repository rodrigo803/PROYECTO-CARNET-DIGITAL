using Microservicio.Carreras.Data;
using Microservicio.Carreras.DTOs;
using Microservicio.Carreras.Entities;
using Microservicio.Carreras.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Carreras.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CarrerasController : ControllerBase
    {
        private readonly CarrerasDbContext _context;
        private readonly IBitacoraService _bitacoraService;
        private readonly IInstitucionesValidator _institucionesValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarrerasController(
            CarrerasDbContext context,
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

        // GET: api/Carreras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarreraDto>>> GetCarreras()
        {
            var token = GetToken();
            if (token == null)
                return Unauthorized();

            var carreras = await _context.Carreras
                .Where(c => c.Activo)
                .ToListAsync();

            var resultado = new List<CarreraDto>();

            foreach (var carrera in carreras)
            {
                var institucion = await _institucionesValidator.ObtenerInstitucionAsync(carrera.IdInstitucion, token);

                resultado.Add(new CarreraDto
                {
                    Id = carrera.Id,
                    Nombre = carrera.Nombre,
                    Director = carrera.Director,
                    Email = carrera.Email,
                    Telefono = carrera.Telefono,
                    IdInstitucion = carrera.IdInstitucion,
                    NombreInstitucion = institucion?.Nombre ?? "(Institución no disponible)"
                });
            }

            return Ok(resultado);
        }

        // GET: api/Carreras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarreraDto>> GetCarrera(int id)
        {
            var token = GetToken();
            if (token == null)
                return Unauthorized();

            var carrera = await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

            if (carrera == null)
                return NotFound(new { mensaje = $"No se encontró una carrera con ID {id}" });

            var institucion = await _institucionesValidator.ObtenerInstitucionAsync(carrera.IdInstitucion, token);

            return Ok(new CarreraDto
            {
                Id = carrera.Id,
                Nombre = carrera.Nombre,
                Director = carrera.Director,
                Email = carrera.Email,
                Telefono = carrera.Telefono,
                IdInstitucion = carrera.IdInstitucion,
                NombreInstitucion = institucion?.Nombre ?? "(Institución no disponible)"
            });
        }

        // POST: api/Carreras
        [HttpPost]
        public async Task<ActionResult<CarreraDto>> CrearCarrera([FromBody] CrearCarreraDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Director))
                return BadRequest(new { mensaje = "El director es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { mensaje = "El email es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Telefono))
                return BadRequest(new { mensaje = "El teléfono es requerido y no puede ser solo espacios en blanco" });

            var token = GetToken();
            if (token == null)
                return Unauthorized();

            // Validar que la institución exista vía HTTP al Microservicio.Instituciones
            var institucion = await _institucionesValidator.ObtenerInstitucionAsync(dto.IdInstitucion, token);
            if (institucion == null)
                return BadRequest(new { mensaje = $"No existe una institución activa con ID {dto.IdInstitucion}" });

            var carrera = new Carrera
            {
                Nombre = dto.Nombre.Trim(),
                Director = dto.Director.Trim(),
                Email = dto.Email.Trim(),
                Telefono = dto.Telefono.Trim(),
                IdInstitucion = dto.IdInstitucion,
                Activo = true
            };

            _context.Carreras.Add(carrera);
            await _context.SaveChangesAsync();

            // Bitácora
            var usuarioId = GetUsuarioId();
            if (usuarioId.HasValue)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Creó la carrera '{carrera.Nombre}' (ID: {carrera.Id})",
                    token);
            }

            return CreatedAtAction(nameof(GetCarrera), new { id = carrera.Id }, new CarreraDto
            {
                Id = carrera.Id,
                Nombre = carrera.Nombre,
                Director = carrera.Director,
                Email = carrera.Email,
                Telefono = carrera.Telefono,
                IdInstitucion = carrera.IdInstitucion,
                NombreInstitucion = institucion.Nombre
            });
        }

        // PUT: api/Carreras/5
        [HttpPut("{id}")]
        public async Task<ActionResult<CarreraDto>> ActualizarCarrera(int id, [FromBody] ActualizarCarreraDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Director))
                return BadRequest(new { mensaje = "El director es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { mensaje = "El email es requerido y no puede ser solo espacios en blanco" });

            if (string.IsNullOrWhiteSpace(dto.Telefono))
                return BadRequest(new { mensaje = "El teléfono es requerido y no puede ser solo espacios en blanco" });

            var token = GetToken();
            if (token == null)
                return Unauthorized();

            var carrera = await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

            if (carrera == null)
                return NotFound(new { mensaje = $"No se encontró una carrera con ID {id}" });

            var institucion = await _institucionesValidator.ObtenerInstitucionAsync(dto.IdInstitucion, token);
            if (institucion == null)
                return BadRequest(new { mensaje = $"No existe una institución activa con ID {dto.IdInstitucion}" });

            carrera.Nombre = dto.Nombre.Trim();
            carrera.Director = dto.Director.Trim();
            carrera.Email = dto.Email.Trim();
            carrera.Telefono = dto.Telefono.Trim();
            carrera.IdInstitucion = dto.IdInstitucion;

            await _context.SaveChangesAsync();

            // Bitácora
            var usuarioId = GetUsuarioId();
            if (usuarioId.HasValue)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Modificó la carrera '{carrera.Nombre}' (ID: {carrera.Id})",
                    token);
            }

            return Ok(new CarreraDto
            {
                Id = carrera.Id,
                Nombre = carrera.Nombre,
                Director = carrera.Director,
                Email = carrera.Email,
                Telefono = carrera.Telefono,
                IdInstitucion = carrera.IdInstitucion,
                NombreInstitucion = institucion.Nombre
            });
        }

        // DELETE: api/Carreras/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCarrera(int id)
        {
            var carrera = await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

            if (carrera == null)
                return NotFound(new { mensaje = $"No se encontró una carrera con ID {id}" });

            carrera.Activo = false;
            await _context.SaveChangesAsync();

            // Bitácora
            var usuarioId = GetUsuarioId();
            var token = GetToken();
            if (usuarioId.HasValue && token != null)
            {
                await _bitacoraService.RegistrarAsync(
                    usuarioId.Value,
                    $"Eliminó la carrera '{carrera.Nombre}' (ID: {carrera.Id})",
                    token);
            }

            return NoContent();
        }
    }
}