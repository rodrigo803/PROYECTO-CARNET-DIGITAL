using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace CarnetDigital.Frontend.Validation
{
    // Para listas de selección múltiple (ej. checkboxes de Pantallas en Roles),
    // donde [Required] no alcanza: en una List<string> solo rechaza null, no una lista vacía.
    public class AtLeastOneAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is ICollection collection && collection.Count > 0;
        }
    }
}
