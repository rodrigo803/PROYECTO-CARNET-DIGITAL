using System.ComponentModel.DataAnnotations;

namespace ParameterService.Entities
{
    public class Parametro
    {
        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^[A-Z]+$", ErrorMessage = "Solo letras mayúsculas")]
        public string Id { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Valor { get; set; } = null!;
    }
}