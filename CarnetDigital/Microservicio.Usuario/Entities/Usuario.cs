using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservicio.Usuario.Entities
{
    public class Usuario
    {
        [EmailAddress]
        public string Email { get; set; }

        [Key]
        public string Identificacion { get; set; }

        public string NombreCompleto { get; set; }
        public string ContrasenaEncriptada { get; set; }

        // ¡AQUÍ ESTÁ EL CAMBIO! Agregamos el '?' para permitir nulos
        public string? FotografiaBase64 { get; set; }

        public string? TokenConfirmacion { get; set; }
        public DateTime? FechaExpiracionToken { get; set; }

        public int EstadoId { get; set; }
        public int TipoIdentificacionId { get; set; }
        public int TipoUsuarioId { get; set; }
        public int RolId { get; set; }

        // También es buena idea permitir nulos aquí por si acaso

        [NotMapped]
        public string? TipoIdentificacion { get; set; }
        [NotMapped]
        public string? TipoUsuario { get; set; }

        /// <summary>
        /// Relaciones con los otros microservicios
        /// </summary>
        public List<int> InstitucionesIds { get; set; } = new List<int>();
        public List<int> CarrerasIds { get; set; } = new List<int>();
        public List<int> AreasIds { get; set; } = new List<int>();
    }

    public class EstadoUsuario
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }


}
