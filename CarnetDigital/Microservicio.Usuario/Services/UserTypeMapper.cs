namespace Microservicio.Usuario.Services
{
    /// <summary>
    /// Traduce el nombre del catálogo de TiposUsuario al valor de UsersAuth.UserType.
    /// "Administrador" se mapea a "Admin" para no romper las cuentas ya existentes;
    /// el resto de los tipos (Estudiante, Funcionario, Visitante, Auditor) pasan tal cual.
    /// </summary>
    public static class UserTypeMapper
    {
        public static string ToUserType(string nombreTipoUsuarioCatalogo) => nombreTipoUsuarioCatalogo switch
        {
            "Administrador" => "Admin",
            _ => nombreTipoUsuarioCatalogo
        };
    }
}
