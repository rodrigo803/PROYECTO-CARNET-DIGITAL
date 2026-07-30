using CarnetDigital.Frontend.Helpers;
using CarnetDigital.Frontend.Models.Auth;
using CarnetDigital.Frontend.Models.Usuarios;
using CarnetDigital.Frontend.Services.Areas;
using CarnetDigital.Frontend.Services.Auth;
using CarnetDigital.Frontend.Services.Carreras;
using CarnetDigital.Frontend.Services.Instituciones;
using CarnetDigital.Frontend.Services.Usuarios;
using Microsoft.AspNetCore.Mvc;

namespace CarnetDigital.Frontend.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUsuarioApiService _usuarioApiService;
    private readonly IInstitucionesApiService _institucionesApiService;
    private readonly ICarrerasApiService _carrerasApiService;
    private readonly IAreasApiService _areasApiService;

    public AuthController(
        IAuthService authService,
        ITokenProvider tokenProvider,
        IUsuarioApiService usuarioApiService,
        IInstitucionesApiService institucionesApiService,
        ICarrerasApiService carrerasApiService,
        IAreasApiService areasApiService)
    {
        _authService = authService;
        _tokenProvider = tokenProvider;
        _usuarioApiService = usuarioApiService;
        _institucionesApiService = institucionesApiService;
        _carrerasApiService = carrerasApiService;
        _areasApiService = areasApiService;
    }

    [HttpGet]
    public IActionResult Login(string? message = null)
    {
        ViewBag.Message = message;
        ViewBag.MensajeExito = TempData["MensajeExito"];
        return View();
    }

    public IActionResult Logout()
    {
        _tokenProvider.ClearToken();

        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (LoginAttemptHelper.IsBlocked(model.Username))
        {
            ViewBag.Error = "Su usuario ha sido bloqueado.";

            return View(model);
        }

        var result = await _authService.LoginAsync(model);

        if (!result.Success || result.Data == null)
        {
            LoginAttemptHelper.RegisterFailedAttempt(model.Username);

            if (LoginAttemptHelper.IsBlocked(model.Username))
            {
                ViewBag.Error =
                    "Su usuario ha sido bloqueado después de 3 intentos fallidos.";
            }
            else
            {
                ViewBag.Error = result.Mensaje ?? "Usuario y/o contraseña incorrectos.";
            }

            return View(model);
        }

        _tokenProvider.SetToken(result.Data.Access_Token);
        _tokenProvider.SetUsername(model.Username);

        LoginAttemptHelper.ResetAttempts(model.Username);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Autoregistro()
    {
        await CargarCatalogosAutoregistroAsync();
        return View(new UsuarioDTO());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Autoregistro(UsuarioDTO usuario)
    {
        // Se fuerza el RolId a 2 (Usuario Regular) por seguridad, evitando escalada de privilegios
        usuario.RolId = 2;

        var exito = await _usuarioApiService.AutoregistroAsync(usuario);

        if (exito)
        {
            TempData["MensajeExito"] = "¡Registro exitoso! Por favor revisa tu correo electrónico para confirmar tu cuenta antes de 15 minutos.";
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError("", "No se pudo completar el registro. Verifica los datos ingresados.");
        await CargarCatalogosAutoregistroAsync();
        return View(usuario);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmarCorreo(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            ViewBag.Mensaje = "El token de confirmación no es válido.";
            ViewBag.Exito = false;
            return View();
        }

        var confirmado = await _usuarioApiService.ConfirmarRegistroAsync(token);

        if (confirmado)
        {
            ViewBag.Mensaje = "¡Tu cuenta ha sido confirmada exitosamente! Ya puedes iniciar sesión en el sistema.";
            ViewBag.Exito = true;
        }
        else
        {
            ViewBag.Mensaje = "El enlace de confirmación es inválido o ya ha expirado (recuerda que expira en 15 minutos).";
            ViewBag.Exito = false;
        }

        return View();
    }

    private async Task CargarCatalogosAutoregistroAsync()
    {
        ViewBag.Instituciones = await _institucionesApiService.ObtenerActivasAsync();
        ViewBag.Carreras = await _carrerasApiService.GetAllAsync();
        ViewBag.Areas = await _areasApiService.GetAllAsync();
    }
}
