using CarnetDigital.Frontend.Models.Auth;

namespace CarnetDigital.Frontend.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginViewModel model);
    }
}
