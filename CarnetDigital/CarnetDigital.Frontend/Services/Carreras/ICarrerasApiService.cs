using CarnetDigital.Frontend.Models.Carreras;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Carreras
{
    public interface ICarrerasApiService
    {
        Task<List<CarreraDto>> GetAllAsync();
        Task<CarreraDto?> GetByIdAsync(int id);
        Task<ApiResult<CarreraDto>> CreateAsync(string nombre, string director, string email, string telefono, int idInstitucion);
        Task<ApiResult<CarreraDto>> UpdateAsync(int id, string nombre, string director, string email, string telefono, int idInstitucion);
        Task<bool> DeleteAsync(int id);
    }
}
