using System.ComponentModel.DataAnnotations;

namespace Microservicio.Usuario.Entities
{
    public class Area
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
