package cr.ac.cuc.carnetdigital.usuario.domain.model

/** Entidad de negocio del perfil del usuario autenticado, independiente de Retrofit/Gson. */
data class Perfil(
    val identificacion: String,
    val nombreCompleto: String,
    val tipoUsuario: String,
    val carreraOArea: String,
    val tieneFotografia: Boolean
)
