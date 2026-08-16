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

            /// <summary>
            /// Relaciones con los otros microservicios
            /// </summary>
            public List<int> InstitucionesIds { get; set; } = new List<int>();
            public List<int> CarrerasIds { get; set; } = new List<int>();
            public List<int> AreasIds { get; set; } = new List<int>();

        }

        public class CambioEstadoDto
        {
            public string? Identificacion { get; set; }
            public int EstadoId { get; set; }
        }

        public class FotografiaDto
        {
            public string? Identificacion { get; set; }
            public string? FotoBase64 { get; set; }
        }

        public class UsuarioActualizacionDto
        {
            public string Email { get; set; }
            public string NombreCompleto { get; set; }
            public string Identificacion { get; set; }
        }

        public class PerfilUsuarioDto
        {
            public string Identificacion { get; set; }
            public string Email { get; set; }
            public string NombreCompleto { get; set; }
            public int TipoIdentificacionId { get; set; }
            public string TipoIdentificacion { get; set; }
            public int TipoUsuarioId { get; set; }
            public string TipoUsuario { get; set; }
            public List<int> InstitucionesIds { get; set; } = new List<int>();
            public List<int> CarrerasIds { get; set; } = new List<int>();
            public List<int> AreasIds { get; set; } = new List<int>();
            public string CarreraOArea { get; set; }
            public bool TieneFotografia { get; set; }
        }

        public class UsuarioResumenDto
        {
            public string Identificacion { get; set; }
            public string Email { get; set; }
            public string NombreCompleto { get; set; }
            public int EstadoId { get; set; }
            public int TipoIdentificacionId { get; set; }
            public int TipoUsuarioId { get; set; }
            public int RolId { get; set; }
            public string TipoIdentificacion { get; set; }
            public string TipoUsuario { get; set; }
            public List<int> InstitucionesIds { get; set; } = new List<int>();
            public List<int> CarrerasIds { get; set; } = new List<int>();
            public List<int> AreasIds { get; set; } = new List<int>();
        }
    }
}
