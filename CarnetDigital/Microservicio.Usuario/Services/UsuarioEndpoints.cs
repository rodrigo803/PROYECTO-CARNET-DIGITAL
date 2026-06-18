using Microservicio.Usuario.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using static Microservicio.Usuario.Entities.UsuarioDTOs;

namespace Microservicio.Usuario.Services
{
    public static class UsuarioEndpoints
    {
        public static void MapUsuarioEndpoints(this IEndpointRouteBuilder app)
        {
            var grupo = app.MapGroup("/api/usuario");

            // 1. SRV10: Crear Usuario (Por Administrador)
            grupo.MapPost("/crear", async (UsuarioRegistroDto dto, IUsuarioService servicio) =>
            {
                var nuevoUsuario = new Entities.Usuario
                {
                    Identificacion = dto.Identificacion,
                    Email = dto.Email,
                    NombreCompleto = dto.NombreCompleto,
                    TipoIdentificacionId = dto.TipoIdentificacionId,
                    TipoUsuarioId = dto.TipoUsuarioId,
                    RolId = dto.RolId,
                    TipoIdentificacion = dto.TipoIdentificacion,
                    TipoUsuario = dto.TipoUsuario,

                    // ¡ESTO ES LO QUE FALTABA!
                    InstitucionesIds = dto.InstitucionesIds,
                    CarrerasIds = dto.CarrerasIds,
                    AreasIds = dto.AreasIds
                };

                var usuarioCreado = await servicio.CrearUsuarioAsync(nuevoUsuario, dto.Contrasena);
                return Results.Ok(usuarioCreado);
            }).RequireAuthorization();

            // 2. SRV11: Autoregistro
            grupo.MapPost("/autoregistro", async (UsuarioRegistroDto dto, IUsuarioService servicio) =>
            {
                var usuarioCreado = await servicio.AutoregistroAsync(dto);
                return Results.Ok(usuarioCreado);
            }).AllowAnonymous();

            // 3. Confirmar Registro (Viene del enlace del correo electrónico)
            grupo.MapGet("/autoregistro/confirmar", async (string token, IUsuarioService servicio) =>
            {
                try
                {
                    var usuarioConfirmado = await servicio.ConfirmarRegistroAsync(token);
                    return Results.Ok(usuarioConfirmado);
                }
                catch (Exception ex)
                {
                    // Atrapa errores como "Token expirado" o "Token inválido"
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            }).AllowAnonymous();

            // 4. SRV12: Cambiar Estado
            grupo.MapPatch("/estado/{identificacion}", async (string identificacion, CambioEstadoDto peticion, IUsuarioService servicio) =>
            {
                try
                {
                    var estadoCambiado = await servicio.CambiarEstadoAsync(identificacion, peticion.EstadoId);
                    return estadoCambiado != null ? Results.Ok(estadoCambiado) : Results.NotFound(new { Mensaje = "Usuario no encontrado." });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            }).RequireAuthorization();

            // 5. SRV13: Actualizar Fotografía
            grupo.MapPatch("/fotografia", async (FotografiaDto peticion, IUsuarioService servicio) =>
            {
                try
                {
                    var usuarioActualizado = await servicio.ActualizarFotografiaAsync(peticion);
                    return usuarioActualizado != null ? Results.Ok(usuarioActualizado) : Results.NotFound(new { Mensaje = "Usuario no encontrado." });
                }
                catch (Exception ex)
                {
                    // Atrapa el error "La imagen supera el límite de 1MB"
                    return Results.BadRequest(new { Mensaje = ex.Message });
                }
            }).RequireAuthorization();

            // 6. SRV14: Generar QR Base 64
            grupo.MapGet("/qr/{identificacion}", async (string identificacion, IUsuarioService servicio) =>
            {
                try
                {
                    var qrBase64 = await servicio.GenerarQRBase64Async(identificacion);
                    return Results.Ok(new { QrImagenBase64 = qrBase64 });
                }
                catch (Exception ex)
                {
                    // Atrapa si el usuario no existe para generarle el QR
                    return Results.NotFound(new { Mensaje = ex.Message });
                }
            }).RequireAuthorization();

            // 7. Actualizar Datos Generales del Usuario
            grupo.MapPut("/actualizar", async (UsuarioActualizacionDto registro, IUsuarioService servicio) =>
            {
                var usuarioActualizado = await servicio.ActualizarUsuarioAsync(registro);
                return usuarioActualizado != null ? Results.Ok(usuarioActualizado) : Results.NotFound(new { Mensaje = "Usuario no encontrado." });
            }).RequireAuthorization();

            // 8. Eliminar Usuario
            grupo.MapDelete("/eliminar/{identificacion}", async (string identificacion, IUsuarioService servicio) =>
            {
                var usuarioEliminado = await servicio.EliminarUsuarioAsync(identificacion);
                return usuarioEliminado != null ? Results.Ok(usuarioEliminado) : Results.NotFound(new { Mensaje = "Usuario no encontrado." });
            }).RequireAuthorization();
        }
    }
}