using Microservicio.Instituciones.Entities;
using Microservicio.Instituciones.Repository;

namespace Microservicio.Instituciones.Services
{
    public class InstitucionesService : IInstitucionesService
    {
        private readonly IInstitucionesRepository _repository;

        public InstitucionesService(IInstitucionesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Institucion>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Institucion?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Institucion> CreateAsync(Institucion institucion)
        {
            return await _repository.CreateAsync(institucion);
        }

        public async Task<Institucion?> UpdateAsync(int id, Institucion institucion)
        {
            return await _repository.UpdateAsync(id, institucion);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}