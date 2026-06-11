namespace Microservicio.Usuario.Entities
{
    public class UsuarioArea
    {
        public string IdentificacionUsuario { get; set; }
        public int AreaId { get; set; }

        // Navegación
        public Usuario Usuario { get; set; }
        public Area Area { get; set; }
    }
}
