namespace Microservicio.TiposIdentificacion.DTOs
{
    public class BitacoraRequest
    {
        public int UsuarioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}