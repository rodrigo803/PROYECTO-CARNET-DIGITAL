using System.Net.Http.Json;
using System.Text.Json;
using CarnetDigital.Frontend.Models.Auth;

namespace CarnetDigital.Frontend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResult> LoginAsync(LoginViewModel model)
        {
            var body = new
            {
                usuario = model.Username,
                contrasena = model.Password,
                tipousuario = model.TipoUsuario
            };

            var response = await _httpClient.PostAsJsonAsync("/login", body);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                string? mensaje = null;

                try
                {
                    var error = JsonSerializer.Deserialize<AuthErrorResponse>(json, JsonOptions);
                    mensaje = error?.Mensaje;
                }
                catch (JsonException)
                {
                }

                return new LoginResult { Success = false, Mensaje = mensaje };
            }

            var data = JsonSerializer.Deserialize<LoginResponse>(json, JsonOptions);

            return new LoginResult { Success = true, Data = data };
        }
    }
}
