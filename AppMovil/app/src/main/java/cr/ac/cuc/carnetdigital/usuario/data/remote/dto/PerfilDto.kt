package cr.ac.cuc.carnetdigital.usuario.data.remote.dto

import cr.ac.cuc.carnetdigital.usuario.domain.model.Perfil

/** Respuesta de GET /gateway/usuarios/perfil, ya resuelta con nombres por el backend. */
data class PerfilDto(
    val identificacion: String,
    val nombreCompleto: String,
    val tipoUsuario: String,
    val carreraOArea: String,
    val tieneFotografia: Boolean
)

fun PerfilDto.toDomain() = Perfil(
    identificacion = identificacion,
    nombreCompleto = nombreCompleto,
    tipoUsuario = tipoUsuario,
    carreraOArea = carreraOArea,
    tieneFotografia = tieneFotografia
)
