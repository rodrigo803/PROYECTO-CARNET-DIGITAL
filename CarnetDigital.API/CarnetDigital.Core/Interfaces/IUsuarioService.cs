using System;
using System.Collections.Generic;
using System.Text;
using CarnetDigital.Core.Entities;

namespace CarnetDigital.Core.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario> CrearUsuarioAsync(Usuario usuario, string contrasenaPlana);
        Task<bool> AutoregistroAsync(Usuario usuario, string contrasenaPlana);
        Task<bool> ConfirmarRegistroAsync(string token);
        Task<bool> CambiarEstadoAsync(string email, int nuevoEstadoId);
        Task<bool> ActualizarFotografiaAsync(string email, string fotoBase64);
        Task<string> GenerarQRBase64Async(string identificacion);
    }
}
