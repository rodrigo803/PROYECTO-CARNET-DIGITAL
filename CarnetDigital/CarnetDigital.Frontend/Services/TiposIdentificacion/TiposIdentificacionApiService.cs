using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.TiposIdentificacion;
using CarnetDigital.Frontend.Services.Areas;

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

        public async Task<TipoIdentificacionDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/TiposIdentificacion/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<TipoIdentificacionDto>();
        }

        public async Task<ApiResult<TipoIdentificacionDto>> CreateAsync(string nombre)
        {
            var body = new TipoIdentificacionRequestDto { Nombre = nombre, Activo = true };
            var response = await _httpClient.PostAsJsonAsync("/api/TiposIdentificacion", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<ApiResult<TipoIdentificacionDto>> UpdateAsync(int id, string nombre)
        {
            var body = new TipoIdentificacionRequestDto { Nombre = nombre, Activo = true };
            var response = await _httpClient.PutAsJsonAsync($"/api/TiposIdentificacion/{id}", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/TiposIdentificacion/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<ApiResult<TipoIdentificacionDto>> LeerResultadoAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var tipo = await response.Content.ReadFromJsonAsync<TipoIdentificacionDto>();
                return ApiResult<TipoIdentificacionDto>.Ok(tipo!);
            }

            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            return ApiResult<TipoIdentificacionDto>.Fail(error?.mensaje ?? "Ocurrió un error al procesar la solicitud");
        }

        private class TipoIdentificacionRequestDto
        {
            public string Nombre { get; set; } = string.Empty;
            public bool Activo { get; set; }
        }

        private class ErrorResponseDto
        {
            public string? mensaje { get; set; }
        }
    }
}
