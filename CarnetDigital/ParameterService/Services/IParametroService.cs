using ParameterService.Entities;

namespace ParameterService.Services
{
    public interface IParametroService
    {
        Task<IEnumerable<Parametro>> GetAllAsync();
        Task<Parametro?> GetByIdAsync(string id);
        Task<bool> CreateAsync(Parametro p);
        Task<bool> UpdateAsync(Parametro p);
        Task<bool> DeleteAsync(string id);
    }
}