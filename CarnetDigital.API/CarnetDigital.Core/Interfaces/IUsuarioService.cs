using System;
using System.Collections.Generic;
using System.Text;
using CarnetDigital.Core.Entities;

namespace CarnetDigital.Core.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario> CrearUsuarioAsync(Usuario usuario, string contrasenaPlana);
        Task<Usuario> AutoregistroAsync(Usuario usuario, string contrasenaPlana);
        Task<bool> CambiarEstadoAsync(string email, int nuevoEstadoId);
    }
}
