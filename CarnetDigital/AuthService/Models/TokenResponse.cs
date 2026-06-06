namespace AuthService.Models
{
    public class TokenResponse
    {
        public DateTime expires_in { get; set; }
        public string access_token { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public int usuarioID { get; set; }
    }
}
