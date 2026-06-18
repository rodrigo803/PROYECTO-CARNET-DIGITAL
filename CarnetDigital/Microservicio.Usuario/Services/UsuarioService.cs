using BCrypt.Net;
using Microservicio.Usuario.Entities;
using Microservicio.Usuario.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microservicio.Usuario.Entities.UsuarioDTOs;
using static Microservicio.Usuario.Services.CatalogosApiClient;

namespace Microservicio.Usuario.Services
{
    public class UsuarioService : IUsuarioService
    {
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
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _catalogos = scope.ServiceProvider.GetRequiredService<ICatalogosApiClient>(); // Inyectamos cliente

            // 1. REGLAS DE NEGOCIO DEL CARNET DIGITAL
            if (usuario.InstitucionesIds == null || usuario.InstitucionesIds.Count == 0)
                throw new Exception("El usuario debe pertenecer a al menos una institución.");

            if (usuario.TipoUsuarioId == 1 && (usuario.CarrerasIds == null || usuario.CarrerasIds.Count == 0))
                throw new Exception("Si el usuario es estudiante, debe tener carreras asociadas.");

            if (usuario.TipoUsuarioId == 2 && (usuario.AreasIds == null || usuario.AreasIds.Count == 0))
                throw new Exception("Si el usuario es funcionario, debe tener áreas asociadas.");


            // 2. VALIDAR CONTRA LOS MICROSERVICIOS
            await _catalogos.ValidarCatalogosAsync(usuario.TipoIdentificacionId, usuario.TipoUsuarioId, usuario.InstitucionesIds, usuario.CarrerasIds, usuario.AreasIds);

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
            var _catalogos = scope.ServiceProvider.GetRequiredService<ICatalogosApiClient>(); // Inyectamos cliente

            // 1. REGLAS DE NEGOCIO DEL CARNET DIGITAL
            if (dto.InstitucionesIds == null || dto.InstitucionesIds.Count == 0)
                throw new Exception("El usuario debe pertenecer a al menos una institución.");

            if (dto.TipoUsuarioId == 1 && (dto.CarrerasIds == null || dto.CarrerasIds.Count == 0))
                throw new Exception("Si el usuario es estudiante, debe tener carreras asociadas.");

            if (dto.TipoUsuarioId == 2 && (dto.AreasIds == null || dto.AreasIds.Count == 0))
                throw new Exception("Si el usuario es funcionario, debe tener áreas asociadas.");

            // --- ESTO TE DIRÁ LA VERDAD ---
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"DEBUG: Instituciones recibidas: {(dto.InstitucionesIds != null ? dto.InstitucionesIds.Count : "NULL")}");
            Console.WriteLine($"DEBUG: Carreras recibidas: {(dto.CarrerasIds != null ? dto.CarrerasIds.Count : "NULL")}");
            Console.WriteLine($"DEBUG: JSON completo: {JsonSerializer.Serialize(dto)}");
            Console.WriteLine("------------------------------------------");

            // 2. VALIDAR CONTRA LOS MICROSERVICIOS
            await _catalogos.ValidarCatalogosAsync(dto.TipoIdentificacionId, dto.TipoUsuarioId, dto.InstitucionesIds, dto.CarrerasIds, dto.AreasIds);

            var usuario = new Entities.Usuario
            {
                Identificacion = dto.Identificacion,
                Email = dto.Email,
                NombreCompleto = dto.NombreCompleto,
                TipoIdentificacionId = dto.TipoIdentificacionId,
                TipoUsuarioId = dto.TipoUsuarioId,
                RolId = dto.RolId,
                TipoIdentificacion = dto.TipoIdentificacion, // Opcional si ya solo usas IDs
                TipoUsuario = dto.TipoUsuario,               // Opcional si ya solo usas IDs
                ContrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                EstadoId = 3,
                FotografiaBase64 = "",
                TokenConfirmacion = Guid.NewGuid().ToString(),
                FechaExpiracionToken = DateTime.Now.AddMinutes(15),

                // Mapeo de las listas
                InstitucionesIds = dto.InstitucionesIds,
                CarrerasIds = dto.CarrerasIds,
                AreasIds = dto.AreasIds
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
            if (usuario == null) return null;

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

            var usuario = await _context.Usuarios.FindAsync(peticion.Identificacion);
            if (usuario == null) return null;

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
            var _catalogos = scope.ServiceProvider.GetRequiredService<ICatalogosApiClient>(); // Inyectamos cliente

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Identificacion == identificacion);

            if (usuario == null)
                throw new Exception("Usuario no encontrado con esa identificación.");

            // Consultas asíncronas para obtener los nombres reales del ecosistema
            string nombreTipo = await _catalogos.ObtenerNombreTipoUsuarioAsync(usuario.TipoUsuarioId);
            string nombresInstituciones = await _catalogos.ObtenerNombresInstitucionesAsync(usuario.InstitucionesIds);
            string nombresCarrerasAreas = await _catalogos.ObtenerNombresCarrerasOAreasAsync(usuario.TipoUsuarioId, usuario.CarrerasIds, usuario.AreasIds);

            // Reemplazo del texto estático por los datos reales de los catálogos
            var datosCarnet = new
            {
                Nombre = usuario.NombreCompleto,
                Identificacion = usuario.Identificacion,
                Tipo = nombreTipo,
                Institucion = nombresInstituciones,
                CarrerasAreas = nombresCarrerasAreas,
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