using Microservicio.Carreras.DTOs;
using Microservicio.Carreras.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Microservicio.Carreras.Services
{
    public class InstitucionesValidator : IInstitucionesValidator
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InstitucionesValidator> _logger;

        public InstitucionesValidator(HttpClient httpClient, ILogger<InstitucionesValidator> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<InstitucionInfo?> ObtenerInstitucionAsync(int idInstitucion, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"/api/Instituciones/{idInstitucion}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<InstitucionInfo>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar institución {Id}", idInstitucion);
                return null;
            }
        }
    }
}