using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.TiposUsuario;
using CarnetDigital.Frontend.Services.Areas;

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

        public async Task<TipoUsuarioDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/TiposUsuario/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<TipoUsuarioDto>();
        }

        public async Task<ApiResult<TipoUsuarioDto>> CreateAsync(string nombre)
        {
            var body = new TipoUsuarioRequestDto { Nombre = nombre, Activo = true };
            var response = await _httpClient.PostAsJsonAsync("/api/TiposUsuario", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<ApiResult<TipoUsuarioDto>> UpdateAsync(int id, string nombre)
        {
            var body = new TipoUsuarioRequestDto { Nombre = nombre, Activo = true };
            var response = await _httpClient.PutAsJsonAsync($"/api/TiposUsuario/{id}", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/TiposUsuario/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<ApiResult<TipoUsuarioDto>> LeerResultadoAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var tipo = await response.Content.ReadFromJsonAsync<TipoUsuarioDto>();
                return ApiResult<TipoUsuarioDto>.Ok(tipo!);
            }

            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            return ApiResult<TipoUsuarioDto>.Fail(error?.mensaje ?? "Ocurrió un error al procesar la solicitud");
        }

        private class TipoUsuarioRequestDto
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
