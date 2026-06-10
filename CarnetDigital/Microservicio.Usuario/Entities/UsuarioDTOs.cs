namespace Microservicio.Usuario.Entities
{
    public class UsuarioDTOs
    {
        public class UsuarioRegistroDto
        {
            public string Email { get; set; }
            public string NombreCompleto { get; set; }
            public string Identificacion { get; set; }
            public string Contrasena { get; set; }

            // ¡AQUÍ ESTÁ EL CAMBIO! Ahora son enteros (int)
            public int TipoIdentificacionId { get; set; }
            public int TipoUsuarioId { get; set; }
            public int RolId { get; set; }

            public string TipoIdentificacion { get; set; }
            public string TipoUsuario { get; set; }
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

        public class UsuarioActualizacionDto
        {
            public string Email { get; set; }
            public string NombreCompleto { get; set; }
            public string Identificacion { get; set; }
        }
    }
}
