using BCrypt.Net;
using Microservicio.Usuario.Entities;
using Microservicio.Usuario.Repository;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthSyncClient _authSync;

        public UsuarioService(IServiceScopeFactory scopeFactory, IBitacoraService bitacora, IConfiguration config, IHttpContextAccessor httpContextAccessor, IAuthSyncClient authSync)
        {
            _scopeFactory = scopeFactory;
            _bitacora = bitacora;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _authSync = authSync;
        }

        // Propaga el Bearer de la request entrante hacia los microservicios de catálogo,
        // igual que InstitucionesValidator en Areas/Carreras. En /autoregistro (anónimo)
        // no habrá token disponible.
        private string? ObtenerTokenActual()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            return authHeader.Substring("Bearer ".Length).Trim();
        }

        public async Task<UsuarioActualizacionDto> CrearUsuarioAsync(Entities.Usuario usuario, string contrasenaPlana)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _catalogos = scope.ServiceProvider.GetRequiredService<ICatalogosApiClient>(); // Inyectamos cliente

            // 1. REGLAS DE NEGOCIO DEL CARNET DIGITAL
            if (usuario.InstitucionesIds == null || usuario.InstitucionesIds.Count == 0)
                throw new Exception("El usuario debe pertenecer a al menos una institución.");

            var token = ObtenerTokenActual();
            string nombreTipoUsuario = await _catalogos.ObtenerNombreTipoUsuarioAsync(usuario.TipoUsuarioId, token);

            if (string.Equals(nombreTipoUsuario, "Estudiante", StringComparison.OrdinalIgnoreCase)
                && (usuario.CarrerasIds == null || usuario.CarrerasIds.Count == 0))
                throw new Exception("Si el usuario es estudiante, debe tener carreras asociadas.");

            if (string.Equals(nombreTipoUsuario, "Funcionario", StringComparison.OrdinalIgnoreCase)
                && (usuario.AreasIds == null || usuario.AreasIds.Count == 0))
                throw new Exception("Si el usuario es funcionario, debe tener áreas asociadas.");


            // 2. VALIDAR CONTRA LOS MICROSERVICIOS
            await _catalogos.ValidarCatalogosAsync(usuario.TipoIdentificacionId, usuario.TipoUsuarioId, usuario.InstitucionesIds, usuario.CarrerasIds, usuario.AreasIds, token);

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

            var token = ObtenerTokenActual();
            string nombreTipoUsuario = await _catalogos.ObtenerNombreTipoUsuarioAsync(dto.TipoUsuarioId, token);

            if (string.Equals(nombreTipoUsuario, "Estudiante", StringComparison.OrdinalIgnoreCase)
                && (dto.CarrerasIds == null || dto.CarrerasIds.Count == 0))
                throw new Exception("Si el usuario es estudiante, debe tener carreras asociadas.");

            if (string.Equals(nombreTipoUsuario, "Funcionario", StringComparison.OrdinalIgnoreCase)
                && (dto.AreasIds == null || dto.AreasIds.Count == 0))
                throw new Exception("Si el usuario es funcionario, debe tener áreas asociadas.");

            // --- ESTO TE DIRÁ LA VERDAD ---
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"DEBUG: Instituciones recibidas: {(dto.InstitucionesIds != null ? dto.InstitucionesIds.Count : "NULL")}");
            Console.WriteLine($"DEBUG: Carreras recibidas: {(dto.CarrerasIds != null ? dto.CarrerasIds.Count : "NULL")}");
            Console.WriteLine($"DEBUG: JSON completo: {JsonSerializer.Serialize(dto)}");
            Console.WriteLine("------------------------------------------");

            // 2. VALIDAR CONTRA LOS MICROSERVICIOS
            await _catalogos.ValidarCatalogosAsync(dto.TipoIdentificacionId, dto.TipoUsuarioId, dto.InstitucionesIds, dto.CarrerasIds, dto.AreasIds, token);

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
                // 1. Leemos la URL del frontend desde el appsettings.json
                // Si no la encuentra, usa un localhost por defecto (debes poner el puerto real de tu frontend aquí)
                string urlBaseFrontend = _config["Urls:Frontend"] ?? "https://localhost:7216";

                // 2. Armamos el enlace apuntando a la VISTA del frontend, no a la API
                // NOTA: Cambia "/confirmar-registro" por la ruta real de la página en tu frontend
                string enlaceConfirmacion = $"{urlBaseFrontend}/Auth/ConfirmarCorreo?token={token}";

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
            var _catalogos = scope.ServiceProvider.GetRequiredService<ICatalogosApiClient>();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenConfirmacion == token);

            if (usuario == null) throw new Exception("Token inválido o usuario no encontrado.");

            if (DateTime.Now > usuario.FechaExpiracionToken)
                throw new Exception("El token ha expirado. Han pasado más de 15 minutos.");

            usuario.EstadoId = 1;
            usuario.TokenConfirmacion = null;
            usuario.FechaExpiracionToken = null;

            await _context.SaveChangesAsync();

            // Sincroniza la cuenta de acceso en AuthService ahora que el usuario queda Activo,
            // reusando el mismo hash BCrypt que ya se calculó al crear/autoregistrar el usuario.
            string nombreTipoUsuario = await _catalogos.ObtenerNombreTipoUsuarioAsync(usuario.TipoUsuarioId, null);
            await _authSync.SincronizarCuentaAsync(usuario.Email, usuario.ContrasenaEncriptada, UserTypeMapper.ToUserType(nombreTipoUsuario));

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
            var token = ObtenerTokenActual();
            string nombreTipo = await _catalogos.ObtenerNombreTipoUsuarioAsync(usuario.TipoUsuarioId, token);
            string nombresInstituciones = await _catalogos.ObtenerNombresInstitucionesAsync(usuario.InstitucionesIds, token);
            string nombresCarrerasAreas = await _catalogos.ObtenerNombresCarrerasOAreasAsync(usuario.TipoUsuarioId, usuario.CarrerasIds, usuario.AreasIds, token);

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

        public async Task<IEnumerable<UsuarioResumenDto>> ObtenerTodosAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await _context.Usuarios
                .Select(u => MapearResumen(u))
                .ToListAsync();
        }

        public async Task<UsuarioResumenDto?> ObtenerPorIdAsync(string identificacion)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _catalogos = scope.ServiceProvider.GetRequiredService<ICatalogosApiClient>();

            var usuario = await _context.Usuarios.FindAsync(identificacion);
            if (usuario == null) return null;

            var resumen = MapearResumen(usuario);

            // TipoUsuario es [NotMapped] en la entidad (no existe como columna en BD), así
            // que MapearResumen siempre lo deja en null. Lo resolvemos del catálogo aquí,
            // igual que ya hacen GenerarQRBase64Async y ObtenerPerfilPorEmailAsync, para que
            // coincida con el "Tipo" que trae el QR generado para este mismo usuario.
            var token = ObtenerTokenActual();
            string nombreTipoUsuario = await _catalogos.ObtenerNombreTipoUsuarioAsync(usuario.TipoUsuarioId, token);
            resumen.TipoUsuario = nombreTipoUsuario == "Desconocido" ? usuario.TipoUsuarioId.ToString() : nombreTipoUsuario;

            return resumen;
        }

        public async Task<PerfilUsuarioDto?> ObtenerPerfilPorEmailAsync(string email)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _catalogos = scope.ServiceProvider.GetRequiredService<ICatalogosApiClient>();

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null) return null;

            var token = ObtenerTokenActual();

            // Resuelve los nombres desde los catálogos; si alguno no responde, cae al ID como texto
            // en vez de romper el endpoint.
            string nombreTipoUsuario = await _catalogos.ObtenerNombreTipoUsuarioAsync(usuario.TipoUsuarioId, token);
            string nombreTipoIdentificacion = await _catalogos.ObtenerNombreTipoIdentificacionAsync(usuario.TipoIdentificacionId, token);
            string carreraOArea = await _catalogos.ObtenerNombresCarrerasOAreasAsync(usuario.TipoUsuarioId, usuario.CarrerasIds, usuario.AreasIds, token);

            return new PerfilUsuarioDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto,
                TipoIdentificacionId = usuario.TipoIdentificacionId,
                TipoIdentificacion = nombreTipoIdentificacion == "Desconocido" ? usuario.TipoIdentificacionId.ToString() : nombreTipoIdentificacion,
                TipoUsuarioId = usuario.TipoUsuarioId,
                TipoUsuario = nombreTipoUsuario == "Desconocido" ? usuario.TipoUsuarioId.ToString() : nombreTipoUsuario,
                InstitucionesIds = usuario.InstitucionesIds,
                CarrerasIds = usuario.CarrerasIds,
                AreasIds = usuario.AreasIds,
                CarreraOArea = carreraOArea,
                TieneFotografia = !string.IsNullOrEmpty(usuario.FotografiaBase64)
            };
        }

        public async Task<IEnumerable<UsuarioResumenDto>> FiltrarAsync(string? identificacion, string? nombre, int? tipoUsuarioId)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(identificacion))
                query = query.Where(u => u.Identificacion.Contains(identificacion));

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(u => u.NombreCompleto.Contains(nombre));

            if (tipoUsuarioId.HasValue)
                query = query.Where(u => u.TipoUsuarioId == tipoUsuarioId.Value);

            var usuarios = await query.ToListAsync();
            return usuarios.Select(MapearResumen);
        }

        public async Task<FotografiaDto?> ObtenerFotografiaAsync(string identificacion)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = await _context.Usuarios.FindAsync(identificacion);
            if (usuario == null) return null;

            return new FotografiaDto
            {
                Identificacion = usuario.Identificacion,
                FotoBase64 = usuario.FotografiaBase64
            };
        }

        public async Task<UsuarioActualizacionDto?> EliminarFotografiaAsync(string identificacion)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var usuario = await _context.Usuarios.FindAsync(identificacion);
            if (usuario == null) return null;

            usuario.FotografiaBase64 = "";
            await _context.SaveChangesAsync();

            int cedulaInt = int.TryParse(usuario.Identificacion, out int result) ? result : 0;
            await _bitacora.RegistrarAccionAsync(cedulaInt, $"El usuario {usuario.Identificacion} eliminó su fotografía del carnet.");

            return new UsuarioActualizacionDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto
            };
        }

        private static UsuarioResumenDto MapearResumen(Entities.Usuario usuario)
        {
            return new UsuarioResumenDto
            {
                Identificacion = usuario.Identificacion,
                Email = usuario.Email,
                NombreCompleto = usuario.NombreCompleto,
                EstadoId = usuario.EstadoId,
                TipoIdentificacionId = usuario.TipoIdentificacionId,
                TipoUsuarioId = usuario.TipoUsuarioId,
                RolId = usuario.RolId,
                TipoIdentificacion = usuario.TipoIdentificacion,
                TipoUsuario = usuario.TipoUsuario,
                InstitucionesIds = usuario.InstitucionesIds,
                CarrerasIds = usuario.CarrerasIds,
                AreasIds = usuario.AreasIds
            };
        }
    }
}