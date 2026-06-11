using System.Net.Http.Headers;

namespace OrganizationService.Services
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

            if (string.IsNullOrWhiteSpace(authHeader))
                return;

            var requestMsg = new HttpRequestMessage(
                HttpMethod.Post,
                "https://localhost:7290/bitacora" 
            );

            requestMsg.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", authHeader.Replace("Bearer ", ""));

            requestMsg.Content = JsonContent.Create(new
            {
                usuarioId = usuarioId,
                descripcion = descripcion
            });

            await _http.SendAsync(requestMsg);
        }
    }
}