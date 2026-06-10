namespace Microservicio.Carreras.DTOs
{
    public class CarreraDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int IdInstitucion { get; set; }
        public string NombreInstitucion { get; set; } = string.Empty;
    }
}