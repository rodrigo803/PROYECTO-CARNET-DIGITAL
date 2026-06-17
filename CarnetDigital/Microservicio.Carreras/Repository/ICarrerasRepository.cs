using Microservicio.Carreras.Entities;

namespace Microservicio.Carreras.Repository
{
    public interface ICarrerasRepository
    {
        Task<IEnumerable<Carrera>> GetAllAsync();
        Task<Carrera?> GetByIdAsync(int id);
        Task<Carrera> CreateAsync(Carrera carrera);
        Task<Carrera?> UpdateAsync(int id, Carrera carrera);
        Task<bool> DeleteAsync(int id);
    }
}