using Microservicio.Areas.DTOs;

namespace Microservicio.Areas.Interfaces
{
    public interface IInstitucionesValidator
    {
        Task<InstitucionInfo?> ObtenerInstitucionAsync(int idInstitucion, string token);
    }
}