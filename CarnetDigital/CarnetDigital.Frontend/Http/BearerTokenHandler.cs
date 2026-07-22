using System.Net.Http.Headers;
using CarnetDigital.Frontend.Services.Auth;

namespace CarnetDigital.Frontend.Http
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;

        public BearerTokenHandler(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _tokenProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
