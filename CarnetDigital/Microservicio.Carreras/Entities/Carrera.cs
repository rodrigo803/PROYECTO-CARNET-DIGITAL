using System.ComponentModel.DataAnnotations;

namespace Microservicio.Carreras.Entities
{
    public class Carrera
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Director { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;

        // FK lógica 
        [Required]
        public int IdInstitucion { get; set; }
    }
}