package cr.ac.cuc.carnetdigital.guarda.data.model

/**
 * Coincide exactamente con LoginRequest.cs de AuthService: los tres campos
 * son en minúsculas y sin camelCase.
 */
data class LoginRequest(
    val usuario: String,
    val contrasena: String,
    val tipousuario: String
)

/**
 * Coincide con TokenResponse.cs. expiresIn llega como fecha/hora ISO (no como
 * segundos), tal como lo genera JwtService.GenerateToken en el backend.
 */
data class LoginResponse(
    val expires_in: String,
    val access_token: String,
    val refresh_token: String,
    val usuarioID: Int
)

/** Forma del cuerpo de error que devuelve AuthController en 401/403/400. */
data class ErrorResponse(
    val mensaje: String?
)
