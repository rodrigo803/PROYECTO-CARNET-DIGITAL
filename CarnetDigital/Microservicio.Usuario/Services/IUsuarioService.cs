using System;
using System.Collections.Generic;
using System.Text;


namespace Microservicio.Usuario.Services
{
    public interface IUsuarioService
    {
        public Task<Entities.Usuario> CrearUsuarioAsync(Entities.Usuario usuario, string contrasenaPlana);
        public Task<bool> AutoregistroAsync(Entities.Usuario usuario, string contrasenaPlana);
        public Task<bool> ConfirmarRegistroAsync(string token);
        public Task<bool> CambiarEstadoAsync(string email, int nuevoEstadoId);
        public Task<bool> ActualizarFotografiaAsync(string email, string fotoBase64);
        public Task<string> GenerarQRBase64Async(string identificacion);
        Task<bool> ActualizarUsuarioAsync(Entities.Usuario registro);
        Task<bool> EliminarUsuarioAsync(string email);
    }
}
