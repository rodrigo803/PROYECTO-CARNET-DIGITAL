using Microservicio.TiposUsuario.Entities;
using Microservicio.TiposUsuario.Repository;

namespace Microservicio.TiposUsuario.Services
{
    public class TiposUsuarioService : ITiposUsuarioService
    {
        private readonly ITiposUsuarioRepository _repository;

        public TiposUsuarioService(ITiposUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TipoUsuario>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<TipoUsuario?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<TipoUsuario> CreateAsync(TipoUsuario tipo) => await _repository.CreateAsync(tipo);
        public async Task<TipoUsuario?> UpdateAsync(int id, TipoUsuario tipo) => await _repository.UpdateAsync(id, tipo);
        public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }
}