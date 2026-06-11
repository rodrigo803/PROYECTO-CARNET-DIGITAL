using System.ComponentModel.DataAnnotations;

namespace Microservicio.Usuario.Entities
{
    public class UsuarioTelefono
    {
        [Key]
        public int Id { get; set; }
        public string IdentificacionUsuario { get; set; }
        public string Numero { get; set; }

        // Propiedad de navegación
        public Usuario Usuario { get; set; }
    }
}
