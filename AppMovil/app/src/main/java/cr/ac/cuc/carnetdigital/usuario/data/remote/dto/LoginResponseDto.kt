package cr.ac.cuc.carnetdigital.usuario.data.remote.dto

import com.google.gson.annotations.SerializedName
import cr.ac.cuc.carnetdigital.usuario.domain.model.Sesion

/** Respuesta de AuthService al loguear. Los nombres de campo son snake_case tal como los envía el backend. */
data class LoginResponseDto(
    @SerializedName("access_token") val accessToken: String,
    @SerializedName("refresh_token") val refreshToken: String,
    @SerializedName("usuarioID") val usuarioId: Int,
    @SerializedName("expires_in") val expiresIn: String
)

/** Respuesta de error estándar que devuelven los endpoints de AuthService. */
data class ErrorResponseDto(val mensaje: String?)

fun LoginResponseDto.toDomain() = Sesion(
    accessToken = accessToken,
    refreshToken = refreshToken,
    usuarioId = usuarioId
)
