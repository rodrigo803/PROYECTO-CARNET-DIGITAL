using AccessControlService.Entities;
using AccessControlService.Repository;

namespace AccessControlService.Services
{
    public class PantallaService : IPantallaService
    {
        private readonly PantallaRepository _repo;

        public PantallaService(PantallaRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Pantalla>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<Pantalla?> GetByIdAsync(string id)
            => await _repo.GetByIdAsync(id);

        public async Task<bool> CreateAsync(Pantalla p)
            => await _repo.CreateAsync(p) > 0;

        public async Task<bool> UpdateAsync(Pantalla p)
            => await _repo.UpdateAsync(p) > 0;

        public async Task<bool> DeleteAsync(string id)
            => await _repo.DeleteAsync(id) > 0;
    }
}