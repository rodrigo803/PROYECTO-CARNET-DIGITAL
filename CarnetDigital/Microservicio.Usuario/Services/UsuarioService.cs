using BCrypt.Net;
using Microservicio.Usuario.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; // <-- Requerido para IServiceScopeFactory
using QRCoder;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microservicio.Usuario.Entities.UsuarioDTOs;

namespace Microservicio.Usuario.Services
{
    public class UsuarioService : IUsuarioService
    {
        // Reemplazamos ApplicationDbContext directo por el generador de Scopes
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBitacoraService _bitacora;
        private readonly IConfiguration _config;

        public UsuarioService(IServiceScopeFactory scopeFactory, IBitacoraService bitacora, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _bitacora = bitacora;
            _config = config;
        }

        public async Task<UsuarioActualizacionDto> CrearUsuarioAsync(Entities.Usuario usuario, string contrasenaPlana)
        {
            // Creamos el hilo seguro para la base de datos
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            usuario.ContrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);
            usuario.EstadoId = 3;
            usuario.FotografiaBase64 = "";
            usuario.TokenConfirmacion = Guid.NewGuid().ToString();
            usuario.FechaExpiracionToken = DateTime.Now.AddMinutes(15);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El administrador registró al usuario {usuario.Identificacion}");

            await EnviarCorreoConfirmacion(usuario.Email, usuario.TokenConfirmacion);

