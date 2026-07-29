using CarnetDigital.Frontend.Models.Bitacoras;
using CarnetDigital.Frontend.Models.Shared;

namespace CarnetDigital.Frontend.Services.Bitacoras
{
    public interface IBitacorasApiService
    {
        Task<PagedResult<BitacoraDto>> GetPagedAsync(
            DateTime? fecha,
            int? usuarioId,
            string? descripcion,
            int page,
            int pageSize);
    }
}
