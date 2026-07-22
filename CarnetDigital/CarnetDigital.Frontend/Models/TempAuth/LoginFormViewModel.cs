// TEMPORAL - reemplazar con Web1
using System.ComponentModel.DataAnnotations;
using CarnetDigital.Frontend.Validation;

namespace CarnetDigital.Frontend.Models.TempAuth
{
    public class LoginFormViewModel
    {
        [Display(Name = "Usuario")]
        [NotWhitespace]
        public string Usuario { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [NotWhitespace]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;

        [Display(Name = "Tipo de usuario")]
        [NotWhitespace]
        public string TipoUsuario { get; set; } = string.Empty;
    }
}
