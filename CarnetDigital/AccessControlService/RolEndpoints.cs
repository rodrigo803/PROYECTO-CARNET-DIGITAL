using System.ComponentModel.DataAnnotations;
using AccessControlService.Services;
using AccessControlService.Entities;

namespace AccessControlService
{
    public static class RolEndpoints
    {
        public static void MapRolEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/rol")
                              .WithTags("Roles")
                              .RequireAuthorization();

            // GET ALL
            group.MapGet("/", async (
                IRolService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor) =>
            {
                var data = await service.GetAllAsync();

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, "Consultó roles", token);

                return Results.Ok(data);
            });

            // GET BY ID
            group.MapGet("/{id}", async (
                IRolService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                string id) =>
            {
                var rol = await service.GetByIdAsync(id);

                if (rol == null)
                    return Results.NotFound();

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Consultó rol {id}", token);

                return Results.Ok(rol);
            });

            // CREATE
            group.MapPost("/", async (
                IRolService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                Rol rol) =>
            {
                var errores = ValidarRol(rol);
                if (errores.Count > 0)
                    return Results.BadRequest(new { message = "Datos inválidos", errores });

                var exists = await service.GetByIdAsync(rol.Id);

                if (exists != null)
                    return Results.Conflict(
                        $"Ya existe un rol con ID '{rol.Id}'.");

                var created = await service.CreateAsync(rol);

                if (!created)
                    return Results.Problem(
                        "No fue posible crear el rol.");

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Creó rol {rol.Id}", token);

                return Results.Created(
                    $"/rol/{rol.Id}",
                    rol);
            });

            // UPDATE
            group.MapPut("/{id}", async (
                IRolService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                string id,
                Rol rol) =>
            {
                if (id != rol.Id)
                {
                    return Results.BadRequest(
                        "El ID de la URL no coincide con el ID enviado.");
                }

                var errores = ValidarRol(rol);
                if (errores.Count > 0)
                    return Results.BadRequest(new { message = "Datos inválidos", errores });

                var exists = await service.GetByIdAsync(id);

                if (exists == null)
                    return Results.NotFound();

                var updated = await service.UpdateAsync(rol);

                if (!updated)
                    return Results.Problem(
                        "No fue posible actualizar el rol.");

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Actualizó rol {rol.Id}", token);

                return Results.NoContent();
            });

            // DELETE
            group.MapDelete("/{id}", async (
                IRolService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                string id) =>
            {
                var exists = await service.GetByIdAsync(id);

                if (exists == null)
                    return Results.NotFound();

                var deleted = await service.DeleteAsync(id);

                if (!deleted)
                    return Results.Problem(
                        "No fue posible eliminar el rol.");

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Eliminó rol {id}", token);

                return Results.NoContent();
            });
        }

        // Aplica en runtime las DataAnnotations de Rol (Nombre: solo letras, números y espacios),
        // que antes estaban declaradas pero nunca se evaluaban.
        private static List<string> ValidarRol(Rol rol)
        {
            var context = new ValidationContext(rol);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(rol, context, results, validateAllProperties: true);

            return results.Select(r => r.ErrorMessage ?? "Dato inválido").ToList();
        }

        private static (int? usuarioId, string? token) GetAuthInfo(IHttpContextAccessor httpContextAccessor)
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null) return (null, null);

            var uidClaim = context.User.FindFirst("uid");
            int? usuarioId = null;
            if (uidClaim != null && int.TryParse(uidClaim.Value, out int uid))
                usuarioId = uid;

            var authHeader = context.Request.Headers["Authorization"].ToString();
            string? token = null;
            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
                token = authHeader.Substring("Bearer ".Length).Trim();

            return (usuarioId, token);
        }
    }
}
