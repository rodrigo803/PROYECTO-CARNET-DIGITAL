using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Microservicio.Usuario.Services
{
    public interface IBitacoraService
    {
        Task RegistrarAccionAsync(int usuarioId, string descripcion);
    }

    public class BitacoraApiClient : IBitacoraService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Inyectamos el IHttpContextAccessor para poder leer la petición original
        public BitacoraApiClient(HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            string auditUrl = config["Urls:AuditService"];
            if (!string.IsNullOrEmpty(auditUrl))
            {
                _httpClient.BaseAddress = new Uri(auditUrl);
            }
        }

        public async Task RegistrarAccionAsync(int usuarioId, string descripcion)
        {
            try
            {
                var payload = new { UsuarioId = usuarioId, Descripcion = descripcion };

                // Creamos la petición específicamente para este log
                using var request = new HttpRequestMessage(HttpMethod.Post, "");
                request.Content = JsonContent.Create(payload);

                // Pasamos el token solo a ESTA petición, sin afectar al HttpClient global
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader))
                {
                    request.Headers.Add("Authorization", authHeader);
                }

                // Enviamos
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    // Útil para debuguear si el AuditService te está rechazando
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"AuditService respondió con error: {response.StatusCode} - {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fallo de comunicación con AuditService: {ex.Message}");
            }
        }
    }
}