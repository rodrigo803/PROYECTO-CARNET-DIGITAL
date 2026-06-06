namespace AuditService.Models
{
    public class CreateBitacoraRequest
    {
        public int UsuarioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}