using System.ComponentModel.DataAnnotations;
using CarnetDigital.Frontend.Validation;

namespace CarnetDigital.Frontend.Models.Roles
{
    public class RolFormViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "El Id es obligatorio.")]
        [NotWhitespace]
        [MaxLength(50, ErrorMessage = "El Id no puede superar los 50 caracteres.")]
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [NotWhitespace]
        [MaxLength(40, ErrorMessage = "El nombre no puede superar los 40 caracteres.")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "El nombre debe contener solo letras y espacios.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Pantallas")]
        [AtLeastOne(ErrorMessage = "Debe seleccionar al menos una pantalla.")]
        public List<string> PantallasSeleccionadas { get; set; } = new();

        public List<PantallaCheckboxItem> PantallasDisponibles { get; set; } = new();

        public bool HayPantallasDisponibles => PantallasDisponibles.Count > 0;

        public bool EsEdicion { get; set; }
    }
}
