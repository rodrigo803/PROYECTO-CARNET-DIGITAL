namespace Microservicio.Usuario.Entities
{
    public class UsuarioCarrera
    {
        public string IdentificacionUsuario { get; set; }
        public int CarreraId { get; set; }

        // Navegación
        public Usuario Usuario { get; set; }
        public Carrera Carrera { get; set; }
    }
}
