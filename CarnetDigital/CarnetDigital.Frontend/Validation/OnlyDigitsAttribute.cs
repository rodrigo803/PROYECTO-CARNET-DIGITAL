using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CarnetDigital.Frontend.Validation
{
    // Reutilizable por Carreras/Instituciones: teléfono solo dígitos, sin guiones, espacios ni "+".
    public partial class OnlyDigitsAttribute : ValidationAttribute
    {
        public OnlyDigitsAttribute()
        {
            ErrorMessage = "El teléfono solo permite valores numéricos.";
        }

        public override bool IsValid(object? value)
        {
            return value is string s && DigitsOnlyRegex().IsMatch(s);
        }

        [GeneratedRegex(@"^\d+$")]
        private static partial Regex DigitsOnlyRegex();
    }
}
