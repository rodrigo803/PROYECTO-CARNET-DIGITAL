using System.ComponentModel.DataAnnotations;
using System;

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

        // Relaciones (El usuario puede tener más de una)
        public List<string> Telefonos { get; set; } = new List<string>();
        public List<int> InstitucionesIds { get; set; } = new List<int>();
        public List<int> CarrerasIds { get; set; } = new List<int>();
        public List<int> AreasIds { get; set; } = new List<int>();

        // Esto le indica al código que un Usuario posee "listas" de datos en otras tablas

        public virtual ICollection<UsuarioTelefono> Telefonos { get; set; } = new List<UsuarioTelefono>();
        public virtual ICollection<UsuarioCarrera> CarrerasAsociadas { get; set; } = new List<UsuarioCarrera>();
        public virtual ICollection<UsuarioArea> AreasAsociadas { get; set; } = new List<UsuarioArea>();
        public virtual ICollection<UsuarioInstitucion> InstitucionesAsociadas { get; set; } = new List<UsuarioInstitucion>();

    }

    public class EstadoUsuario
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }


}
