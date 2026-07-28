namespace CarnetDigital.Frontend.Models.Auth;

public class LoginResult
{
    public bool Success { get; set; }
    public LoginResponse? Data { get; set; }
    public string? Mensaje { get; set; }
}

public class AuthErrorResponse
{
    public string? Mensaje { get; set; }
}
