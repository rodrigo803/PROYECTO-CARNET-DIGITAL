package cr.ac.cuc.carnetdigital.guarda.data.remote.dto

import com.google.gson.annotations.SerializedName

// Estructura de los datos que enviamos al hacer login
data class LoginRequestDto(
    @SerializedName("usuario")
    val usuario: String,

    @SerializedName("contrasena")
    val contrasena: String,

    @SerializedName("tipousuario")
    val tipousuario: String
)

// Estructura de la respuesta que nos devuelve el servidor (con el Token JWT)
data class LoginResponseDto(
    @SerializedName("access_token")
    val token: String?,

    @SerializedName("refresh_token")
    val refreshToken: String?,

    @SerializedName("expires_in")
    val expiresIn: Long?,

    @SerializedName("usuarioID")
    val usuarioID: Int?,

    @SerializedName("mensaje")
    val mensaje: String?
)