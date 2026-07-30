using CarnetDigital.Frontend.Models.TiposUsuario;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.TiposUsuario
{
    public interface ITiposUsuarioApiService
    {
        Task<List<TipoUsuarioDto>> GetAllAsync();
        Task<TipoUsuarioDto?> GetByIdAsync(int id);
        Task<ApiResult<TipoUsuarioDto>> CreateAsync(string nombre);
        Task<ApiResult<TipoUsuarioDto>> UpdateAsync(int id, string nombre);
        Task<bool> DeleteAsync(int id);
    }
}
