namespace CarnetDigital.Frontend.Models.Instituciones
{
    public class InstitucionDetalleDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public List<DominioDto> Dominios { get; set; } = new();
    }
}
