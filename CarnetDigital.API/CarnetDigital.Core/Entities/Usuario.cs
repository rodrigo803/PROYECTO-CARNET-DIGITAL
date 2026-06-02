using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CarnetDigital.Core.Entities
{
    public class Usuario
    {
        // El documento indica que el Email será la identificación única 
        [Key]
        [EmailAddress(ErrorMessage = "Debe validarse el formato del email")]
        public string? Email { get; set; }

        [Required]
        public string? TipoIdentificacion { get; set; }

        [Required]
        public string? Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre completo no puede ser vacío ni espacios en blanco")]
        public string? NombreCompleto { get; set; }

        [Required]
        public string? ContrasenaEncriptada { get; set; } // Debe almacenarse encriptada 

        [Required]
        public string? TipoUsuario { get; set; } // Funcionario, estudiante o administrador 

        public int TipoIdentificacionId { get; set; }
        public int TipoUsuarioId { get; set; }
        public int RolId { get; set; }
        public string? TokenConfirmacion { get; set; }
        public DateTime? FechaExpiracionToken { get; set; }

        // Estado para la historia SRV12 (activo/inactivo) [cite: 100]
        [Required]
        public string Estado { get; set; } = "activo";

        //Fotografía para la historia SRV13 (formato Base64) [cite: 100]
        public string? FotografiaBase64 { get; set; }

        // Relaciones (Estos serían otras entidades o tablas relacionales en SQL Server)
        // Si es estudiante tendrá carreras, si es funcionario tendrá áreas [cite: 98]
        public List<string> InstitucionesAsociadas { get; set; } = new List<string>();
        public List<string> CarrerasAsociadas { get; set; } = new List<string>();
        public List<string> AreasAsociadas { get; set; } = new List<string>();
        public List<string> Telefonos { get; set; } = new List<string>(); // No son obligatorios
        public List<int> InstitucionesIds { get; set; }
        public List<int> CarrerasIds { get; set; }
    }
}
