// TEMPORAL - reemplazar con Web1
namespace CarnetDigital.Frontend.Services.Auth
{
    public class SessionTokenProvider : ITokenProvider
    {
        private const string SessionKey = "AuthToken";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public string? GetToken() => Session.GetString(SessionKey);

        public void SetToken(string token) => Session.SetString(SessionKey, token);

        public void ClearToken() => Session.Remove(SessionKey);

        public bool IsAuthenticated() => !string.IsNullOrWhiteSpace(GetToken());
    }
}
