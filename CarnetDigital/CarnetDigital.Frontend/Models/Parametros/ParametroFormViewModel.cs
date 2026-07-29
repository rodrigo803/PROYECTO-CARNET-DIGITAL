using System.ComponentModel.DataAnnotations;
using CarnetDigital.Frontend.Validation;

namespace CarnetDigital.Frontend.Models.Parametros
{
    public class ParametroFormViewModel
    {
        [Display(Name = "Código")]
        [Required(ErrorMessage = "El código es obligatorio.")]
        [NotWhitespace]
        [MaxLength(10, ErrorMessage = "El código no puede superar los 10 caracteres.")]
        [RegularExpression(@"^[A-Z]+$", ErrorMessage = "El código debe contener solo letras mayúsculas (A-Z), sin espacios ni números.")]
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Valor")]
        [Required(ErrorMessage = "El valor es obligatorio.")]
        [NotWhitespace]
        [MaxLength(500, ErrorMessage = "El valor no puede superar los 500 caracteres.")]
        public string Valor { get; set; } = string.Empty;

        public bool EsEdicion { get; set; }
    }
}
