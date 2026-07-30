using CarnetDigital.Frontend.Models.TiposUsuario;

namespace CarnetDigital.Frontend.Services.TiposUsuario
{
    public interface ITiposUsuarioApiService
    {
        Task<List<TipoUsuarioDto>> GetAllAsync();
    }
}
