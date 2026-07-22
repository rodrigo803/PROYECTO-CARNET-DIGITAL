using CarnetDigital.Frontend.Models.Areas;

namespace CarnetDigital.Frontend.Services.Areas
{
    public interface IAreasApiService
    {
        Task<List<AreaDto>> GetAllAsync();
        Task<AreaDto?> GetByIdAsync(int id);
        Task<ApiResult<AreaDto>> CreateAsync(string nombre, int idInstitucion);
        Task<ApiResult<AreaDto>> UpdateAsync(int id, string nombre, int idInstitucion);
        Task<bool> DeleteAsync(int id);
    }

    public class ApiResult<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? ErrorMessage { get; init; }

        public static ApiResult<T> Ok(T data) => new() { Success = true, Data = data };
        public static ApiResult<T> Fail(string mensaje) => new() { Success = false, ErrorMessage = mensaje };
    }
}
