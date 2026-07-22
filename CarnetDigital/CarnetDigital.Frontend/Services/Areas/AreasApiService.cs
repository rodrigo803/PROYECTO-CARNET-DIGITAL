using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.Areas;

namespace CarnetDigital.Frontend.Services.Areas
{
    public class AreasApiService : IAreasApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AreasApiService> _logger;

        public AreasApiService(HttpClient httpClient, ILogger<AreasApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<AreaDto>> GetAllAsync()
        {
            try
            {
                var areas = await _httpClient.GetFromJsonAsync<List<AreaDto>>("/api/Areas");
                return areas ?? new List<AreaDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener áreas de trabajo");
                return new List<AreaDto>();
            }
        }

        public async Task<AreaDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Areas/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AreaDto>();
        }

        public async Task<ApiResult<AreaDto>> CreateAsync(string nombre, int idInstitucion)
        {
            var body = new AreaRequestDto { Nombre = nombre, IdInstitucion = idInstitucion, Activo = true };
            var response = await _httpClient.PostAsJsonAsync("/api/Areas", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<ApiResult<AreaDto>> UpdateAsync(int id, string nombre, int idInstitucion)
        {
            var body = new AreaRequestDto { Nombre = nombre, IdInstitucion = idInstitucion, Activo = true };
            var response = await _httpClient.PutAsJsonAsync($"/api/Areas/{id}", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Areas/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<ApiResult<AreaDto>> LeerResultadoAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var area = await response.Content.ReadFromJsonAsync<AreaDto>();
                return ApiResult<AreaDto>.Ok(area!);
            }

            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            return ApiResult<AreaDto>.Fail(error?.mensaje ?? "Ocurrió un error al procesar la solicitud");
        }

        private class AreaRequestDto
        {
            public string Nombre { get; set; } = string.Empty;
            public int IdInstitucion { get; set; }
            public bool Activo { get; set; }
        }

        private class ErrorResponseDto
        {
            public string? mensaje { get; set; }
        }
    }
}
