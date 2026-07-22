using System.ComponentModel.DataAnnotations;

namespace CarnetDigital.Frontend.Validation
{
    // Reutilizable por Areas/Carreras/Instituciones: [Required] no rechaza cadenas de solo espacios.
    public class NotWhitespaceAttribute : ValidationAttribute
    {
        public NotWhitespaceAttribute()
        {
            ErrorMessage = "El campo {0} es requerido";
        }

        public override bool IsValid(object? value)
        {
            return value is string s && !string.IsNullOrWhiteSpace(s);
        }
    }
}
