using System.ComponentModel.DataAnnotations;
using CarnetDigital.Frontend.Validation;

namespace CarnetDigital.Frontend.Models.Pantallas
{
    public class PantallaFormViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "El Id es obligatorio.")]
        [NotWhitespace]
        [MaxLength(50, ErrorMessage = "El Id no puede superar los 50 caracteres.")]
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [NotWhitespace]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "El nombre debe contener solo letras y espacios.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [NotWhitespace]
        [MaxLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9 ]+$", ErrorMessage = "La descripción debe contener solo letras, números y espacios.")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Ruta")]
        [Required(ErrorMessage = "La ruta es obligatoria.")]
        [NotWhitespace]
        [MaxLength(200, ErrorMessage = "La ruta no puede superar los 200 caracteres.")]
        public string Ruta { get; set; } = string.Empty;

        public bool EsEdicion { get; set; }
    }
}
