// TEMPORAL - reemplazar con Web1
using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.TempAuth;
using CarnetDigital.Frontend.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CarnetDigital.Frontend.Controllers
{
    public class TempAuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public TempAuthController(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_tokenProvider.IsAuthenticated())
                return RedirectToAction("Index", "Areas");

            return View(new LoginFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginFormViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);

            var client = _httpClientFactory.CreateClient("AuthServiceClient");

            var request = new LoginRequestDto
            {
                usuario = form.Usuario,
                contrasena = form.Contrasena,
                tipousuario = form.TipoUsuario
            };

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsJsonAsync("/login", request);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "No se pudo contactar el servicio de autenticación. Intente de nuevo más tarde.";
                return View(form);
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Usuario y/o contraseña incorrectos";
                return View(form);
            }

            var token = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
            if (token is null || string.IsNullOrWhiteSpace(token.access_token))
            {
                TempData["Error"] = "No se pudo iniciar sesión. Intente de nuevo.";
                return View(form);
            }

            _tokenProvider.SetToken(token.access_token);
            TempData["Success"] = "Sesión iniciada correctamente.";
            return RedirectToAction("Index", "Areas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _tokenProvider.ClearToken();
            return RedirectToAction("Login");
        }
    }
}
