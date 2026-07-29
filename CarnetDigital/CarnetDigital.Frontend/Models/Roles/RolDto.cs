namespace CarnetDigital.Frontend.Models.Roles
{
    public class RolDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public List<string> Pantallas { get; set; } = new();
    }
}
