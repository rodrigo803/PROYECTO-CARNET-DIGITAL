namespace Microservicio.Areas.Entities
{
    public class BitacoraRequest
    {
        public int UsuarioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}