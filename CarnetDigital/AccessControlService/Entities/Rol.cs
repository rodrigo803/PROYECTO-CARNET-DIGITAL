using System.ComponentModel.DataAnnotations;

namespace AccessControlService.Entities
{
    public class Rol
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$")]
        public string Nombre { get; set; } = null!;

        public List<string> Pantallas { get; set; } = new();
    }
}