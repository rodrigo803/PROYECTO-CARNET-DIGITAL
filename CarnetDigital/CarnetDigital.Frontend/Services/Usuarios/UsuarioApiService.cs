using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.Usuarios;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Usuarios
{
    public class UsuarioApiService : IUsuarioApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsuarioApiService> _logger;

        public UsuarioApiService(HttpClient httpClient, ILogger<UsuarioApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UsuarioDTO?> ObtenerPorIdAsync(string identificacion)
        {
            var response = await _httpClient.GetAsync($"/api/usuario/{identificacion}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UsuarioDTO>();
        }

        public async Task<List<UsuarioDTO>> FiltrarAsync(string? identificacion, string? nombre, int? tipoUsuarioId)
        {
            try
            {
                var query = new List<string>();
                if (!string.IsNullOrWhiteSpace(identificacion))
                    query.Add($"identificacion={Uri.EscapeDataString(identificacion)}");
                if (!string.IsNullOrWhiteSpace(nombre))
                    query.Add($"nombre={Uri.EscapeDataString(nombre)}");
                if (tipoUsuarioId.HasValue)
                    query.Add($"tipoUsuarioId={tipoUsuarioId.Value}");

                var url = "/api/usuario/filtrar" + (query.Count > 0 ? "?" + string.Join("&", query) : string.Empty);
                var usuarios = await _httpClient.GetFromJsonAsync<List<UsuarioDTO>>(url);
                return usuarios ?? new List<UsuarioDTO>();
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al filtrar usuarios");
                return new List<UsuarioDTO>();
            }
        }

        public async Task<ApiResult<UsuarioDTO>> CrearAsync(UsuarioDTO usuario)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/usuario/crear", usuario);
            return await LeerResultadoAsync(response);
        }

        public async Task<ApiResult<UsuarioDTO>> ActualizarAsync(UsuarioDTO usuario)
        {
            var response = await _httpClient.PutAsJsonAsync("/api/usuario/actualizar", usuario);
            return await LeerResultadoAsync(response);
        }

        public async Task<bool> EliminarAsync(string identificacion)
        {
            var response = await _httpClient.DeleteAsync($"/api/usuario/eliminar/{identificacion}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AutoregistroAsync(UsuarioDTO usuario)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/usuario/autoregistro", usuario);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ConfirmarRegistroAsync(string token)
        {
            var response = await _httpClient.GetAsync($"/api/usuario/autoregistro/confirmar?token={Uri.EscapeDataString(token)}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CambiarEstadoAsync(string identificacion, int estadoId)
        {
            var body = new { Identificacion = identificacion, EstadoId = estadoId };
            var response = await _httpClient.PatchAsJsonAsync($"/api/usuario/estado/{identificacion}", body);
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> ObtenerFotografiaAsync(string identificacion)
        {
            var response = await _httpClient.GetAsync($"/api/usuario/fotografia/{identificacion}");
            if (!response.IsSuccessStatusCode)
                return null;

            var foto = await response.Content.ReadFromJsonAsync<FotografiaResponseDto>();
            return foto?.FotoBase64;
        }

        public async Task<bool> ActualizarFotografiaAsync(string identificacion, string fotoBase64)
        {
            var body = new { Identificacion = identificacion, FotoBase64 = fotoBase64 };
            var response = await _httpClient.PatchAsJsonAsync("/api/usuario/fotografia", body);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarFotografiaAsync(string identificacion)
        {
            var response = await _httpClient.DeleteAsync($"/api/usuario/fotografia/{identificacion}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> ObtenerQRAsync(string identificacion)
        {
            var response = await _httpClient.GetAsync($"/api/usuario/qr/{identificacion}");
            if (!response.IsSuccessStatusCode)
                return null;

            var qr = await response.Content.ReadFromJsonAsync<QrResponseDto>();
            return qr?.QrImagenBase64;
        }

        private static async Task<ApiResult<UsuarioDTO>> LeerResultadoAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var usuario = await response.Content.ReadFromJsonAsync<UsuarioDTO>();
                return ApiResult<UsuarioDTO>.Ok(usuario!);
            }

            var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            return ApiResult<UsuarioDTO>.Fail(error?.Mensaje ?? "Ocurrió un error al procesar la solicitud");
        }

        private class FotografiaResponseDto
        {
            public string? Identificacion { get; set; }
            public string? FotoBase64 { get; set; }
        }

        private class QrResponseDto
        {
            public string? QrImagenBase64 { get; set; }
        }

        private class ErrorResponseDto
        {
            public string? Mensaje { get; set; }
        }
    }
}
