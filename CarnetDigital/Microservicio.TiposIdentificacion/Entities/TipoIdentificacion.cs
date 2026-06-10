using System.ComponentModel.DataAnnotations;

namespace Microservicio.TiposIdentificacion.Entities
{
    public class TipoIdentificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;
    }
}