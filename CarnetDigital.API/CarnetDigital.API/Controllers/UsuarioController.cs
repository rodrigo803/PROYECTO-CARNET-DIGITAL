using CarnetDigital.Core.Entities;
using CarnetDigital.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
                // Los datos no pueden ser vacíos 
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

        [AllowAnonymous]
        [HttpPost("autoregistro")]
        public async Task<IActionResult> Autoregistro([FromBody] UsuarioRegistroDto dto)
        {
            try
            {
                var nuevoUsuario = new Usuario
                {
                    Email = dto.Email,
                    NombreCompleto = dto.NombreCompleto,
                    Identificacion = dto.Identificacion,
                    TipoIdentificacionId = dto.TipoIdentificacionId,
                    TipoUsuarioId = dto.TipoUsuarioId,
                    RolId = dto.RolId
                };

                await _usuarioService.AutoregistroAsync(nuevoUsuario, dto.Contrasena);
                return Ok(new { Mensaje = "Registro exitoso. Revisa tu correo electrónico para confirmar la cuenta (expira en 15 minutos)." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("autoregistro/confirmar")]
        public async Task<IActionResult> ConfirmarRegistro([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return BadRequest("El token no puede estar vacío.");

                await _usuarioService.ConfirmarRegistroAsync(token);

                // Retornamos un HTML simple para que el usuario lo vea bonito en el navegador
                return Content("<html><body><h2>¡Tu cuenta ha sido activada exitosamente!</h2><p>Ya puedes iniciar sesión en la aplicación móvil.</p></body></html>", "text/html");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }

        [Authorize] // Asegura que solo usuarios logueados puedan pedir el QR
        [HttpGet("usuario/qr/{identificacion}")]
        public async Task<IActionResult> ObtenerQR(string identificacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identificacion))
                    return BadRequest("Debe proveer una identificación válida.");

                var qrBase64 = await _usuarioService.GenerarQRBase64Async(identificacion);

                // Retornamos el Base64. 
                // El frontend o la app móvil solo tendrá que poner: "data:image/png;base64," + qrBase64
                return Ok(new
                {
                    Identificacion = identificacion,
                    QrImagenBase64 = qrBase64,
                    Formato = "image/png"
                });
            }
            catch (Exception ex)
            {
                return NotFound(new { Mensaje = ex.Message });
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
        public string Rol { get; internal set; }
        public int TipoIdentificacionId { get; internal set; }
        public int TipoUsuarioId { get; internal set; }
        public int RolId { get; internal set; }
    }

    public class CambioEstadoDto
    {
        public string? Email { get; set; }
        public int EstadoId { get; set; }
    }

    public class FotografiaDto
    {
        public string? Email { get; set; }
        public string? FotoBase64 { get; set; }
    }
}