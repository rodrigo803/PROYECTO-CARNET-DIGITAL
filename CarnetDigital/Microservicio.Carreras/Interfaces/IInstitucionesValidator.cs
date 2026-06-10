using Microservicio.Carreras.DTOs;

namespace Microservicio.Carreras.Interfaces
{
    public interface IInstitucionesValidator
    {
        Task<InstitucionInfo?> ObtenerInstitucionAsync(int idInstitucion, string token);
    }
}