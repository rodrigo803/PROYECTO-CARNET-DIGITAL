using System;
using System.Collections.Generic;
using System.Text;
using static Microservicio.Usuario.Entities.UsuarioDTOs;


namespace Microservicio.Usuario.Services
{
    public interface IUsuarioService
    {
        Task<Entities.Usuario> CrearUsuarioAsync(Entities.Usuario usuario, string contrasenaPlana);
        Task<bool> AutoregistroAsync(Entities.Usuario usuario, string contrasenaPlana);
        Task<bool> ConfirmarRegistroAsync(string token);

        // ¡OJO AQUÍ! Asegúrate que aquí diga 'identificacion' y no 'email'
        Task<bool> CambiarEstadoAsync(string identificacion, int nuevoEstadoId);
        Task<bool> ActualizarFotografiaAsync(string identificacion, string fotoBase64);

        Task<string> GenerarQRBase64Async(string identificacion);
        Task<bool> ActualizarUsuarioAsync(UsuarioActualizacionDto registro);
        Task<bool> EliminarUsuarioAsync(string identificacion); // <--- Identificación!
    }
}
