using AccessControlService.Entities;

namespace AccessControlService.Services
{
    public interface IPantallaService
    {
        Task<IEnumerable<Pantalla>> GetAllAsync();
        Task<Pantalla?> GetByIdAsync(string id);
        Task<bool> CreateAsync(Pantalla p);
        Task<bool> UpdateAsync(Pantalla p);
        Task<bool> DeleteAsync(string id);
    }
}