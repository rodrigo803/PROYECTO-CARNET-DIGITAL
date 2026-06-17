using Microservicio.Areas.Entities;
using Microservicio.Areas.Repository;

namespace Microservicio.Areas.Services
{
    public class AreasService : IAreasService
    {
        private readonly IAreasRepository _repository;

        public AreasService(IAreasRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AreaTrabajo>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<AreaTrabajo?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<AreaTrabajo> CreateAsync(AreaTrabajo area) => await _repository.CreateAsync(area);
        public async Task<AreaTrabajo?> UpdateAsync(int id, AreaTrabajo area) => await _repository.UpdateAsync(id, area);
        public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);
    }
}