using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.TiposIdentificacion;

namespace CarnetDigital.Frontend.Services.TiposIdentificacion
{
    public class TiposIdentificacionApiService : ITiposIdentificacionApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TiposIdentificacionApiService> _logger;

        public TiposIdentificacionApiService(HttpClient httpClient, ILogger<TiposIdentificacionApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<TipoIdentificacionDto>> GetAllAsync()
        {
            try
            {
                var tipos = await _httpClient.GetFromJsonAsync<List<TipoIdentificacionDto>>("/api/TiposIdentificacion");
                return tipos ?? new List<TipoIdentificacionDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener tipos de identificación");
                return new List<TipoIdentificacionDto>();
            }
        }
    }
}
