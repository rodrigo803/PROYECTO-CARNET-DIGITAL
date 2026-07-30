using System.ComponentModel.DataAnnotations;
using CarnetDigital.Frontend.Validation;

namespace CarnetDigital.Frontend.Models.TiposIdentificacion
{
    public class TipoIdentificacionFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [NotWhitespace]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }
}
