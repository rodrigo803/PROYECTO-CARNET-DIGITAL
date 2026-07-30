using CarnetDigital.Frontend.Models.Usuarios;
using CarnetDigital.Frontend.Services.Areas;

namespace CarnetDigital.Frontend.Services.Usuarios
{
    public interface IUsuarioApiService
    {
        Task<UsuarioDTO?> ObtenerPorIdAsync(string identificacion);
        Task<List<UsuarioDTO>> FiltrarAsync(string? identificacion, string? nombre, int? tipoUsuarioId);
        Task<ApiResult<UsuarioDTO>> CrearAsync(UsuarioDTO usuario);
        Task<ApiResult<UsuarioDTO>> ActualizarAsync(UsuarioDTO usuario);
        Task<bool> EliminarAsync(string identificacion);

        Task<bool> AutoregistroAsync(UsuarioDTO usuario);
        Task<bool> ConfirmarRegistroAsync(string token);

        Task<bool> CambiarEstadoAsync(string identificacion, int estadoId);

        Task<string?> ObtenerFotografiaAsync(string identificacion);
        Task<bool> ActualizarFotografiaAsync(string identificacion, string fotoBase64);
        Task<bool> EliminarFotografiaAsync(string identificacion);

        Task<string?> ObtenerQRAsync(string identificacion);
    }
}
