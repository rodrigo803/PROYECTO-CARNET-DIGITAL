using System;
using System.Collections.Generic;
using System.Text;
using CarnetDigital.Core.Entities;
using CarnetDigital.Core.Interfaces;
using CarnetDigital.Data.Data;
using BCrypt.Net;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using QRCoder;

namespace CarnetDigital.Core.Services
{
    public class UsuarioService : IUsuarioService
    {
        // En el futuro, aquí inyectaremos el repositorio para guardar en SQL
        // private readonly IUsuarioRepository _repository;

        // 1. Declaramos la variable de la base de datos
        private readonly ApplicationDbContext _context;

        // 2. El constructor que inyecta la conexión a SQL Server
        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> CrearUsuarioAsync(Usuario usuario, string contrasenaPlana)
        {
            // 1. Encriptar la contraseña
            usuario.ContrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);

            // 2. Reglas de negocio iniciales
            usuario.Estado = "activo";

            // TODO: Llamar al repositorio para guardar en base de datos
            // await _repository.AddAsync(usuario);

            return usuario;
        }

        public async Task<bool> AutoregistroAsync(Usuario usuario, string contrasenaPlana)
        {
            // 1. Encriptar contraseña y poner estado inicial
            usuario.ContrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);
            usuario.Estado = "pendiente"; // Suponiendo que "pendiente" es "Pendiente de Confirmación" en tu tabla EstadoUsuario

            // 2. Generar Token y fecha de expiración (15 minutos a partir de ahora)
            usuario.TokenConfirmacion = Guid.NewGuid().ToString(); // Genera un código único aleatorio
            usuario.FechaExpiracionToken = DateTime.Now.AddMinutes(15);

            // 3. Guardar en la base de datos
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // 4. Enviar el correo electrónico
            EnviarCorreoConfirmacion(usuario.Email, usuario.TokenConfirmacion);

            return true;
        }

        // Método auxiliar para el envío del correo (Usando SMTP de Gmail para desarrollo local)
        private void EnviarCorreoConfirmacion(string emailDestino, string token)
        {
            try
            {
                // Enlace simulado que apuntará a tu API local
                string enlaceConfirmacion = $"https://localhost:7123/api/usuario/autoregistro/confirmar?token={token}";

                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("tu_correo_de_prueba@gmail.com");
                correo.To.Add(emailDestino);
                correo.Subject = "Confirma tu registro en Carnet Digital CUC";
                correo.Body = $"<h1>Bienvenido</h1><p>Para activar tu cuenta, haz clic en el siguiente enlace antes de 15 minutos:</p><br><a href='{enlaceConfirmacion}'>Confirmar Cuenta</a>";
                correo.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                // Ocupas generar una "Contraseña de aplicación" en tu cuenta de Google para esto
                smtp.Credentials = new NetworkCredential("tu_correo_de_prueba@gmail.com", "tu_contraseña_de_aplicacion");
                smtp.EnableSsl = true;

                smtp.Send(correo);
            }
            catch (Exception ex)
            {
                // Manejar el error si el correo falla
                Console.WriteLine("Error enviando correo: " + ex.Message);
            }
        }
        public async Task<bool> ConfirmarRegistroAsync(string token)
        {
            // Buscar si existe un usuario con ese token
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenConfirmacion == token);

            if (usuario == null) throw new Exception("Token inválido o usuario no encontrado.");

            // Validar el tiempo de expiración
            if (DateTime.Now > usuario.FechaExpiracionToken)
                throw new Exception("El token ha expirado. Han pasado más de 15 minutos.");

            // Activar el usuario y limpiar el token por seguridad
            usuario.EstadoId = 1; // Suponiendo que 1 es "Activo"
            usuario.TokenConfirmacion = null;
            usuario.FechaExpiracionToken = null;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CambiarEstadoAsync(string email, int nuevoEstadoId)
        {
            // 1. Buscar el usuario
            var usuario = await _context.Usuarios.FindAsync(email);
            if (usuario == null) return false;

            // 2. Validar que el estado exista en la base de datos
            var estadoExiste = await _context.EstadoUsuario.AnyAsync(e => e.Id == nuevoEstadoId);
            if (!estadoExiste) throw new Exception("El estado indicado no existe.");

            // 3. Actualizar
            usuario.EstadoId = nuevoEstadoId;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActualizarFotografiaAsync(string email, string fotoBase64)
        {
            // Validar peso máximo de 1MB
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
            // 1. Buscar al usuario por su Identificación (cédula), no por email
            // En un escenario real, aquí usarías .Include() de Entity Framework 
            // para traer también sus carreras o áreas e instituciones.
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Identificacion == identificacion);

            if (usuario == null)
                throw new Exception("Usuario no encontrado con esa identificación.");

            // 2. Construir el objeto anónimo con la información requerida para el carnet
            var datosCarnet = new
            {
                Nombre = usuario.NombreCompleto,
                Identificacion = usuario.Identificacion,
                // Aquí deberías mapear los nombres reales desde las tablas relacionadas
                Tipo = usuario.TipoUsuarioId == 1 ? "Estudiante" : "Funcionario",
                Institucion = "Colegio Universitario de Cartago",
                CarrerasAreas = "Programación / TI",
                Vencimiento = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd") // Ejemplo de regla de negocio
            };

            // 3. Convertir el objeto a formato JSON
            string jsonString = JsonSerializer.Serialize(datosCarnet);

            // 4. Generar el Código QR
            using QRCodeGenerator qrGenerator = new QRCodeGenerator();
            // ECCLevel.Q permite un buen nivel de corrección de errores (útil si las pantallas de celular están sucias o rotas)
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(jsonString, QRCodeGenerator.ECCLevel.Q);

            // Usamos PngByteQRCode para obtener directamente los bytes de la imagen sin depender de System.Drawing
            using PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

            // El parámetro 20 indica el tamaño de los píxeles del QR
            byte[] qrCodeImageBytes = qrCode.GetGraphic(20);

            // 5. Convertir a Base64 y retornar
            return Convert.ToBase64String(qrCodeImageBytes);
        }
    }
}
