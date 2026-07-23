// TEMPORAL - reemplazar con Web1
using System.ComponentModel.DataAnnotations;
using CarnetDigital.Frontend.Validation;

namespace CarnetDigital.Frontend.Models.TempAuth
{
    public class LoginFormViewModel
    {
        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [NotWhitespace]
        public string Usuario { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [NotWhitespace]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;

        [Display(Name = "Tipo de usuario")]
        [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
        [NotWhitespace]
        public string TipoUsuario { get; set; } = string.Empty;
    }
}
