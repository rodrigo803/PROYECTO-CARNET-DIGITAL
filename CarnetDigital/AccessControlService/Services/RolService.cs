using AccessControlService.Entities;
using AccessControlService.Repository;

namespace AccessControlService.Services
{
    public class RolService : IRolService
    {
        private readonly RolRepository _repo;

        public RolService(RolRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<Rol?> GetByIdAsync(string id)
            => await _repo.GetByIdAsync(id);

        public async Task<bool> CreateAsync(Rol rol)
            => await _repo.CreateAsync(rol) > 0;

        public async Task<bool> UpdateAsync(Rol rol)
            => await _repo.UpdateAsync(rol) > 0;

        public async Task<bool> DeleteAsync(string id)
            => await _repo.DeleteAsync(id) > 0;
    }
}