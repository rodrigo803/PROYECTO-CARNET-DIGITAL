using CarnetDigital.Frontend.Models.Instituciones;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Instituciones
{
    public interface IInstitucionesCrudApiService
    {
        Task<List<InstitucionDetalleDto>> GetAllAsync();
        Task<InstitucionDetalleDto?> GetByIdAsync(int id);
        Task<ApiResult<InstitucionDetalleDto>> CreateAsync(string nombre, string email, string telefono, List<string> dominios);
        Task<ApiResult<InstitucionDetalleDto>> UpdateAsync(int id, string nombre, string email, string telefono, List<string> dominios);
        Task<bool> DeleteAsync(int id);
    }
}
