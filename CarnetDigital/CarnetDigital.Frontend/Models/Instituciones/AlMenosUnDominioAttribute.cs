using System.ComponentModel.DataAnnotations;

namespace CarnetDigital.Frontend.Models.Instituciones
{
    // Específico del textarea de dominios de Instituciones: no depende del 400 del backend.
    public class AlMenosUnDominioAttribute : ValidationAttribute
    {
        public AlMenosUnDominioAttribute()
        {
            ErrorMessage = "Debe indicar al menos un dominio";
        }

        public override bool IsValid(object? value)
        {
            return value is string s && DominiosTextoHelper.Parse(s).Count > 0;
        }
    }
}
