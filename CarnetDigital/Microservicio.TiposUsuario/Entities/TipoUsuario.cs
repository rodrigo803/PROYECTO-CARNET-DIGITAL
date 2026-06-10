using System.ComponentModel.DataAnnotations;

namespace Microservicio.TiposUsuario.Entities
{
    public class TipoUsuario
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