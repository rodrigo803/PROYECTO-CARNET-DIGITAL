using CarnetDigital.Frontend.Models.Roles;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Roles
{
    public interface IRolesApiService
    {
        Task<List<RolDto>> GetAllAsync();
        Task<RolDto?> GetByIdAsync(string id);
        Task<ApiResult<RolDto>> CreateAsync(RolDto rol);
        Task<ApiResult<RolDto>> UpdateAsync(string id, RolDto rol);
        Task<bool> DeleteAsync(string id);
    }
}
