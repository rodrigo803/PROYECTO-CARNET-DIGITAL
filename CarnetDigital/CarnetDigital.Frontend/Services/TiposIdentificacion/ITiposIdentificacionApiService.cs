using CarnetDigital.Frontend.Models.TiposIdentificacion;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.TiposIdentificacion
{
    public interface ITiposIdentificacionApiService
    {
        Task<List<TipoIdentificacionDto>> GetAllAsync();
        Task<TipoIdentificacionDto?> GetByIdAsync(int id);
        Task<ApiResult<TipoIdentificacionDto>> CreateAsync(string nombre);
        Task<ApiResult<TipoIdentificacionDto>> UpdateAsync(int id, string nombre);
        Task<bool> DeleteAsync(int id);
    }
}
