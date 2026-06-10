namespace Microservicio.Carreras.DTOs
{
    public class CrearCarreraDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int IdInstitucion { get; set; }
    }
}