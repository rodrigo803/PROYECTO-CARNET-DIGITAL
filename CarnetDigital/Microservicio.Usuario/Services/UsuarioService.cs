using BCrypt.Net;
using Microservicio.Usuario.Repository;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace Microservicio.Usuario.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Entities.Usuario> CrearUsuarioAsync(Entities.Usuario usuario, string contrasenaPlana)
        {
            // 1. Encriptar la contraseña
            usuario.ContrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);

            // 2. Reglas de negocio iniciales (CORREGIDO A Id NUMÉRICO)
            // 1 = Activo, 2 = Inactivo, 3 = Pendiente_Confirmacion
            usuario.EstadoId = 1;

            // 3. Guardar en la base de datos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        public async Task<bool> AutoregistroAsync(Entities.Usuario usuario, string contrasenaPlana)
        {
            // 1. Encriptar contraseña y poner estado inicial (CORREGIDO A Id NUMÉRICO)
            usuario.ContrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);
            usuario.EstadoId = 3; // 3 = Pendiente_Confirmacion

            // 2. Generar Token y fecha de expiración (15 minutos a partir de ahora)
            usuario.TokenConfirmacion = Guid.NewGuid().ToString();
            usuario.FechaExpiracionToken = DateTime.Now.AddMinutes(15);

            // 3. Guardar en la base de datos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // 4. Enviar el correo electrónico
            EnviarCorreoConfirmacion(usuario.Email, usuario.TokenConfirmacion);

            return true;
        }

        private void EnviarCorreoConfirmacion(string emailDestino, string token)
        {
            try
            {
                string enlaceConfirmacion = $"https://localhost:7123/api/usuario/autoregistro/confirmar?token={token}";

                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("tu_correo_de_prueba@gmail.com");
                correo.To.Add(emailDestino);
                correo.Subject = "Confirma tu registro en Carnet Digital CUC";
                correo.Body = $"<h1>Bienvenido</h1><p>Para activar tu cuenta, haz clic en el siguiente enlace antes de 15 minutos:</p><br><a href='{enlaceConfirmacion}'>Confirmar Cuenta</a>";
                correo.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("tu_correo_de_prueba@gmail.com", "tu_contraseña_de_aplicacion");
                smtp.EnableSsl = true;

                smtp.Send(correo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error enviando correo: " + ex.Message);
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
            return true;
        }

        public async Task<bool> CambiarEstadoAsync(string email, int nuevoEstadoId)
        {
            var usuario = await _context.Usuarios.FindAsync(email);
            if (usuario == null) return false;

            var estadoExiste = await _context.EstadoUsuario.AnyAsync(e => e.Id == nuevoEstadoId);
            if (!estadoExiste) throw new Exception("El estado indicado no existe.");

            usuario.EstadoId = nuevoEstadoId;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActualizarFotografiaAsync(string email, string fotoBase64)
        {
            int longitud = fotoBase64.Length;
            int padding = fotoBase64.EndsWith("==") ? 2 : (fotoBase64.EndsWith("=") ? 1 : 0);
            long pesoBytes = (long)(longitud * 3 / 4) - padding;

            if (pesoBytes > 1048576)
            {
                throw new Exception("La imagen supera el límite de 1MB.");
            }

            var usuario = await _context.Usuarios.FindAsync(email);
            if (usuario == null) return false;

            usuario.FotografiaBase64 = fotoBase64;
            await _context.SaveChangesAsync();

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

            return Convert.ToBase64String(qrCodeImageBytes);
        }

        public async Task<bool> ActualizarUsuarioAsync(Entities.Usuario registro)
        {
            var usuario = await _context.Usuarios.FindAsync(registro.Email);
            if (usuario == null) return false;

            usuario.NombreCompleto = registro.NombreCompleto;
            usuario.Identificacion = registro.Identificacion;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarUsuarioAsync(string email)
        {
            var usuario = await _context.Usuarios.FindAsync(email);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
