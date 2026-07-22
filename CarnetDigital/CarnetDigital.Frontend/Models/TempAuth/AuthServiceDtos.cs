// TEMPORAL - reemplazar con Web1
namespace CarnetDigital.Frontend.Models.TempAuth
{
    public class LoginRequestDto
    {
        public string usuario { get; set; } = string.Empty;
        public string contrasena { get; set; } = string.Empty;
        public string tipousuario { get; set; } = string.Empty;
    }

    public class TokenResponseDto
    {
        public DateTime expires_in { get; set; }
        public string access_token { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public int usuarioID { get; set; }
    }
}
