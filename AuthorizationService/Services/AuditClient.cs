using System.Net.Http.Json;

namespace AuthorizationService.Services
{
    public class AuditClient
    {
        private readonly HttpClient _http;

        public AuditClient(HttpClient http)
        {
            _http = http;
        }

        public async Task Registrar(HttpRequest request, int usuarioId, string descripcion)
        {
            var authHeader = request.Headers["Authorization"].ToString();

            var requestMsg = new HttpRequestMessage(
                HttpMethod.Post,
                "https://localhost:7290/bitacora" // ⚠️ cambia puerto real
            );

            requestMsg.Headers.Add("Authorization", authHeader);

            requestMsg.Content = JsonContent.Create(new
            {
                UsuarioId = usuarioId,
                Descripcion = descripcion
            });

            await _http.SendAsync(requestMsg);
        }
    }
}
