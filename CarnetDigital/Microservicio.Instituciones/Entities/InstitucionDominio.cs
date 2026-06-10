using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservicio.Instituciones.Entities
{
    public class InstitucionDominio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Dominio { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;

        // FK
        [Required]
        public int IdInstitucion { get; set; }

        [ForeignKey("IdInstitucion")]
        public Institucion? Institucion { get; set; }
    }
}