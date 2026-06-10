using System.ComponentModel.DataAnnotations;

namespace Microservicio.Instituciones.Entities
{
    public class Institucion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;

        // Relación: una Institución tiene muchos Dominios
        public ICollection<InstitucionDominio> Dominios { get; set; } = new List<InstitucionDominio>();
    }
}