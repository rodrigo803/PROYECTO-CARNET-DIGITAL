namespace Microservicio.Usuario.Entities
{
    public class UsuarioInstitucion
    {
        public string IdentificacionUsuario { get; set; }
        public int InstitucionId { get; set; }

        // Navegación
        public Usuario Usuario { get; set; }
        public Institucion Institucion { get; set; }
    }
}
