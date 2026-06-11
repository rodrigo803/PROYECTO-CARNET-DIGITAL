using System.ComponentModel.DataAnnotations;
using System;

namespace Microservicio.Usuario.Entities
{
    public class Usuario
    {
        [Key]
        [EmailAddress]
        public string Email { get; set; }

        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string ContrasenaEncriptada { get; set; }

        // SRV13: Fotografía
        public string FotografiaBase64 { get; set; }

        // SRV11: Autoregistro
        public string TokenConfirmacion { get; set; }
        public DateTime? FechaExpiracionToken { get; set; }

        // Ids Numéricos (Esto quita los errores de EstadoId, TipoUsuarioId, etc.)
        public int EstadoId { get; set; }
        public int TipoIdentificacionId { get; set; }
        public int TipoUsuarioId { get; set; }
        public int RolId { get; set; }
        public string TipoIdentificacion { get; set; }
        public string TipoUsuario { get; set; }
    }

    public class EstadoUsuario
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
