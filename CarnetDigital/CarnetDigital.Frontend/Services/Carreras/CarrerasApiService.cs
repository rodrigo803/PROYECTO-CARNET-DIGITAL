using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.Carreras;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Carreras
{
    public class CarrerasApiService : ICarrerasApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CarrerasApiService> _logger;

        public CarrerasApiService(HttpClient httpClient, ILogger<CarrerasApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<CarreraDto>> GetAllAsync()
        {
            try
            {
                var carreras = await _httpClient.GetFromJsonAsync<List<CarreraDto>>("/api/Carreras");
                return carreras ?? new List<CarreraDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener carreras");
                return new List<CarreraDto>();
            }
        }

        public async Task<CarreraDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Carreras/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<CarreraDto>();
        }

        public async Task<ApiResult<CarreraDto>> CreateAsync(string nombre, string director, string email, string telefono, int idInstitucion)
        {
            var body = new CarreraRequestDto { Nombre = nombre, Director = director, Email = email, Telefono = telefono, IdInstitucion = idInstitucion, Activo = true };
            var response = await _httpClient.PostAsJsonAsync("/api/Carreras", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<ApiResult<CarreraDto>> UpdateAsync(int id, string nombre, string director, string email, string telefono, int idInstitucion)
        {
            var body = new CarreraRequestDto { Nombre = nombre, Director = director, Email = email, Telefono = telefono, IdInstitucion = idInstitucion, Activo = true };
            var response = await _httpClient.PutAsJsonAsync($"/api/Carreras/{id}", body);
            return await LeerResultadoAsync(response);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Carreras/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<ApiResult<CarreraDto>> LeerResultadoAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var carrera = await response.Content.ReadFromJsonAsync<CarreraDto>();
                return ApiResult<CarreraDto>.Ok(carrera!);
            }

            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            return ApiResult<CarreraDto>.Fail(error?.mensaje ?? "Ocurrió un error al procesar la solicitud");
        }

        private class CarreraRequestDto
        {
            public string Nombre { get; set; } = string.Empty;
            public string Director { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Telefono { get; set; } = string.Empty;
            public int IdInstitucion { get; set; }
            public bool Activo { get; set; }
        }

        private class ErrorResponseDto
        {
            public string? mensaje { get; set; }
        }
    }
}
