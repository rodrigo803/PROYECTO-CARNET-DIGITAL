namespace Microservicio.Instituciones.Interfaces
{
    public interface IBitacoraService
    {
        Task RegistrarAsync(int usuarioId, string descripcion, string token);
    }
}