namespace CarnetDigital.Frontend.Models.Auth;

public class LoginResponse
{
    public DateTime Expires_In { get; set; }

    public string Access_Token { get; set; } = string.Empty;

    public string Refresh_Token { get; set; } = string.Empty;

    public int UsuarioID { get; set; }
}
