using ParameterService.Services;
using ParameterService.Entities;

namespace ParameterService
{
    public static class ParametroEndpoints
    {
        public static void MapParametroEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/parametro")
                .WithTags("Parametro")
                .RequireAuthorization();

            //  GET ALL  200 OK
            group.MapGet("/", async (
                IParametroService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor) =>
            {
                var result = await service.GetAllAsync();

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, "Consultó parámetros", token);

                return Results.Ok(result); // 200
            });


            //  GET BY ID  200 / 404
            group.MapGet("/{id}", async (
                IParametroService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                string id) =>
            {
                var result = await service.GetByIdAsync(id);

                if (result is null)
                    return Results.NotFound(); // 404

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Consultó parámetro {id}", token);

                return Results.Ok(result); // 200
            });


            //  CREATE  201 / 400 / 409
            group.MapPost("/", async (
                IParametroService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                Parametro p) =>
            {
                if (string.IsNullOrWhiteSpace(p.Id) || string.IsNullOrWhiteSpace(p.Valor))
                    return Results.BadRequest(new { message = "Todos los datos son requeridos" }); // 400

                var exists = await service.GetByIdAsync(p.Id);
                if (exists != null)
                    return Results.Conflict(new { message = $"Ya existe el parámetro {p.Id}" }); // 409

                var created = await service.CreateAsync(p);

                if (!created)
                    return Results.Problem("No se pudo crear"); // 500

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Creó parámetro {p.Id}", token);

                return Results.Created($"/parametro/{p.Id}", p); // 201
            });


            // UPDATE  204 / 400 / 404
            group.MapPut("/{id}", async (
                IParametroService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                string id,
                Parametro p) =>
            {
                if (id != p.Id)
                    return Results.BadRequest(new { message = "El ID de la ruta y el del cuerpo no coinciden" }); // 400

                if (string.IsNullOrWhiteSpace(p.Valor))
                    return Results.BadRequest(new { message = "Valor es requerido" }); // 400

                var exists = await service.GetByIdAsync(id);
                if (exists is null)
                    return Results.NotFound(); // 404

                var updated = await service.UpdateAsync(p);

                if (!updated)
                    return Results.Problem("No se pudo actualizar"); // 500

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Actualizó parámetro {p.Id}", token);

                return Results.NoContent(); // ✅ 204 (REST correcto)
            });


            // DELETE  204 / 404
            group.MapDelete("/{id}", async (
                IParametroService service,
                AuditClient audit,
                IHttpContextAccessor httpContextAccessor,
                string id) =>
            {
                var exists = await service.GetByIdAsync(id);
                if (exists is null)
                    return Results.NotFound(); // 404

                var deleted = await service.DeleteAsync(id);

                if (!deleted)
                    return Results.Problem("No se pudo eliminar"); // 500

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await audit.LogAsync(usuarioId.Value, $"Eliminó parámetro {id}", token);

                return Results.NoContent(); // ✅ 204
            });
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