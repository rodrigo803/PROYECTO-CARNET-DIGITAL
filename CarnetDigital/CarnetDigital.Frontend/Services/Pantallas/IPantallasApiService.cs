using CarnetDigital.Frontend.Models.Pantallas;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Pantallas
{
    public interface IPantallasApiService
    {
        Task<List<PantallaDto>> GetAllAsync();
        Task<PantallaDto?> GetByIdAsync(string id);
        Task<ApiResult<PantallaDto>> CreateAsync(PantallaDto pantalla);
        Task<ApiResult<PantallaDto>> UpdateAsync(string id, PantallaDto pantalla);
        Task<bool> DeleteAsync(string id);
    }
}
