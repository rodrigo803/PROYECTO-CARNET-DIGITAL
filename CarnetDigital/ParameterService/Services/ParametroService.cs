using ParameterService.Entities;
using ParameterService.Repository;

namespace ParameterService.Services
{
    public class ParametroService : IParametroService
    {
        private readonly ParametroRepository _repo;

        public ParametroService(ParametroRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Parametro>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<Parametro?> GetByIdAsync(string id)
            => await _repo.GetByIdAsync(id);

        public async Task<bool> CreateAsync(Parametro p)
            => await _repo.CreateAsync(p) > 0;

        public async Task<bool> UpdateAsync(Parametro p)
            => await _repo.UpdateAsync(p) > 0;

        public async Task<bool> DeleteAsync(string id)
            => await _repo.DeleteAsync(id) > 0;
    }
}