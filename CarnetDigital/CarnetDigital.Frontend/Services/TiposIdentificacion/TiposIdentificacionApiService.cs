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
            var contenido = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrWhiteSpace(contenido))
                    return ApiResult<TipoIdentificacionDto>.Fail("Error al procesar la solicitud");

                try
                {
                    var tipo = System.Text.Json.JsonSerializer.Deserialize<TipoIdentificacionDto>(contenido,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return ApiResult<TipoIdentificacionDto>.Ok(tipo!);
                }
                catch (System.Text.Json.JsonException)
                {
                    return ApiResult<TipoIdentificacionDto>.Fail($"Error al procesar la solicitud (status {(int)response.StatusCode})");
                }
            }

            if (string.IsNullOrWhiteSpace(contenido))
                return ApiResult<TipoIdentificacionDto>.Fail($"Error al procesar la solicitud (status {(int)response.StatusCode})");

            try
            {
                var error = System.Text.Json.JsonSerializer.Deserialize<ErrorResponseDto>(contenido,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return ApiResult<TipoIdentificacionDto>.Fail(error?.mensaje ?? "Ocurrió un error al procesar la solicitud");
            }
            catch (System.Text.Json.JsonException)
            {
                return ApiResult<TipoIdentificacionDto>.Fail($"Error al procesar la solicitud (status {(int)response.StatusCode})");
            }
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
