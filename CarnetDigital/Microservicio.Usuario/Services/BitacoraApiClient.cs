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
                // 1. "Robamos" el token JWT de la petición original (la de Postman)
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                // 2. Si hay token, se lo pegamos a nuestro Postman interno
                if (!string.IsNullOrEmpty(authHeader))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);
                }

                var payload = new
                {
                    UsuarioId = usuarioId,
                    Descripcion = descripcion
                };

                // 3. Enviamos la petición ya autorizada
                await _httpClient.PostAsJsonAsync("", payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fallo de comunicación con AuditService: {ex.Message}");
            }
        }
    }
}