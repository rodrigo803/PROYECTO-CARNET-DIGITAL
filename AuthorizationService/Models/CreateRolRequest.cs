namespace AuthorizationService.Models
{
    public class CreateRolRequest
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public List<int> Pantallas { get; set; } = new();
    }
}