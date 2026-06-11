namespace OrganizationService.Models
{
    
    public class CreateAreaRequest
    {
        public int Id { get; set; }   // ✅ requerido
        public string Nombre { get; set; } = string.Empty;
        public int InstitucionId { get; set; }
    }
}