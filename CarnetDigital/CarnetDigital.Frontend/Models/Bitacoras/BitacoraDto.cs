namespace CarnetDigital.Frontend.Models.Bitacoras
{
    public class BitacoraDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}
