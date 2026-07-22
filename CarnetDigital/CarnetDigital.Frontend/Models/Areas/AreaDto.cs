namespace CarnetDigital.Frontend.Models.Areas
{
    public class AreaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdInstitucion { get; set; }
        public bool Activo { get; set; }
    }
}
