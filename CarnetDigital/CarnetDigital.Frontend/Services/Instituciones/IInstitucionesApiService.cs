using CarnetDigital.Frontend.Models.Instituciones;

namespace CarnetDigital.Frontend.Services.Instituciones
{
    public interface IInstitucionesApiService
    {
        Task<List<InstitucionDto>> ObtenerActivasAsync();
    }
}
