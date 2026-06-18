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