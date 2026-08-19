package cr.ac.cuc.carnetdigital.guarda.data.model

/**
 * Coincide con PerfilUsuarioDto.cs de Microservicio.Usuario. El backend serializa
 * en camelCase por defecto (Minimal APIs + System.Text.Json).
 */
data class PerfilUsuarioDto(
    val identificacion: String,
    val email: String,
    val nombreCompleto: String,
    val tipoIdentificacionId: Int,
    val tipoIdentificacion: String,
    val tipoUsuarioId: Int,
    val tipoUsuario: String,
    val institucionesIds: List<Int> = emptyList(),
    val carrerasIds: List<Int> = emptyList(),
    val areasIds: List<Int> = emptyList(),
    val carreraOArea: String?,
    val tieneFotografia: Boolean
)

/** Coincide con FotografiaDto.cs. */
data class FotografiaDto(
    val identificacion: String?,
    val fotoBase64: String?
)
