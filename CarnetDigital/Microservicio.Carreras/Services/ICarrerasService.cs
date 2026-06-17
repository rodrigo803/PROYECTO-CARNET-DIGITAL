using Microservicio.Carreras.Entities;

namespace Microservicio.Carreras.Services
{
    public interface ICarrerasService
    {
        Task<IEnumerable<Carrera>> GetAllAsync();
        Task<Carrera?> GetByIdAsync(int id);
        Task<Carrera> CreateAsync(Carrera carrera);
        Task<Carrera?> UpdateAsync(int id, Carrera carrera);
        Task<bool> DeleteAsync(int id);
    }
}