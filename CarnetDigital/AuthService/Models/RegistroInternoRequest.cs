namespace AuthService.Models
{
    public class RegistroInternoRequest
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
    }
}
