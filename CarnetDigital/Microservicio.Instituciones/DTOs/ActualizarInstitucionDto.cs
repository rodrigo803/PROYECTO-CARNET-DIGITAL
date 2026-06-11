namespace Microservicio.Instituciones.DTOs
{
    public class ActualizarInstitucionDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public List<string> Dominios { get; set; } = new List<string>();
    }
}