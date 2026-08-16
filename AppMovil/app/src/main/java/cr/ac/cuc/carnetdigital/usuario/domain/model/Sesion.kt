package cr.ac.cuc.carnetdigital.usuario.domain.model

/** Entidad de negocio de la sesión activa, independiente de Retrofit/Gson. */
data class Sesion(
    val accessToken: String,
    val refreshToken: String,
    val usuarioId: Int
)
