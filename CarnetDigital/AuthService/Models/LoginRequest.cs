namespace AuthService.Models
{
    public class LoginRequest
    {
        public string usuario { get; set; } = string.Empty;
        public string contrasena { get; set; } = string.Empty;
        public string tipousuario { get; set; } = string.Empty;
    }
}