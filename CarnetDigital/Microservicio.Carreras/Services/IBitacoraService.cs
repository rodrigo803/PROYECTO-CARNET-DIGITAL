namespace Microservicio.Carreras.Services
{
    public interface IBitacoraService
    {
        Task RegistrarAsync(int usuarioId, string descripcion, string token);
    }
}