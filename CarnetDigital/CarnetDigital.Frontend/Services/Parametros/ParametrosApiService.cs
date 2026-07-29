using System.Net;
using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.Parametros;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Parametros
{
    public class ParametrosApiService : IParametrosApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ParametrosApiService> _logger;

        public ParametrosApiService(HttpClient httpClient, ILogger<ParametrosApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ParametroDto>> GetAllAsync()
        {
            try
            {
                var parametros = await _httpClient.GetFromJsonAsync<List<ParametroDto>>("/parametro");
                return parametros ?? new List<ParametroDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener parámetros");
                return new List<ParametroDto>();
            }
        }

        public async Task<ParametroDto?> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"/parametro/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ParametroDto>();
        }

        public async Task<ApiResult<ParametroDto>> CreateAsync(string id, string valor)
        {
            var body = new ParametroRequestDto { Id = id, Valor = valor };
            var response = await _httpClient.PostAsJsonAsync("/parametro", body);

            if (response.IsSuccessStatusCode)
            {
                var creado = await response.Content.ReadFromJsonAsync<ParametroDto>();
                return ApiResult<ParametroDto>.Ok(creado!);
            }

            return await LeerErrorAsync(response);
        }

        public async Task<ApiResult<ParametroDto>> UpdateAsync(string id, string valor)
        {
            var body = new ParametroRequestDto { Id = id, Valor = valor };
            var response = await _httpClient.PutAsJsonAsync($"/parametro/{id}", body);

            if (response.IsSuccessStatusCode)
                return ApiResult<ParametroDto>.Ok(new ParametroDto { Id = id, Valor = valor });

            return await LeerErrorAsync(response);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"/parametro/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<ApiResult<ParametroDto>> LeerErrorAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
                return ApiResult<ParametroDto>.Fail("Ya existe un parámetro con ese código.");

            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            return ApiResult<ParametroDto>.Fail(error?.Message ?? "Ocurrió un error al procesar la solicitud");
        }

        private class ParametroRequestDto
        {
            public string Id { get; set; } = string.Empty;
            public string Valor { get; set; } = string.Empty;
        }

        private class ErrorResponseDto
        {
            public string? Message { get; set; }
        }
    }
}
