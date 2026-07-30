using Microservicio.Areas.Entities;
using Microservicio.Areas.Services;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Areas
{
    public static class AreasEndpoints
    {
        public static void MapAreasEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/Areas")
                .WithTags(nameof(AreaTrabajo))
                .RequireAuthorization();

            // GET /api/Areas
            group.MapGet("/", async ([FromServices] IAreasService service) =>
            {
                return Results.Ok(await service.GetAllAsync());
            })
            .WithName("GetAllAreas")
            // AllowAnonymous: requerido por el autoregistro publico (Web16)
            .AllowAnonymous();

            // GET /api/Areas/{id}
            group.MapGet("/{id}", async ([FromServices] IAreasService service, int id) =>
            {
                var area = await service.GetByIdAsync(id);
                return area is null
                    ? Results.NotFound(new { mensaje = $"No se encontró un área de trabajo con ID {id}" })
                    : Results.Ok(area);
            })
            .WithName("GetAreaById")
            // AllowAnonymous: requerido por el autoregistro publico (Web16) para validar catalogos sin sesion
            .AllowAnonymous();

            // POST /api/Areas
            group.MapPost("/", async ([FromServices] IAreasService service,
                                     [FromServices] IBitacoraService bitacoraService,
                                     [FromServices] IInstitucionesValidator institucionesValidator,
                                     [FromServices] IHttpContextAccessor httpContextAccessor,
                                     [FromBody] AreaTrabajo area) =>
            {
                if (string.IsNullOrWhiteSpace(area.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (token == null) return Results.Unauthorized();

                var institucion = await institucionesValidator.ObtenerInstitucionAsync(area.IdInstitucion, token);
                if (institucion == null)
                    return Results.BadRequest(new { mensaje = $"No existe una institución activa con ID {area.IdInstitucion}" });

                area.Activo = true;
                var created = await service.CreateAsync(area);

                if (usuarioId.HasValue)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Creó el área de trabajo '{created.Nombre}' (ID: {created.Id})", token);

                return Results.Created($"/api/Areas/{created.Id}", created);
            })
            .WithName("CreateArea");

            // PUT /api/Areas/{id}
            group.MapPut("/{id}", async ([FromServices] IAreasService service,
                                        [FromServices] IBitacoraService bitacoraService,
                                        [FromServices] IInstitucionesValidator institucionesValidator,
                                        [FromServices] IHttpContextAccessor httpContextAccessor,
                                        int id, [FromBody] AreaTrabajo area) =>
            {
                if (string.IsNullOrWhiteSpace(area.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (token == null) return Results.Unauthorized();

                var institucion = await institucionesValidator.ObtenerInstitucionAsync(area.IdInstitucion, token);
                if (institucion == null)
                    return Results.BadRequest(new { mensaje = $"No existe una institución activa con ID {area.IdInstitucion}" });

                var updated = await service.UpdateAsync(id, area);
                if (updated is null)
                    return Results.NotFound(new { mensaje = $"No se encontró un área de trabajo con ID {id}" });

                if (usuarioId.HasValue)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Modificó el área de trabajo '{updated.Nombre}' (ID: {updated.Id})", token);

                return Results.Ok(updated);
            })
            .WithName("UpdateArea");

            // DELETE /api/Areas/{id}
            group.MapDelete("/{id}", async ([FromServices] IAreasService service,
                                           [FromServices] IBitacoraService bitacoraService,
                                           [FromServices] IHttpContextAccessor httpContextAccessor,
                                           int id) =>
            {
                var area = await service.GetByIdAsync(id);
                if (area is null)
                    return Results.NotFound(new { mensaje = $"No se encontró un área de trabajo con ID {id}" });

                await service.DeleteAsync(id);

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Eliminó el área de trabajo '{area.Nombre}' (ID: {area.Id})", token);

                return Results.NoContent();
            })
            .WithName("DeleteArea");
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