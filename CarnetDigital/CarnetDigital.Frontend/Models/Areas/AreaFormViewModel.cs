using System.ComponentModel.DataAnnotations;
using CarnetDigital.Frontend.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarnetDigital.Frontend.Models.Areas
{
    public class AreaFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [NotWhitespace]
        [MaxLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Institución")]
        [Required(ErrorMessage = "Debe seleccionar una institución")]
        public int? IdInstitucion { get; set; }

        public List<SelectListItem> Instituciones { get; set; } = new();

        public bool HayInstitucionesActivas => Instituciones.Count > 0;
    }
}
