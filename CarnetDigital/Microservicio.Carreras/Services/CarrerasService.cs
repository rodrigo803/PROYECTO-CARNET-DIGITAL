using Microservicio.Carreras.Entities;
using Microservicio.Carreras.Repository;

namespace Microservicio.Carreras.Services
{
    public class CarrerasService : ICarrerasService
    {
        private readonly ICarrerasRepository _repository;

        public CarrerasService(ICarrerasRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Carrera>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<Carrera?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<Carrera> CreateAsync(Carrera carrera) => await _repository.CreateAsync(carrera);
        public async Task<Carrera?> UpdateAsync(int id, Carrera carrera) => await _repository.UpdateAsync(id, carrera);
        public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }
}