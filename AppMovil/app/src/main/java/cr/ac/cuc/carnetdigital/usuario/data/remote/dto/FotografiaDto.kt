package cr.ac.cuc.carnetdigital.usuario.data.remote.dto

/** Respuesta de GET /gateway/usuarios/fotografia/{identificacion}. */
data class FotografiaDto(
    val identificacion: String,
    val fotoBase64: String
)
