namespace CarnetDigital.Frontend.Services.Auth
{
    // Punto único de reemplazo cuando llegue Web1 (login real): solo cambia la implementación registrada en Program.cs.
    public interface ITokenProvider
    {
        string? GetToken();
        void SetToken(string token);
        void ClearToken();
        bool IsAuthenticated();
        string? GetUsername();
        void SetUsername(string username);
    }
}
