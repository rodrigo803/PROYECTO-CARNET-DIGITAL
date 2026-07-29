using System.ComponentModel.DataAnnotations;
using AccessControlService.Entities;
using AccessControlService.Services;

namespace AccessControlService
{
    public static class PantallaEndpoints
    {
        public static void MapPantallaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/pantallas")
                              .WithTags("Pantallas")
                              .RequireAuthorization();

            // GET ALL
            group.MapGet("/", async (
                IPantallaService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor) =>
            {
                var data = await service.GetAllAsync();

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, "Consultó pantallas", token);

                return Results.Ok(data);
            });

            // GET BY ID
            group.MapGet("/{id}", async (
                IPantallaService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                string id) =>
            {
                var pantalla = await service.GetByIdAsync(id);

                if (pantalla == null)
                    return Results.NotFound();

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Consultó pantalla {id}", token);

                return Results.Ok(pantalla);
            });

            // CREATE
            group.MapPost("/", async (
                IPantallaService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                Pantalla pantalla) =>
            {
                var errores = ValidarPantalla(pantalla);
                if (errores.Count > 0)
                    return Results.BadRequest(new { message = "Datos inválidos", errores });

                var exists = await service.GetByIdAsync(pantalla.Id);

                if (exists != null)
                {
                    return Results.Conflict(
                        $"Ya existe una pantalla con ID '{pantalla.Id}'.");
                }

                var created = await service.CreateAsync(pantalla);

                if (!created)
                {
                    return Results.Problem(
                        "No fue posible crear la pantalla.");
                }

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Creó pantalla {pantalla.Id}", token);

                return Results.Created(
                    $"/pantallas/{pantalla.Id}",
                    pantalla);
            });

            // UPDATE
            group.MapPut("/{id}", async (
                IPantallaService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                string id,
                Pantalla pantalla) =>
            {
                if (id != pantalla.Id)
                {
                    return Results.BadRequest(
                        "El ID de la URL no coincide con el ID enviado.");
                }

                var errores = ValidarPantalla(pantalla);
                if (errores.Count > 0)
                    return Results.BadRequest(new { message = "Datos inválidos", errores });

                var exists = await service.GetByIdAsync(id);

                if (exists == null)
                    return Results.NotFound();

                var updated = await service.UpdateAsync(pantalla);

                if (!updated)
                {
                    return Results.Problem(
                        "No fue posible actualizar la pantalla.");
                }

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Actualizó pantalla {pantalla.Id}", token);

                return Results.NoContent();
            });

            // DELETE
            group.MapDelete("/{id}", async (
                IPantallaService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                string id) =>
            {
                var exists = await service.GetByIdAsync(id);

                if (exists == null)
                    return Results.NotFound();

                var deleted = await service.DeleteAsync(id);

                if (!deleted)
                {
                    return Results.Problem(
                        "No fue posible eliminar la pantalla.");
                }

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Eliminó pantalla {id}", token);

                return Results.NoContent();
            });
        }

        // Aplica en runtime las DataAnnotations de Pantalla (Nombre/Descripción: solo letras,
        // números y espacios), que antes estaban declaradas pero nunca se evaluaban.
        private static List<string> ValidarPantalla(Pantalla pantalla)
        {
            var context = new ValidationContext(pantalla);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(pantalla, context, results, validateAllProperties: true);

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
