using System.ComponentModel.DataAnnotations;
using CarnetDigital.Frontend.Validation;

namespace CarnetDigital.Frontend.Models.Instituciones
{
    public class InstitucionFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [NotWhitespace]
        [MaxLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [NotWhitespace]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [MaxLength(200, ErrorMessage = "El correo electrónico no puede superar los 200 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [NotWhitespace]
        [OnlyDigits]
        [MaxLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        [Display(Name = "Dominios")]
        [Required(ErrorMessage = "Los dominios son obligatorios.")]
        [AlMenosUnDominio]
        public string DominiosTexto { get; set; } = string.Empty;
    }
}
