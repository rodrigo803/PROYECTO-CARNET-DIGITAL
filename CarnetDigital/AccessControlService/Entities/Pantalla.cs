using System.ComponentModel.DataAnnotations;

namespace AccessControlService.Entities
{
    public class Pantalla
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Solo letras, números y espacios")]
        public string Nombre { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Solo letras, números y espacios")]
        public string Descripcion { get; set; } = null!;

        [Required]
        public string Ruta { get; set; } = null!;
    }
}