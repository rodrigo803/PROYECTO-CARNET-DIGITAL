using Microservicio.Areas.Entities;

namespace Microservicio.Areas.Services
{
    public interface IInstitucionesValidator
    {
        Task<InstitucionInfo?> ObtenerInstitucionAsync(int idInstitucion, string token);
    }
}