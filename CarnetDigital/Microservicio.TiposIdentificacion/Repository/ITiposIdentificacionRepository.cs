using Microservicio.TiposIdentificacion.Entities;

namespace Microservicio.TiposIdentificacion.Repository
{
    public interface ITiposIdentificacionRepository
    {
        Task<IEnumerable<TipoIdentificacion>> GetAllAsync();
        Task<TipoIdentificacion?> GetByIdAsync(int id);
        Task<TipoIdentificacion> CreateAsync(TipoIdentificacion tipo);
        Task<TipoIdentificacion?> UpdateAsync(int id, TipoIdentificacion tipo);
        Task<bool> DeleteAsync(int id);
    }
}