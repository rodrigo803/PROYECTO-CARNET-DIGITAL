using CarnetDigital.Core.Entities;
using CarnetDigital.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarnetDigital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        // Inyección de dependencias
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioRegistroDto dto)
        {
            var nuevoUsuario = new Usuario
            {
                Email = dto.Email,
                NombreCompleto = dto.NombreCompleto,
                Identificacion = dto.Identificacion,
                TipoIdentificacion = dto.TipoIdentificacion,
                TipoUsuario = dto.TipoUsuario
            };

            // Llamamos a la lógica para encriptar y procesar
            var resultado = await _usuarioService.CrearUsuarioAsync(nuevoUsuario, dto.Contrasena);

            return Ok(new { Mensaje = "Usuario creado exitosamente", Usuario = resultado.Email });
        }

        [Authorize]
        [HttpPatch("usuarios/estado")] // Endpoint exacto
        public async Task<IActionResult> CambiarEstado([FromBody] CambioEstadoDto peticion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(peticion.Email))
                    return BadRequest("El identificador no puede estar vacío");

                var exito = await _usuarioService.CambiarEstadoAsync(peticion.Email, peticion.EstadoId);
                if (!exito) return NotFound(new { Mensaje = "Usuario no encontrado" });

                return Ok(new { Mensaje = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("usuario/fotografia")]
        public async Task<IActionResult> ActualizarFotografia([FromBody] FotografiaDto peticion)
        {
            try
            {
                [cite_start]// Los datos no pueden ser vacíos 
                if (string.IsNullOrWhiteSpace(peticion.FotoBase64) || string.IsNullOrWhiteSpace(peticion.Email))
                    return BadRequest("Todos los datos son requeridos.");

                var exito = await _usuarioService.ActualizarFotografiaAsync(peticion.Email, peticion.FotoBase64);
                if (!exito) return NotFound("Usuario no encontrado.");

                return Ok(new { Mensaje = "Fotografía actualizada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }

    // DTO (Data Transfer Object) para recibir solo los datos necesarios en la petición
    public class UsuarioRegistroDto
    {
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public string Identificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string TipoUsuario { get; set; }
        public string Contrasena { get; set; }
    }

    public class CambioEstadoDto
    {
        public string Email { get; set; }
        public int EstadoId { get; set; }
    }

    public class FotografiaDto
    {
        public string Email { get; set; }
        public string FotoBase64 { get; set; }
    }
}