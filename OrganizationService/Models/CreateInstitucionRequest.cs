namespace OrganizationService.Models
{
    public class CreateInstitucionRequest
    {
        public int Id { get; set; }   // ✅ ahora requerido
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Dominios { get; set; } = string.Empty;
    }
}
