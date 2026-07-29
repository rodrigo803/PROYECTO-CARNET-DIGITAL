using System.Net;
using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.Roles;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Roles
{
    public class RolesApiService : IRolesApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RolesApiService> _logger;

        public RolesApiService(HttpClient httpClient, ILogger<RolesApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<RolDto>> GetAllAsync()
        {
            try
            {
                var roles = await _httpClient.GetFromJsonAsync<List<RolDto>>("/rol");
                return roles ?? new List<RolDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener roles");
                return new List<RolDto>();
            }
        }

        public async Task<RolDto?> GetByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"/rol/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<RolDto>();
        }

        public async Task<ApiResult<RolDto>> CreateAsync(RolDto rol)
        {
            var response = await _httpClient.PostAsJsonAsync("/rol", rol);

            if (response.IsSuccessStatusCode)
            {
                var creado = await response.Content.ReadFromJsonAsync<RolDto>();
                return ApiResult<RolDto>.Ok(creado!);
            }

            return await LeerErrorAsync(response);
        }

        public async Task<ApiResult<RolDto>> UpdateAsync(string id, RolDto rol)
        {
            var response = await _httpClient.PutAsJsonAsync($"/rol/{id}", rol);

            if (response.IsSuccessStatusCode)
                return ApiResult<RolDto>.Ok(rol);

            return await LeerErrorAsync(response);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"/rol/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<ApiResult<RolDto>> LeerErrorAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
                return ApiResult<RolDto>.Fail("Ya existe un rol con ese Id.");

            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            if (error?.Errores is { Count: > 0 })
                return ApiResult<RolDto>.Fail(string.Join(" ", error.Errores));

            return ApiResult<RolDto>.Fail(error?.Message ?? "Ocurrió un error al procesar la solicitud");
        }

        private class ErrorResponseDto
        {
            public string? Message { get; set; }
            public List<string>? Errores { get; set; }
        }
    }
}
