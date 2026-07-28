using CarnetDigital.Frontend.Helpers;
using CarnetDigital.Frontend.Models.Auth;
using CarnetDigital.Frontend.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CarnetDigital.Frontend.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenProvider _tokenProvider;

    public AuthController(IAuthService authService, ITokenProvider tokenProvider)
    {
        _authService = authService;
        _tokenProvider = tokenProvider;
    }

    [HttpGet]
    public IActionResult Login(string? message = null)
    {
        ViewBag.Message = message;
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
}
