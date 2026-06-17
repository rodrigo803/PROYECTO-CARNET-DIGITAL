using Microservicio.TiposIdentificacion.Entities;
using Microservicio.TiposIdentificacion.Repository;

namespace Microservicio.TiposIdentificacion.Services
{
    public class TiposIdentificacionService : ITiposIdentificacionService
    {
        private readonly ITiposIdentificacionRepository _repository;

        public TiposIdentificacionService(ITiposIdentificacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TipoIdentificacion>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<TipoIdentificacion?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<TipoIdentificacion> CreateAsync(TipoIdentificacion tipo) => await _repository.CreateAsync(tipo);
        public async Task<TipoIdentificacion?> UpdateAsync(int id, TipoIdentificacion tipo) => await _repository.UpdateAsync(id, tipo);
        public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }
}