using System.ComponentModel.DataAnnotations;

namespace Microservicio.Usuario.Entities
{
    public class Institucion
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
