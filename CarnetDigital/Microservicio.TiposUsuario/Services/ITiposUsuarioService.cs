using Microservicio.TiposUsuario.Entities;

namespace Microservicio.TiposUsuario.Services
{
    public interface ITiposUsuarioService
    {
        Task<IEnumerable<TipoUsuario>> GetAllAsync();
        Task<TipoUsuario?> GetByIdAsync(int id);
        Task<TipoUsuario> CreateAsync(TipoUsuario tipo);
        Task<TipoUsuario?> UpdateAsync(int id, TipoUsuario tipo);
        Task<bool> DeleteAsync(int id);
    }
}