using Microservicio.Carreras.Entities;
using Microservicio.Carreras.Services;
using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Carreras
{
    public static class CarrerasEndpoints
    {
        public static void MapCarrerasEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/Carreras")
                .WithTags(nameof(Carrera))
                .RequireAuthorization();

            // GET /api/Carreras
            group.MapGet("/", async ([FromServices] ICarrerasService service) =>
            {
                return Results.Ok(await service.GetAllAsync());
            })
            .WithName("GetAllCarreras");

            // GET /api/Carreras/{id}
            group.MapGet("/{id}", async ([FromServices] ICarrerasService service, int id) =>
            {
                var carrera = await service.GetByIdAsync(id);
                return carrera is null
                    ? Results.NotFound(new { mensaje = $"No se encontró una carrera con ID {id}" })
                    : Results.Ok(carrera);
            })
            .WithName("GetCarreraById");

            // POST /api/Carreras
            group.MapPost("/", async ([FromServices] ICarrerasService service,
                                     [FromServices] IBitacoraService bitacoraService,
                                     [FromServices] IInstitucionesValidator institucionesValidator,
                                     [FromServices] IHttpContextAccessor httpContextAccessor,
                                     [FromBody] Carrera carrera) =>
            {
                if (string.IsNullOrWhiteSpace(carrera.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });
                if (string.IsNullOrWhiteSpace(carrera.Director))
                    return Results.BadRequest(new { mensaje = "El director es requerido" });
                if (string.IsNullOrWhiteSpace(carrera.Email))
                    return Results.BadRequest(new { mensaje = "El email es requerido" });
                if (string.IsNullOrWhiteSpace(carrera.Telefono))
                    return Results.BadRequest(new { mensaje = "El teléfono es requerido" });

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (token == null) return Results.Unauthorized();

                // Validar institución vía HTTP al SRV2
                var institucion = await institucionesValidator.ObtenerInstitucionAsync(carrera.IdInstitucion, token);
                if (institucion == null)
                    return Results.BadRequest(new { mensaje = $"No existe una institución activa con ID {carrera.IdInstitucion}" });

                carrera.Activo = true;
                var created = await service.CreateAsync(carrera);

                if (usuarioId.HasValue)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Creó la carrera '{created.Nombre}' (ID: {created.Id})", token);

                return Results.Created($"/api/Carreras/{created.Id}", created);
            })
            .WithName("CreateCarrera");

            // PUT /api/Carreras/{id}
            group.MapPut("/{id}", async ([FromServices] ICarrerasService service,
                                        [FromServices] IBitacoraService bitacoraService,
                                        [FromServices] IInstitucionesValidator institucionesValidator,
                                        [FromServices] IHttpContextAccessor httpContextAccessor,
                                        int id, [FromBody] Carrera carrera) =>
            {
                if (string.IsNullOrWhiteSpace(carrera.Nombre))
                    return Results.BadRequest(new { mensaje = "El nombre es requerido" });
                if (string.IsNullOrWhiteSpace(carrera.Director))
                    return Results.BadRequest(new { mensaje = "El director es requerido" });
                if (string.IsNullOrWhiteSpace(carrera.Email))
                    return Results.BadRequest(new { mensaje = "El email es requerido" });
                if (string.IsNullOrWhiteSpace(carrera.Telefono))
                    return Results.BadRequest(new { mensaje = "El teléfono es requerido" });

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (token == null) return Results.Unauthorized();

                var institucion = await institucionesValidator.ObtenerInstitucionAsync(carrera.IdInstitucion, token);
                if (institucion == null)
                    return Results.BadRequest(new { mensaje = $"No existe una institución activa con ID {carrera.IdInstitucion}" });

                var updated = await service.UpdateAsync(id, carrera);
                if (updated is null)
                    return Results.NotFound(new { mensaje = $"No se encontró una carrera con ID {id}" });

                if (usuarioId.HasValue)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Modificó la carrera '{updated.Nombre}' (ID: {updated.Id})", token);

                return Results.Ok(updated);
            })
            .WithName("UpdateCarrera");

            // DELETE /api/Carreras/{id}
            group.MapDelete("/{id}", async ([FromServices] ICarrerasService service,
                                           [FromServices] IBitacoraService bitacoraService,
                                           [FromServices] IHttpContextAccessor httpContextAccessor,
                                           int id) =>
            {
                var carrera = await service.GetByIdAsync(id);
                if (carrera is null)
                    return Results.NotFound(new { mensaje = $"No se encontró una carrera con ID {id}" });

                await service.DeleteAsync(id);

                var (usuarioId, token) = GetAuthInfo(httpContextAccessor);
                if (usuarioId.HasValue && token != null)
                    await bitacoraService.RegistrarAsync(usuarioId.Value,
                        $"Eliminó la carrera '{carrera.Nombre}' (ID: {carrera.Id})", token);

                return Results.NoContent();
            })
            .WithName("DeleteCarrera");
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