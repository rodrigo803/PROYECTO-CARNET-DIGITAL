namespace CarnetDigital.Frontend.Models.Carreras
{
    public class CarreraDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int IdInstitucion { get; set; }
        public bool Activo { get; set; }
    }
}
