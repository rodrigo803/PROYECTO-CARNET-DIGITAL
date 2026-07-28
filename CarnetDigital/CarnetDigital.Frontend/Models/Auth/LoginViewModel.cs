using System.ComponentModel.DataAnnotations;

namespace CarnetDigital.Frontend.Models.Auth;

public class LoginViewModel
{
    [Required(ErrorMessage = "El usuario es requerido")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione un tipo de usuario")]
    public string TipoUsuario { get; set; } = string.Empty;
}
