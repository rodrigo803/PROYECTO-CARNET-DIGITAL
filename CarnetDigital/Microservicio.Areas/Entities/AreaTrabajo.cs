using System.ComponentModel.DataAnnotations;

namespace Microservicio.Areas.Entities
{
    public class AreaTrabajo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;

        [Required]
        public int IdInstitucion { get; set; }
    }
}