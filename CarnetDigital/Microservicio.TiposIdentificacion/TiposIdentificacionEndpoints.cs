using Microservicio.TiposIdentificacion.Entities;
using Microservicio.TiposIdentificacion.Services;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.TiposIdentificacion
{
    public static class TiposIdentificacionEndpoints
    {
        public static void MapTiposIdentificacionEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/TiposIdentificacion")
                .WithTags(nameof(TipoIdentificacion))
                .RequireAuthorization(); // Mantenemos el grupo protegido por defecto

            // GET /api/TiposIdentificacion (ABIERTO)
            group.MapGet("/", async ([FromServices] ITiposIdentificacionService service) =>
            {
                return Results.Ok(await service.GetAllAsync());
            })
            .WithName("GetAllTiposIdentificacion")
            .AllowAnonymous(); // <--- AGREGAR ESTO: Permite acceso sin token

            // GET /api/TiposIdentificacion/{id} (ABIERTO)
            group.MapGet("/{id}", async ([FromServices] ITiposIdentificacionService service, int id) =>
            {
                var tipo = await service.GetByIdAsync(id);
                return tipo is null
                    ? Results.NotFound(new { mensaje = $"No se encontró un tipo de identificación con ID {id}" })
                    : Results.Ok(tipo);
            })
            .WithName("GetTipoIdentificacionById")
            .AllowAnonymous(); // <--- AGREGAR ESTO: Permite acceso sin token

            // POST /api/TiposIdentificacion
            group.MapPost("/", async ([FromServices] ITiposIdentificacionService service,
                                     [FromServices] IBitacoraService bitacoraService,
                                     [FromServices] IHttpContextAccessor httpContextAccessor,
                                     [FromBody] TipoIdentificacion tipo) =>
            {
                if (string.IsNullOrWhiteSpace(tipo.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });

                tipo.Activo = true;
                var created = await service.CreateAsync(tipo);

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Creó el tipo de identificación '{created.Nombre}' (ID: {created.Id})", token);

                return Results.Created($"/api/TiposIdentificacion/{created.Id}", created);
            })
            .WithName("CreateTipoIdentificacion");

            // PUT /api/TiposIdentificacion/{id}
            group.MapPut("/{id}", async ([FromServices] ITiposIdentificacionService service,
                                        [FromServices] IBitacoraService bitacoraService,
                                        [FromServices] IHttpContextAccessor httpContextAccessor,
                                        int id, [FromBody] TipoIdentificacion tipo) =>
            {
                if (string.IsNullOrWhiteSpace(tipo.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });

                var updated = await service.UpdateAsync(id, tipo);
                if (updated is null)
                    return Results.NotFound(new { mensaje = $"No se encontró un tipo de identificación con ID {id}" });

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Modificó el tipo de identificación '{updated.Nombre}' (ID: {updated.Id})", token);

                return Results.Ok(updated);
            })
            .WithName("UpdateTipoIdentificacion");

            // DELETE /api/TiposIdentificacion/{id}
            group.MapDelete("/{id}", async ([FromServices] ITiposIdentificacionService service,
                                           [FromServices] IBitacoraService bitacoraService,
                                           [FromServices] IHttpContextAccessor httpContextAccessor,
                                           int id) =>
            {
                var tipo = await service.GetByIdAsync(id);
                if (tipo is null)
                    return Results.NotFound(new { mensaje = $"No se encontró un tipo de identificación con ID {id}" });

                await service.DeleteAsync(id);

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Eliminó el tipo de identificación '{tipo.Nombre}' (ID: {tipo.Id})", token);

                return Results.NoContent();
            })
            .WithName("DeleteTipoIdentificacion");
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