using Microservicio.Areas.Entities;

namespace Microservicio.Areas.Repository
{
    public interface IAreasRepository
    {
        Task<IEnumerable<AreaTrabajo>> GetAllAsync();
        Task<AreaTrabajo?> GetByIdAsync(int id);
        Task<AreaTrabajo> CreateAsync(AreaTrabajo area);
        Task<AreaTrabajo?> UpdateAsync(int id, AreaTrabajo area);
        Task<bool> DeleteAsync(int id);
    }
}