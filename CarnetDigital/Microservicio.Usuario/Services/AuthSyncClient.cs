using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Microservicio.Usuario.Services
{
    public interface IAuthSyncClient
    {
        Task SincronizarCuentaAsync(string email, string passwordHash, string userType);
    }

    /// <summary>
    /// Mantiene sincronizada la cuenta de acceso en AuthService.UsersAuth cuando un usuario
    /// se confirma en Usuarios. Reusa el hash BCrypt ya calculado, sin resetear contraseñas.
    /// </summary>
    public class AuthSyncClient : IAuthSyncClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AuthSyncClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task SincronizarCuentaAsync(string email, string passwordHash, string userType)
        {
            string baseUrl = _config["Microservicios:AuthService"];
            if (string.IsNullOrEmpty(baseUrl)) return;

            try
            {
                await _httpClient.PostAsJsonAsync($"{baseUrl}/registro-interno", new
                {
                    Email = email,
                    PasswordHash = passwordHash,
                    UserType = userType
                });
            }
            catch (Exception ex)
            {
                // No debe tumbar la confirmación del registro si AuthService está caído.
                Console.WriteLine($"No se pudo sincronizar la cuenta de acceso para {email}: {ex.Message}");
            }
        }
    }
}
