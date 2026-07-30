using System.Threading.Tasks;
using static Microservicio.Usuario.Entities.UsuarioDTOs;

namespace Microservicio.Usuario.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioActualizacionDto> CrearUsuarioAsync(Entities.Usuario usuario, string contrasenaPlana);
        Task<UsuarioActualizacionDto> AutoregistroAsync(UsuarioRegistroDto dto);
        Task<UsuarioActualizacionDto?> ConfirmarRegistroAsync(string token);
        Task<UsuarioActualizacionDto?> CambiarEstadoAsync(string Identificacion, int nuevoEstadoId);
        Task<UsuarioActualizacionDto?> ActualizarFotografiaAsync(FotografiaDto peticion);
        Task<string> GenerarQRBase64Async(string identificacion);
        Task<UsuarioActualizacionDto?> ActualizarUsuarioAsync(UsuarioActualizacionDto registro);
        Task<UsuarioActualizacionDto?> EliminarUsuarioAsync(string identificacion);

        Task<IEnumerable<UsuarioResumenDto>> ObtenerTodosAsync();
        Task<UsuarioResumenDto?> ObtenerPorIdAsync(string identificacion);
        Task<IEnumerable<UsuarioResumenDto>> FiltrarAsync(string? identificacion, string? nombre, int? tipoUsuarioId);
        Task<FotografiaDto?> ObtenerFotografiaAsync(string identificacion);
        Task<UsuarioActualizacionDto?> EliminarFotografiaAsync(string identificacion);
    }
}

