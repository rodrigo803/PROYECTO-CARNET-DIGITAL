using CarnetDigital.Frontend.Models.TiposIdentificacion;

namespace CarnetDigital.Frontend.Services.TiposIdentificacion
{
    public interface ITiposIdentificacionApiService
    {
        Task<List<TipoIdentificacionDto>> GetAllAsync();
    }
}
