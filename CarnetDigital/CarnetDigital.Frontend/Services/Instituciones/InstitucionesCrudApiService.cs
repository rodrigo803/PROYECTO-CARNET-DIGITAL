using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.Instituciones;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Instituciones
{
    public class InstitucionesCrudApiService : IInstitucionesCrudApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InstitucionesCrudApiService> _logger;

        public InstitucionesCrudApiService(HttpClient httpClient, ILogger<InstitucionesCrudApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<InstitucionDetalleDto>> GetAllAsync()
        {
            try
            {
                var instituciones = await _httpClient.GetFromJsonAsync<List<InstitucionDetalleDto>>("/api/Instituciones");
                return instituciones ?? new List<InstitucionDetalleDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener instituciones");
                return new List<InstitucionDetalleDto>();
            }
        }

        public async Task<InstitucionDetalleDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Instituciones/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<InstitucionDetalleDto>();
        }

        public async Task<ApiResult<InstitucionDetalleDto>> CreateAsync(string nombre, string email, string telefono, List<string> dominios)
        {
            var body = new InstitucionRequestDto
            {
                Nombre = nombre,
                Email = email,
                Telefono = telefono,
                Activo = true,
                Dominios = dominios.Select(d => new DominioRequestDto { Dominio = d }).ToList()
            };
            var response = await _httpClient.PostAsJsonAsync("/api/Instituciones", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<ApiResult<InstitucionDetalleDto>> UpdateAsync(int id, string nombre, string email, string telefono, List<string> dominios)
        {
            var body = new InstitucionRequestDto
            {
                Nombre = nombre,
                Email = email,
                Telefono = telefono,
                Activo = true,
                Dominios = dominios.Select(d => new DominioRequestDto { Dominio = d }).ToList()
            };
            var response = await _httpClient.PutAsJsonAsync($"/api/Instituciones/{id}", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Instituciones/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<ApiResult<InstitucionDetalleDto>> LeerResultadoAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var institucion = await response.Content.ReadFromJsonAsync<InstitucionDetalleDto>();
                return ApiResult<InstitucionDetalleDto>.Ok(institucion!);
            }

            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            return ApiResult<InstitucionDetalleDto>.Fail(error?.mensaje ?? "Ocurrió un error al procesar la solicitud");
        }

        private class InstitucionRequestDto
        {
            public string Nombre { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Telefono { get; set; } = string.Empty;
            public bool Activo { get; set; }
            public List<DominioRequestDto> Dominios { get; set; } = new();
        }

        private class DominioRequestDto
        {
            public string Dominio { get; set; } = string.Empty;
        }

        private class ErrorResponseDto
        {
            public string? mensaje { get; set; }
        }
    }
}
