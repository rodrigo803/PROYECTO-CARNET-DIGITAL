using BCrypt.Net;
using Microservicio.Usuario.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly ApplicationDbContext _context;
        private readonly IBitacoraService _bitacora;
        private readonly IConfiguration _config; // <-- ¡Movido adentro de la clase!

        // <-- ¡Agregado el parámetro al constructor!
        public UsuarioService(ApplicationDbContext context, IBitacoraService bitacora, IConfiguration config)
        {
            _context = context;
            _bitacora = bitacora;
            _config = config;
        }

        public async Task<Entities.Usuario> CrearUsuarioAsync(Entities.Usuario usuario, string contrasenaPlana)
        {
            // 1. Encriptar la contraseña
            usuario.ContrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);

            // 2. Reglas de negocio iniciales (Modificado para requerir confirmación)
            usuario.EstadoId = 3; // 3 = Pendiente_Confirmacion
            usuario.FotografiaBase64 = "";

            // Generar Token y fecha de expiración
            usuario.TokenConfirmacion = Guid.NewGuid().ToString();
            usuario.FechaExpiracionToken = DateTime.Now.AddMinutes(15);

            // 3. Guardar en la base de datos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // 4. BITÁCORA
            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El administrador registró al usuario {usuario.Identificacion}");

            // 5. Enviar el correo electrónico
            await EnviarCorreoConfirmacion(usuario.Email, usuario.TokenConfirmacion);

            return usuario;
        }

        public async Task<bool> AutoregistroAsync(Entities.Usuario usuario, string contrasenaPlana)
        {
            // 1. Encriptar contraseña y poner estado inicial
            usuario.ContrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);
            usuario.EstadoId = 3; // 3 = Pendiente_Confirmacion
            usuario.FotografiaBase64 = ""; // Aseguramos que no vaya nulo

            // 2. Generar Token y fecha de expiración (15 minutos a partir de ahora)
            usuario.TokenConfirmacion = Guid.NewGuid().ToString();
            usuario.FechaExpiracionToken = DateTime.Now.AddMinutes(15);

            // 3. Guardar en la base de datos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // 4. BITÁCORA
            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"Autoregistro exitoso. El usuario {usuario.Identificacion} quedó en estado Pendiente.");

            // 5. Enviar el correo electrónico (Ahora con await)
            await EnviarCorreoConfirmacion(usuario.Email, usuario.TokenConfirmacion);

            return true;
        }

        private async Task EnviarCorreoConfirmacion(string emailDestino, string token)
        {
            try
            {
                string enlaceConfirmacion = $"https://localhost:7123/api/usuario/autoregistro/confirmar?token={token}";

                // 1. Extraemos la sección de configuración del appsettings.json
                var smtpSettings = _config.GetSection("SmtpSettings");
                string servidor = smtpSettings["Server"] ?? "smtp.gmail.com";
                int puerto = int.TryParse(smtpSettings["Port"], out int p) ? p : 587;
                string correoEmisor = smtpSettings["SenderEmail"];
                string contrasenaAplicacion = smtpSettings["AppPassword"];

                MailMessage correo = new MailMessage();
                // Usamos el correo emisor dinámico de la configuración
                correo.From = new MailAddress(correoEmisor, "Carnet Digital CUC");
                correo.To.Add(emailDestino);
                correo.Subject = "Confirma tu registro en Carnet Digital CUC";
                correo.Body = $"<h1>Bienvenido</h1><p>Para activar tu cuenta, haz clic en el siguiente enlace antes de 15 minutos:</p><br><a href='{enlaceConfirmacion}'>Confirmar Cuenta</a>";
                correo.IsBodyHtml = true;

                // 2. Configuramos el cliente SMTP con las variables leídas
                SmtpClient smtp = new SmtpClient(servidor, puerto);
                smtp.Credentials = new NetworkCredential(correoEmisor, contrasenaAplicacion);
                smtp.EnableSsl = true;

                smtp.Send(correo);

                // BITÁCORA (Acción del sistema)
                await _bitacora.RegistrarAccionAsync(0, $"Sistema: Se envió el correo de confirmación a la dirección {emailDestino}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo: " + ex.Message);
                await _bitacora.RegistrarAccionAsync(0, $"Sistema Error: Fallo al enviar correo a {emailDestino}. Detalle: {ex.Message}");
            }
        }

        public async Task<bool> ConfirmarRegistroAsync(string token)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenConfirmacion == token);

            if (usuario == null) throw new Exception("Token inválido o usuario no encontrado.");

            if (DateTime.Now > usuario.FechaExpiracionToken)
                throw new Exception("El token ha expirado. Han pasado más de 15 minutos.");

            // Activar el usuario y limpiar el token por seguridad
            usuario.EstadoId = 1; // 1 = Activo
            usuario.TokenConfirmacion = null;
            usuario.FechaExpiracionToken = null;

            await _context.SaveChangesAsync();

            // BITÁCORA
            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El usuario {usuario.Identificacion} confirmó su cuenta exitosamente mediante el token.");

            return true;
        }

        public async Task<bool> CambiarEstadoAsync(string Identificacion, int nuevoEstadoId)
        {
            var usuario = await _context.Usuarios.FindAsync(Identificacion);
            if (usuario == null) return false;

            var estadoExiste = await _context.EstadoUsuario.AnyAsync(e => e.Id == nuevoEstadoId);
            if (!estadoExiste) throw new Exception("El estado indicado no existe.");

            usuario.EstadoId = nuevoEstadoId;
            await _context.SaveChangesAsync();

            // BITÁCORA
            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El estado del usuario {usuario.Identificacion} fue cambiado al EstadoId: {usuario.EstadoId}.");   

            return true;
        }

        public async Task<bool> ActualizarFotografiaAsync(string Identificacion, string fotoBase64)
        {
            int longitud = fotoBase64.Length;
            int padding = fotoBase64.EndsWith("==") ? 2 : (fotoBase64.EndsWith("=") ? 1 : 0);
            long pesoBytes = (long)(longitud * 3 / 4) - padding;

            if (pesoBytes > 1048576)
            {
                throw new Exception("La imagen supera el límite de 1MB.");
            }

            var usuario = await _context.Usuarios.FindAsync(Identificacion);
            if (usuario == null) return false;

            usuario.FotografiaBase64 = fotoBase64;
            await _context.SaveChangesAsync();

            // BITÁCORA
            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El usuario {usuario.Identificacion} actualizó su fotografía del carnet.");

            return true;
        }

        public async Task<string> GenerarQRBase64Async(string identificacion)
        {
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

            // BITÁCORA
            int cedulaInt = int.TryParse(identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"Se generó y consultó el código QR para la identificación {identificacion}.");

            return Convert.ToBase64String(qrCodeImageBytes);
        }

        public async Task<bool> ActualizarUsuarioAsync(UsuarioActualizacionDto registro)
        {
            var usuario = await _context.Usuarios.FindAsync(registro.Identificacion);
            if (usuario == null) return false;

            usuario.NombreCompleto = registro.NombreCompleto;
            usuario.Email = registro.Email; // <--- Esto está bien, el correo es mutable ahora

            await _context.SaveChangesAsync();

            // BITÁCORA
            int cedulaInt = int.TryParse(registro.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"Se modificaron los datos personales del usuario {registro.Identificacion}.");

            return true;
        }

        public async Task<bool> EliminarUsuarioAsync(string identificacion) // <-- CAMBIO AQUÍ
        {
            // FindAsync busca automáticamente por la Llave Primaria (que ahora es la cédula)
            var usuario = await _context.Usuarios.FindAsync(identificacion); // <-- CAMBIO AQUÍ

            if (usuario == null)
            {
                await _bitacora.RegistrarAccionAsync(0, $"Alerta: Intento fallido de eliminación. Usuario {identificacion} no encontrado.");
                return false;
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El usuario con cédula {usuario.Identificacion} fue eliminado permanentemente del sistema.");

            return true;
        }
    }
}