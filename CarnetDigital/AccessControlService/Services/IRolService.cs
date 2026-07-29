using AccessControlService.Entities;

namespace AccessControlService.Services
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(string id);
        Task<bool> CreateAsync(Rol rol);
        Task<bool> UpdateAsync(Rol rol);
        Task<bool> DeleteAsync(string id);
    }
}