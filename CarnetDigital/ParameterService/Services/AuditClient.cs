using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ParameterService.Services
{
    public class AuditClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<AuditClient> _logger;

        public AuditClient(HttpClient http, ILogger<AuditClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task LogAsync(int usuarioId, string descripcion, string token)
        {
            try
            {
                var body = new
                {
                    UsuarioId = usuarioId,
                    Descripcion = descripcion
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _http.PostAsync("/bitacora", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Error al registrar bitácora. Status: {Status}, Mensaje: {Mensaje}",
                        response.StatusCode, errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al registrar bitácora");
            }
        }
    }
}