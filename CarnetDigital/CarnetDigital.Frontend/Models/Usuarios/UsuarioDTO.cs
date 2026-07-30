namespace CarnetDigital.Frontend.Models.Usuarios
{
    public class UsuarioDTO
    {
        public string Identificacion { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Contrasena { get; set; }

        public int? TipoIdentificacionId { get; set; }
        public int? TipoUsuarioId { get; set; }
        public int? RolId { get; set; }

        public string? TipoIdentificacion { get; set; }
        public string? TipoUsuario { get; set; }
        public int EstadoId { get; set; }

        // Nombres resueltos para WEB9 (el microservicio solo devuelve IDs;
        // el Frontend los resuelve con sus propios clientes tipados)
        public string? Institucion { get; set; }
        public string? CarreraOArea { get; set; }

        public List<int> InstitucionesIds { get; set; } = new List<int>();
        public List<int> CarrerasIds { get; set; } = new List<int>();
        public List<int> AreasIds { get; set; } = new List<int>();
    }
}
