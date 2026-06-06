namespace Microservicio.Usuario.Entities
{
    public class UsuarioDTOs
    {
        public class UsuarioRegistroDto
        {
            public string Email { get; set; }
            public string NombreCompleto { get; set; }
            public string Identificacion { get; set; }
            public string TipoIdentificacion { get; set; }
            public string TipoUsuario { get; set; }
            public string Contrasena { get; set; }
            public string Rol { get; set; }                 // Se quitó el internal
            public int TipoIdentificacionId { get; set; }    // Se quitó el internal
            public int TipoUsuarioId { get; set; }           // Se quitó el internal
            public int RolId { get; set; }                  // Se quitó el internal
        }

        public class CambioEstadoDto
        {
            public string? Email { get; set; }
            public int EstadoId { get; set; }
        }

        public class FotografiaDto
        {
            public string? Email { get; set; }
            public string? FotoBase64 { get; set; }
        }
    }
}
