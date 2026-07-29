using CarnetDigital.Frontend.Models.Parametros;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Parametros
{
    public interface IParametrosApiService
    {
        Task<List<ParametroDto>> GetAllAsync();
        Task<ParametroDto?> GetByIdAsync(string id);
        Task<ApiResult<ParametroDto>> CreateAsync(string id, string valor);
        Task<ApiResult<ParametroDto>> UpdateAsync(string id, string valor);
        Task<bool> DeleteAsync(string id);
    }
}
