using Microservicio.TiposUsuario.Entities;
using Microservicio.TiposUsuario.Services;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.TiposUsuario
{
    public static class TiposUsuarioEndpoints
    {
        public static void MapTiposUsuarioEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/TiposUsuario")
                .WithTags(nameof(TipoUsuario))
                .RequireAuthorization();

            // GET /api/TiposUsuario
            group.MapGet("/", async ([FromServices] ITiposUsuarioService service) =>
            {
                return Results.Ok(await service.GetAllAsync());
            })
            .WithName("GetAllTiposUsuario");

            // GET /api/TiposUsuario/{id}
            group.MapGet("/{id}", async ([FromServices] ITiposUsuarioService service, int id) =>
            {
                var tipo = await service.GetByIdAsync(id);
                return tipo is null
                    ? Results.NotFound(new { mensaje = $"No se encontró un tipo de usuario con ID {id}" })
                    : Results.Ok(tipo);
            })
            .WithName("GetTipoUsuarioById")
            // AllowAnonymous: requerido por el autoregistro publico (Web16) para validar catalogos sin sesion
            .AllowAnonymous();

            // POST /api/TiposUsuario
            group.MapPost("/", async ([FromServices] ITiposUsuarioService service,
                                     [FromServices] IBitacoraService bitacoraService,
                                     [FromServices] IHttpContextAccessor httpContextAccessor,
                                     [FromBody] TipoUsuario tipo) =>
            {
                if (string.IsNullOrWhiteSpace(tipo.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });

                tipo.Activo = true;
                var created = await service.CreateAsync(tipo);

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Creó el tipo de usuario '{created.Nombre}' (ID: {created.Id})", token);

                return Results.Created($"/api/TiposUsuario/{created.Id}", created);
            })
            .WithName("CreateTipoUsuario");

            // PUT /api/TiposUsuario/{id}
            group.MapPut("/{id}", async ([FromServices] ITiposUsuarioService service,
                                        [FromServices] IBitacoraService bitacoraService,
                                        [FromServices] IHttpContextAccessor httpContextAccessor,
                                        int id, [FromBody] TipoUsuario tipo) =>
            {
                if (string.IsNullOrWhiteSpace(tipo.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });

                var updated = await service.UpdateAsync(id, tipo);
                if (updated is null)
                    return Results.NotFound(new { mensaje = $"No se encontró un tipo de usuario con ID {id}" });

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Modificó el tipo de usuario '{updated.Nombre}' (ID: {updated.Id})", token);

                return Results.Ok(updated);
            })
            .WithName("UpdateTipoUsuario");

            // DELETE /api/TiposUsuario/{id}
            group.MapDelete("/{id}", async ([FromServices] ITiposUsuarioService service,
                                           [FromServices] IBitacoraService bitacoraService,
                                           [FromServices] IHttpContextAccessor httpContextAccessor,
                                           int id) =>
            {
                var tipo = await service.GetByIdAsync(id);
                if (tipo is null)
                    return Results.NotFound(new { mensaje = $"No se encontró un tipo de usuario con ID {id}" });

                await service.DeleteAsync(id);

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Eliminó el tipo de usuario '{tipo.Nombre}' (ID: {tipo.Id})", token);

                return Results.NoContent();
            })
            .WithName("DeleteTipoUsuario");
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