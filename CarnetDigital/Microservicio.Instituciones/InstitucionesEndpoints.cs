using Microservicio.Instituciones.Entities;
using Microservicio.Instituciones.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Instituciones
{
    public static class InstitucionesEndpoints
    {
        public static void MapInstitucionesEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/Instituciones")
                .WithTags(nameof(Institucion))
                .RequireAuthorization();

            // GET /api/Instituciones
            group.MapGet("/", async ([FromServices] IInstitucionesService service) =>
            {
                return Results.Ok(await service.GetAllAsync());
            })
            .WithName("GetAllInstituciones")
            // AllowAnonymous: requerido por el autoregistro publico (Web16)
            .AllowAnonymous();
            

            // GET /api/Instituciones/{id}
            group.MapGet("/{id}", async ([FromServices] IInstitucionesService service, int id) =>
            {
                var institucion = await service.GetByIdAsync(id);
                return institucion is null
                    ? Results.NotFound(new { mensaje = $"No se encontró una institución con ID {id}" })
                    : Results.Ok(institucion);
            })
            .WithName("GetInstitucionById")
            // AllowAnonymous: requerido por el autoregistro publico (Web16) para validar catalogos sin sesion
            .AllowAnonymous();

            // POST /api/Instituciones
            group.MapPost("/", async ([FromServices] IInstitucionesService service,
                                     [FromServices] IBitacoraService bitacoraService,
                                     [FromServices] IHttpContextAccessor httpContextAccessor,
                                     [FromBody] Institucion institucion) =>
            {
                if (string.IsNullOrWhiteSpace(institucion.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });
                if (string.IsNullOrWhiteSpace(institucion.Email))
                    return Results.BadRequest(new { mensaje = "El email es requerido" });
                if (string.IsNullOrWhiteSpace(institucion.Telefono))
                    return Results.BadRequest(new { mensaje = "El teléfono es requerido" });
                if (institucion.Dominios == null || !institucion.Dominios.Any())
                    return Results.BadRequest(new { mensaje = "Debe especificar al menos un dominio" });

                institucion.Activo = true;
                foreach (var d in institucion.Dominios)
                    d.Activo = true;

                var created = await service.CreateAsync(institucion);

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Creó la institución '{created.Nombre}' (ID: {created.Id})", token);

                return Results.Created($"/api/Instituciones/{created.Id}", created);
            })
            .WithName("CreateInstitucion");

            // PUT /api/Instituciones/{id}
            group.MapPut("/{id}", async ([FromServices] IInstitucionesService service,
                                        [FromServices] IBitacoraService bitacoraService,
                                        [FromServices] IHttpContextAccessor httpContextAccessor,
                                        int id, [FromBody] Institucion institucion) =>
            {
                if (string.IsNullOrWhiteSpace(institucion.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });
                if (string.IsNullOrWhiteSpace(institucion.Email))
                    return Results.BadRequest(new { mensaje = "El email es requerido" });
                if (string.IsNullOrWhiteSpace(institucion.Telefono))
                    return Results.BadRequest(new { mensaje = "El teléfono es requerido" });
                if (institucion.Dominios == null || !institucion.Dominios.Any())
                    return Results.BadRequest(new { mensaje = "Debe especificar al menos un dominio" });

                var updated = await service.UpdateAsync(id, institucion);
                if (updated is null)
                    return Results.NotFound(new { mensaje = $"No se encontró una institución con ID {id}" });

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Modificó la institución '{updated.Nombre}' (ID: {updated.Id})", token);

                return Results.Ok(updated);
            })
            .WithName("UpdateInstitucion");

            // DELETE /api/Instituciones/{id}
            group.MapDelete("/{id}", async ([FromServices] IInstitucionesService service,
                                           [FromServices] IBitacoraService bitacoraService,
                                           [FromServices] IHttpContextAccessor httpContextAccessor,
                                           int id) =>
            {
                var institucion = await service.GetByIdAsync(id);
                if (institucion is null)
                    return Results.NotFound(new { mensaje = $"No se encontró una institución con ID {id}" });

                await service.DeleteAsync(id);

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Eliminó la institución '{institucion.Nombre}' (ID: {institucion.Id})", token);

                return Results.NoContent();
            })
            .WithName("DeleteInstitucion");
        }

        // Helper para obtener usuario y token del contexto HTTP
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