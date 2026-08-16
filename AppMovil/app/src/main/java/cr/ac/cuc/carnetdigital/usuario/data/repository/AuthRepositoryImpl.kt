package cr.ac.cuc.carnetdigital.usuario.data.repository

import com.google.gson.Gson
import cr.ac.cuc.carnetdigital.usuario.core.network.NetworkResult
import cr.ac.cuc.carnetdigital.usuario.data.remote.AuthApi
import cr.ac.cuc.carnetdigital.usuario.data.remote.dto.ErrorResponseDto
import cr.ac.cuc.carnetdigital.usuario.data.remote.dto.LoginRequestDto
import cr.ac.cuc.carnetdigital.usuario.data.remote.dto.toDomain
import cr.ac.cuc.carnetdigital.usuario.domain.model.Sesion
import cr.ac.cuc.carnetdigital.usuario.domain.repository.AuthRepository
import java.io.IOException

private const val MENSAJE_CREDENCIALES_INCORRECTAS = "Usuario y/o contraseña incorrectos."

/** Implementación del repositorio de autenticación que encapsula Retrofit, mapeos y errores de red. */
class AuthRepositoryImpl(private val api: AuthApi) : AuthRepository {
    private val gson = Gson()

    override suspend fun login(usuario: String, contrasena: String, tipoUsuario: String): NetworkResult<Sesion> = try {
        val response = api.login(LoginRequestDto(usuario, contrasena, tipoUsuario))
        val body = response.body()
        if (response.isSuccessful && body != null) {
            NetworkResult.Success(body.toDomain())
        } else {
            NetworkResult.Error(mensajeDeError(response.errorBody()?.string()), response.code())
        }
    } catch (_: IOException) {
        NetworkResult.Error(MENSAJE_CREDENCIALES_INCORRECTAS)
    } catch (_: Exception) {
        NetworkResult.Error(MENSAJE_CREDENCIALES_INCORRECTAS)
    }

    private fun mensajeDeError(errorBody: String?): String {
        val mensaje = errorBody?.let {
            runCatching { gson.fromJson(it, ErrorResponseDto::class.java)?.mensaje }.getOrNull()
        }
        return mensaje ?: MENSAJE_CREDENCIALES_INCORRECTAS
    }
}
