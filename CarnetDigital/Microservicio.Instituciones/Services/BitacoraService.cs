using Microservicio.Instituciones.DTOs;
using Microservicio.Instituciones.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Microservicio.Instituciones.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BitacoraService> _logger;

        public BitacoraService(HttpClient httpClient, ILogger<BitacoraService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task RegistrarAsync(int usuarioId, string descripcion, string token)
        {
            try
            {
                var request = new BitacoraRequest
                {
                    UsuarioId = usuarioId,
                    Descripcion = descripcion
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsync("/bitacora", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning(
                        "Error al registrar bitácora. Status: {Status}, Mensaje: {Mensaje}",
                        response.StatusCode, errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al registrar bitácora");
            }
        }
    }
}