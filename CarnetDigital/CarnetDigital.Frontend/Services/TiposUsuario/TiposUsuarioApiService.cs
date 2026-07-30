using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.TiposUsuario;

namespace CarnetDigital.Frontend.Services.TiposUsuario
{
    public class TiposUsuarioApiService : ITiposUsuarioApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TiposUsuarioApiService> _logger;

        public TiposUsuarioApiService(HttpClient httpClient, ILogger<TiposUsuarioApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<TipoUsuarioDto>> GetAllAsync()
        {
            try
            {
                var tipos = await _httpClient.GetFromJsonAsync<List<TipoUsuarioDto>>("/api/TiposUsuario");
                return tipos ?? new List<TipoUsuarioDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener tipos de usuario");
                return new List<TipoUsuarioDto>();
            }
        }
    }
}
