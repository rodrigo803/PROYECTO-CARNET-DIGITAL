using Microservicio.Instituciones.Entities;

namespace Microservicio.Instituciones.Services
{
    public interface IInstitucionesService
    {
        Task<IEnumerable<Institucion>> GetAllAsync();
        Task<Institucion?> GetByIdAsync(int id);
        Task<Institucion> CreateAsync(Institucion institucion);
        Task<Institucion?> UpdateAsync(int id, Institucion institucion);
        Task<bool> DeleteAsync(int id);
    }
}