namespace UserService.Models
{
    public class CreateUsuarioRequest
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public int TipoUsuarioId { get; set; }
        public int TipoIdentificacionId { get; set; }
        public int InstitucionId { get; set; }
        public int CarreraId { get; set; }
        public int AreaId { get; set; }
        public int RolId { get; set; }
    }
}