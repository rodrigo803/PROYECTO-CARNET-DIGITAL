using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.Instituciones;

namespace CarnetDigital.Frontend.Services.Instituciones
{
    public class InstitucionesApiService : IInstitucionesApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InstitucionesApiService> _logger;

        public InstitucionesApiService(HttpClient httpClient, ILogger<InstitucionesApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<InstitucionDto>> ObtenerActivasAsync()
        {
            try
            {
                // GET /api/Instituciones ya devuelve solo las activas (filtrado en el propio microservicio).
                var instituciones = await _httpClient.GetFromJsonAsync<List<InstitucionDto>>("/api/Instituciones");
                return instituciones ?? new List<InstitucionDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener instituciones activas");
                return new List<InstitucionDto>();
            }
        }
    }
}
