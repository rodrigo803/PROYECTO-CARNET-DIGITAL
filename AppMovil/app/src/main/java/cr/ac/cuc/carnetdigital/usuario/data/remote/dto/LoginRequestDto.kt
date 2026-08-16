package cr.ac.cuc.carnetdigital.usuario.data.remote.dto

/** Cuerpo del POST /gateway/auth/login. Nombres de campo iguales a los que espera AuthService. */
data class LoginRequestDto(
    val usuario: String,
    val contrasena: String,
    val tipousuario: String
)