            // Retornamos el DTO
            return new UsuarioActualizacionDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto
            };
        }

        public async Task<UsuarioActualizacionDto> AutoregistroAsync(UsuarioRegistroDto dto)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Mapeamos los datos del DTO a la Entidad Usuario
            var usuario = new Entities.Usuario
            {
                Identificacion = dto.Identificacion,
                Email = dto.Email,
                NombreCompleto = dto.NombreCompleto,
                TipoIdentificacionId = dto.TipoIdentificacionId,
                TipoUsuarioId = dto.TipoUsuarioId,
                RolId = dto.RolId,
                TipoIdentificacion = dto.TipoIdentificacion,
                TipoUsuario = dto.TipoUsuario,
                ContrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena), // Encriptamos directo del DTO
                EstadoId = 3,
                FotografiaBase64 = "",
                TokenConfirmacion = Guid.NewGuid().ToString(),
                FechaExpiracionToken = DateTime.Now.AddMinutes(15)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"Autoregistro exitoso. El usuario {usuario.Identificacion} quedó en estado Pendiente.");

            await EnviarCorreoConfirmacion(usuario.Email, usuario.TokenConfirmacion);

            return new UsuarioActualizacionDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto
            };
        }

        private async Task EnviarCorreoConfirmacion(string emailDestino, string token)
        {
            try
            {
                string enlaceConfirmacion = $"https://localhost:7123/api/usuario/autoregistro/confirmar?token={token}";

                var smtpSettings = _config.GetSection("SmtpSettings");
                string servidor = smtpSettings["Server"] ?? "smtp.gmail.com";
                int puerto = int.TryParse(smtpSettings["Port"], out int p) ? p : 587;
                string correoEmisor = smtpSettings["SenderEmail"];
                string contrasenaAplicacion = smtpSettings["AppPassword"];

                MailMessage correo = new MailMessage();
                correo.From = new MailAddress(correoEmisor, "Carnet Digital CUC");
                correo.To.Add(emailDestino);
                correo.Subject = "Confirma tu registro en Carnet Digital CUC";
                correo.Body = $"<h1>Bienvenido</h1><p>Para activar tu cuenta, haz clic en el siguiente enlace antes de 15 minutos:</p><br><a href='{enlaceConfirmacion}'>Confirmar Cuenta</a>";
                correo.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(servidor, puerto);
                smtp.Credentials = new NetworkCredential(correoEmisor, contrasenaAplicacion);
                smtp.EnableSsl = true;

                smtp.Send(correo);

                await _bitacora.RegistrarAccionAsync(0, $"Sistema: Se envió el correo de confirmación a la dirección {emailDestino}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo: " + ex.Message);
                await _bitacora.RegistrarAccionAsync(0, $"Sistema Error: Fallo al enviar correo a {emailDestino}. Detalle: {ex.Message}");
            }
        }

        public async Task<UsuarioActualizacionDto?> ConfirmarRegistroAsync(string token)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenConfirmacion == token);

            if (usuario == null) throw new Exception("Token inválido o usuario no encontrado.");

            if (DateTime.Now > usuario.FechaExpiracionToken)
                throw new Exception("El token ha expirado. Han pasado más de 15 minutos.");

            usuario.EstadoId = 1;
            usuario.TokenConfirmacion = null;
            usuario.FechaExpiracionToken = null;

            await _context.SaveChangesAsync();

            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El usuario {usuario.Identificacion} confirmó su cuenta exitosamente mediante el token.");

            return new UsuarioActualizacionDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto
            };
        }

        public async Task<UsuarioActualizacionDto?> CambiarEstadoAsync(string Identificacion, int nuevoEstadoId)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = await _context.Usuarios.FindAsync(Identificacion);
            if (usuario == null) return null; // Retornamos null en lugar de false

            var estadoExiste = await _context.EstadoUsuario.AnyAsync(e => e.Id == nuevoEstadoId);
            if (!estadoExiste) throw new Exception("El estado indicado no existe.");

            usuario.EstadoId = nuevoEstadoId;
            await _context.SaveChangesAsync();

            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El estado del usuario {usuario.Identificacion} fue cambiado al EstadoId: {usuario.EstadoId}.");

            return new UsuarioActualizacionDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto
            };
        }

        public async Task<UsuarioActualizacionDto?> ActualizarFotografiaAsync(FotografiaDto peticion)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            int longitud = peticion.FotoBase64.Length;
            int padding = peticion.FotoBase64.EndsWith("==") ? 2 : (peticion.FotoBase64.EndsWith("=") ? 1 : 0);
            long pesoBytes = (long)(longitud * 3 / 4) - padding;

            if (pesoBytes > 1048576)
            {
                throw new Exception("La imagen supera el límite de 1MB.");
            }

            // Buscamos usando el DTO
            var usuario = await _context.Usuarios.FindAsync(peticion.Identificacion);
            if (usuario == null) return null;

            // Actualizamos usando el DTO
            usuario.FotografiaBase64 = peticion.FotoBase64;
            await _context.SaveChangesAsync();

            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El usuario {usuario.Identificacion} actualizó su fotografía del carnet.");

            return new UsuarioActualizacionDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto
            };
        }

        public async Task<string> GenerarQRBase64Async(string identificacion)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Identificacion == identificacion);

            if (usuario == null)
                throw new Exception("Usuario no encontrado con esa identificación.");

            var datosCarnet = new
            {
                Nombre = usuario.NombreCompleto,
                Identificacion = usuario.Identificacion,
                Tipo = usuario.TipoUsuarioId == 1 ? "Estudiante" : "Funcionario",
                Institucion = "Colegio Universitario de Cartago",
                CarrerasAreas = "Programación / TI",
                Vencimiento = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd")
            };

            string jsonString = JsonSerializer.Serialize(datosCarnet);

            using QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(jsonString, QRCodeGenerator.ECCLevel.Q);

            using PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImageBytes = qrCode.GetGraphic(20);

            int cedulaInt = int.TryParse(identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"Se generó y consultó el código QR para la identificación {identificacion}.");

            return Convert.ToBase64String(qrCodeImageBytes);
        }

        public async Task<UsuarioActualizacionDto?> ActualizarUsuarioAsync(UsuarioActualizacionDto registro)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = await _context.Usuarios.FindAsync(registro.Identificacion);
            if (usuario == null) return null;

            usuario.NombreCompleto = registro.NombreCompleto;
            usuario.Email = registro.Email;

            await _context.SaveChangesAsync();

            int cedulaInt = int.TryParse(registro.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"Se modificaron los datos personales del usuario {registro.Identificacion}.");

            return new UsuarioActualizacionDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto
            };
        }

        public async Task<UsuarioActualizacionDto?> EliminarUsuarioAsync(string identificacion)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = await _context.Usuarios.FindAsync(identificacion);

            if (usuario == null)
            {
                await _bitacora.RegistrarAccionAsync(0, $"Alerta: Intento fallido de eliminación. Usuario {identificacion} no encontrado.");
                return null;
            }

            // Rescatamos los datos antes de eliminarlos de la BD
            var datosEliminados = new UsuarioActualizacionDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto
            };

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El usuario con cédula {usuario.Identificacion} fue eliminado permanentemente del sistema.");

            return datosEliminados;
        }
    }
}
