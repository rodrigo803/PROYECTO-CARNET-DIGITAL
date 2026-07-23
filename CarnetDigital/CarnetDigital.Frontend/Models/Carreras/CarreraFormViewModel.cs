using System.ComponentModel.DataAnnotations;
using CarnetDigital.Frontend.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarnetDigital.Frontend.Models.Carreras
{
    public class CarreraFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [NotWhitespace]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Director")]
        [NotWhitespace]
        [MaxLength(200)]
        public string Director { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [NotWhitespace]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        [NotWhitespace]
        [OnlyDigits]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [Display(Name = "Institución")]
        [Required(ErrorMessage = "Debe seleccionar una institución")]
        public int? IdInstitucion { get; set; }

        public List<SelectListItem> Instituciones { get; set; } = new();

        public bool HayInstitucionesActivas => Instituciones.Count > 0;
    }
}
