using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models.Usuarios;
using CarnetDigital.Frontend.Services.Areas;
using CarnetDigital.Frontend.Services.Carreras;
using CarnetDigital.Frontend.Services.Instituciones;
using CarnetDigital.Frontend.Services.TiposIdentificacion;
using CarnetDigital.Frontend.Services.TiposUsuario;
using CarnetDigital.Frontend.Services.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioApiService _usuarioApiService;
        private readonly IInstitucionesApiService _institucionesApiService;
        private readonly ICarrerasApiService _carrerasApiService;
        private readonly IAreasApiService _areasApiService;
        private readonly ITiposUsuarioApiService _tiposUsuarioApiService;
        private readonly ITiposIdentificacionApiService _tiposIdentificacionApiService;

        public UsuarioController(
            IUsuarioApiService usuarioApiService,
            IInstitucionesApiService institucionesApiService,
            ICarrerasApiService carrerasApiService,
            IAreasApiService areasApiService,
            ITiposUsuarioApiService tiposUsuarioApiService,
            ITiposIdentificacionApiService tiposIdentificacionApiService)
        {
            _usuarioApiService = usuarioApiService;
            _institucionesApiService = institucionesApiService;
            _carrerasApiService = carrerasApiService;
            _areasApiService = areasApiService;
            _tiposUsuarioApiService = tiposUsuarioApiService;
            _tiposIdentificacionApiService = tiposIdentificacionApiService;
        }

        // WEB16: Listado y filtro de usuarios
        [HttpGet]
        public async Task<IActionResult> WEB16(string? identificacion, string? nombre, int? tipoUsuarioId)
        {
            var usuarios = await _usuarioApiService.FiltrarAsync(identificacion, nombre, tipoUsuarioId);

            ViewBag.FiltroIdentificacion = identificacion;
            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroTipo = tipoUsuarioId;

            return View(usuarios);
        }

        // WEB9: Detalle/perfil del usuario, con QR
        [HttpGet]
        public async Task<IActionResult> WEB9(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var usuario = await _usuarioApiService.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();

            await ResolverNombresAsync(usuario);

            // SOLUCIÓN PARA EL ROL: Mapeamos el ID a su texto descriptivo usando ViewBag
            // Nota: Ajusta los nombres ("Seguridad", "Estudiante") según la lógica real de tu sistema
            ViewBag.RolNombre = usuario.RolId switch
            {
                1 => "Administrador",
                2 => "Seguridad",
                3 => "Estudiante / Regular",
                _ => "Desconocido"
            };

            ViewBag.QrCode = await _usuarioApiService.ObtenerQRAsync(id);

            return View(usuario);
        }

        // WEB4: Cargar formulario de creación
        [HttpGet]
        public async Task<IActionResult> WEB4()
        {
            await CargarCatalogosViewBagAsync();
            return View();
        }

        // WEB4: Procesar formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WEB4(UsuarioDTO usuario, int? InstitucionTemp, int? CarreraTemp, int? AreaTemp)
        {
            usuario.InstitucionesIds = InstitucionTemp.HasValue ? new List<int> { InstitucionTemp.Value } : new List<int>();
            usuario.CarrerasIds = CarreraTemp.HasValue ? new List<int> { CarreraTemp.Value } : new List<int>();
            usuario.AreasIds = AreaTemp.HasValue ? new List<int> { AreaTemp.Value } : new List<int>();

            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto))
            {
                ViewBag.Error = "El nombre completo no puede estar vacío o contener solo espacios.";
                await CargarCatalogosViewBagAsync();
                return View(usuario);
            }

            var tiposUsuario = await _tiposUsuarioApiService.GetAllAsync();
            var nombreTipoUsuario = tiposUsuario.FirstOrDefault(t => t.Id == usuario.TipoUsuarioId)?.Nombre;
            bool esEstudiante = string.Equals(nombreTipoUsuario, "Estudiante", StringComparison.OrdinalIgnoreCase);

            try
            {
                var correoValidado = new System.Net.Mail.MailAddress(usuario.Email);
                string dominioCorreo = correoValidado.Host.ToLower();

                if (dominioCorreo != "cuc.cr" && dominioCorreo != "cuc.ac.cr")
                {
                    ViewBag.Error = "El correo debe pertenecer a los dominios institucionales @cuc.cr o @cuc.ac.cr.";
                    await CargarCatalogosViewBagAsync();
                    return View(usuario);
                }

                if (dominioCorreo == "cuc.cr" && !esEstudiante)
                {
                    ViewBag.Error = "Los correos con dominio @cuc.cr son exclusivos para estudiantes.";
                    await CargarCatalogosViewBagAsync();
                    return View(usuario);
                }

                if (dominioCorreo == "cuc.ac.cr" && esEstudiante)
                {
                    ViewBag.Error = "Los correos con dominio @cuc.ac.cr son exclusivos para funcionarios y administradores.";
                    await CargarCatalogosViewBagAsync();
                    return View(usuario);
                }
            }
            catch
            {
                ViewBag.Error = "El formato del correo electrónico no es válido.";
                await CargarCatalogosViewBagAsync();
                return View(usuario);
            }

            var resultado = await _usuarioApiService.CrearAsync(usuario);
            if (!resultado.Success)
            {
                ViewBag.Error = resultado.ErrorMessage ?? "No se pudo guardar el usuario en el sistema.";
                await CargarCatalogosViewBagAsync();
                return View(usuario);
            }

            TempData["Mensaje"] = "Usuario creado con éxito.";
            return RedirectToAction(nameof(WEB16));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var eliminado = await _usuarioApiService.EliminarAsync(id);
            TempData[eliminado ? "Mensaje" : "Error"] = eliminado
                ? "El usuario se eliminó correctamente."
                : "No se pudo eliminar el usuario seleccionado.";

            return RedirectToAction(nameof(WEB16));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var usuario = await _usuarioApiService.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(UsuarioDTO usuario)
        {
            var resultado = await _usuarioApiService.ActualizarAsync(usuario);
            if (resultado.Success)
            {
                TempData["Mensaje"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(WEB16));
            }

            ModelState.AddModelError("", resultado.ErrorMessage ?? "No se pudo actualizar la información del usuario.");
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(string identificacion, int estadoId)
        {
            if (string.IsNullOrEmpty(identificacion) || estadoId <= 0)
                return RedirectToAction(nameof(WEB16));

            var exito = await _usuarioApiService.CambiarEstadoAsync(identificacion, estadoId);

            TempData[exito ? "Mensaje" : "Error"] = exito
                ? $"El estado del usuario {identificacion} ha sido actualizado exitosamente."
                : $"No se pudo cambiar el estado del usuario {identificacion}.";

            return RedirectToAction(nameof(WEB16));
        }

        [HttpGet]
        public async Task<IActionResult> WEB8(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction(nameof(WEB16));

            var usuario = await _usuarioApiService.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();

            ViewBag.Identificacion = id;
            ViewBag.NombreCompleto = usuario.NombreCompleto;
            ViewBag.FotoBase64 = await _usuarioApiService.ObtenerFotografiaAsync(id);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarFotografiaWEB8(string identificacion, IFormFile nuevaFoto)
        {
            if (nuevaFoto == null || nuevaFoto.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo.";
                return RedirectToAction(nameof(WEB8), new { id = identificacion });
            }

            if (nuevaFoto.Length > 1048576)
            {
                TempData["Error"] = "La imagen supera el límite máximo de 1 MB permitido.";
                return RedirectToAction(nameof(WEB8), new { id = identificacion });
            }

            if (!nuevaFoto.ContentType.StartsWith("image/"))
            {
                TempData["Error"] = "El archivo debe ser una imagen (JPG, PNG, etc).";
                return RedirectToAction(nameof(WEB8), new { id = identificacion });
            }

            using var ms = new MemoryStream();
            await nuevaFoto.CopyToAsync(ms);
            var base64String = Convert.ToBase64String(ms.ToArray());

            var exito = await _usuarioApiService.ActualizarFotografiaAsync(identificacion, base64String);

            TempData[exito ? "Mensaje" : "Error"] = exito
                ? "Fotografía actualizada exitosamente."
                : "Ocurrió un error al guardar la fotografía.";

            return RedirectToAction(nameof(WEB8), new { id = identificacion });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarFotografiaWEB8(string identificacion)
        {
            var exito = await _usuarioApiService.EliminarFotografiaAsync(identificacion);

            TempData[exito ? "Mensaje" : "Error"] = exito
                ? "Fotografía eliminada exitosamente."
                : "Ocurrió un error al eliminar la fotografía.";

            return RedirectToAction(nameof(WEB8), new { id = identificacion });
        }

        private async Task CargarCatalogosViewBagAsync()
        {
            var tiposIdentificacion = await _tiposIdentificacionApiService.GetAllAsync();
            ViewBag.TiposIdentificacionList = tiposIdentificacion
                .Select(t => new SelectListItem(t.Nombre, t.Id.ToString()));

            var tiposUsuario = await _tiposUsuarioApiService.GetAllAsync();
            ViewBag.TiposUsuariosList = tiposUsuario
                .Select(t => new SelectListItem(t.Nombre, t.Id.ToString()));

            var instituciones = await _institucionesApiService.ObtenerActivasAsync();
            ViewBag.InstitucionesList = instituciones
                .Select(i => new SelectListItem(i.Nombre, i.Id.ToString()));

            var carreras = await _carrerasApiService.GetAllAsync();
            ViewBag.CarrerasList = carreras
                .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()));

            var areas = await _areasApiService.GetAllAsync();
            ViewBag.AreasList = areas
                .Select(a => new SelectListItem(a.Nombre, a.Id.ToString()));
        }

        private async Task ResolverNombresAsync(UsuarioDTO usuario)
        {
            // 1. Resolver Institución
            if (usuario.InstitucionesIds != null && usuario.InstitucionesIds.Count > 0)
            {
                var instituciones = await _institucionesApiService.ObtenerActivasAsync();
                usuario.Institucion = instituciones
                    .FirstOrDefault(i => i.Id == usuario.InstitucionesIds[0])?.Nombre;
            }

            // 2. Resolver Tipo de Usuario
            var tiposUsuario = await _tiposUsuarioApiService.GetAllAsync();
            var nombreTipoUsuario = tiposUsuario.FirstOrDefault(t => t.Id == usuario.TipoUsuarioId)?.Nombre;

            usuario.TipoUsuario = nombreTipoUsuario;

            // 3. Resolver Carrera o Área (Aquí está la corrección)
            if (string.Equals(nombreTipoUsuario, "Estudiante", StringComparison.OrdinalIgnoreCase)
                && usuario.CarrerasIds != null && usuario.CarrerasIds.Count > 0)
            {
                var carreras = await _carrerasApiService.GetAllAsync();
                usuario.CarreraOArea = carreras
                    .FirstOrDefault(c => c.Id == usuario.CarrerasIds[0])?.Nombre;
            }
            else if ((string.Equals(nombreTipoUsuario, "Funcionario", StringComparison.OrdinalIgnoreCase) ||
                      string.Equals(nombreTipoUsuario, "Profesor", StringComparison.OrdinalIgnoreCase))
                      && usuario.AreasIds != null && usuario.AreasIds.Count > 0)
            {
                // Si es Funcionario O Profesor, buscamos en el catálogo de Áreas
                var areas = await _areasApiService.GetAllAsync();
                usuario.CarreraOArea = areas
                    .FirstOrDefault(a => a.Id == usuario.AreasIds[0])?.Nombre;
            }
        }
    }
}
