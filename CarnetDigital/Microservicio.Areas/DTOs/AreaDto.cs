namespace Microservicio.Areas.DTOs
{
    public class AreaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdInstitucion { get; set; }
        public string NombreInstitucion { get; set; } = string.Empty;
    }
}