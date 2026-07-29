using System.Net;
using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.Pantallas;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Pantallas
{
    public class PantallasApiService : IPantallasApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PantallasApiService> _logger;

        public PantallasApiService(HttpClient httpClient, ILogger<PantallasApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<PantallaDto>> GetAllAsync()
        {
            try
            {
                var pantallas = await _httpClient.GetFromJsonAsync<List<PantallaDto>>("/pantallas");
                return pantallas ?? new List<PantallaDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener pantallas");
                return new List<PantallaDto>();
            }
        }

        public async Task<PantallaDto?> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"/pantallas/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PantallaDto>();
        }

        public async Task<ApiResult<PantallaDto>> CreateAsync(PantallaDto pantalla)
        {
            var response = await _httpClient.PostAsJsonAsync("/pantallas", pantalla);

            if (response.IsSuccessStatusCode)
            {
                var creada = await response.Content.ReadFromJsonAsync<PantallaDto>();
                return ApiResult<PantallaDto>.Ok(creada!);
            }

            return await LeerErrorAsync(response);
        }

        public async Task<ApiResult<PantallaDto>> UpdateAsync(string id, PantallaDto pantalla)
        {
            var response = await _httpClient.PutAsJsonAsync($"/pantallas/{id}", pantalla);

            if (response.IsSuccessStatusCode)
                return ApiResult<PantallaDto>.Ok(pantalla);

            return await LeerErrorAsync(response);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"/pantallas/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<ApiResult<PantallaDto>> LeerErrorAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
                return ApiResult<PantallaDto>.Fail("Ya existe una pantalla con ese Id.");

            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            if (error?.Errores is { Count: > 0 })
                return ApiResult<PantallaDto>.Fail(string.Join(" ", error.Errores));

            return ApiResult<PantallaDto>.Fail(error?.Message ?? "Ocurrió un error al procesar la solicitud");
        }

        private class ErrorResponseDto
        {
            public string? Message { get; set; }
            public List<string>? Errores { get; set; }
        }
    }
}
