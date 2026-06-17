using Microservicio.Carreras.Entities;

namespace Microservicio.Carreras.Services
{
    public interface IInstitucionesValidator
    {
        Task<InstitucionInfo?> ObtenerInstitucionAsync(int idInstitucion, string token);
    }
}