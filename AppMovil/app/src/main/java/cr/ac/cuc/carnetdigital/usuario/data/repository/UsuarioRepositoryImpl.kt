package cr.ac.cuc.carnetdigital.usuario.data.repository

import cr.ac.cuc.carnetdigital.usuario.core.network.NetworkResult
import cr.ac.cuc.carnetdigital.usuario.data.remote.UsuarioApi
import cr.ac.cuc.carnetdigital.usuario.data.remote.dto.toDomain
import cr.ac.cuc.carnetdigital.usuario.domain.model.Perfil
import cr.ac.cuc.carnetdigital.usuario.domain.repository.UsuarioRepository
import java.io.IOException

/** Implementación del repositorio de usuario que encapsula Retrofit, mapeos y errores de red. */
class UsuarioRepositoryImpl(private val api: UsuarioApi) : UsuarioRepository {
    override suspend fun obtenerPerfil(): NetworkResult<Perfil> = try {
        val response = api.obtenerPerfil()
        val body = response.body()
        if (response.isSuccessful && body != null) {
            NetworkResult.Success(body.toDomain())
        } else {
            NetworkResult.Error(response.errorBody()?.string(), response.code())
        }
    } catch (_: IOException) {
        NetworkResult.Error()
    } catch (_: Exception) {
        NetworkResult.Error()
    }

    override suspend fun obtenerFotografiaBase64(identificacion: String): NetworkResult<String> = try {
        val response = api.obtenerFotografia(identificacion)
        val body = response.body()
        if (response.isSuccessful && body != null) {
            NetworkResult.Success(body.fotoBase64)
        } else {
            NetworkResult.Error(response.errorBody()?.string(), response.code())
        }
    } catch (_: IOException) {
        NetworkResult.Error()
    } catch (_: Exception) {
        NetworkResult.Error()
    }

    // NUEVO MÉTODO USR3: Consumo del endpoint del QR a través del Gateway
    override suspend fun obtenerQrBase64(identificacion: String): NetworkResult<String> = try {
        val response = api.obtenerQr(identificacion)
        val body = response.body()
        // Validamos contra qrImagenBase64 que viene del servidor de C#
        if (response.isSuccessful && body != null && !body.qrImagenBase64.isNullOrEmpty()) {
            NetworkResult.Success(body.qrImagenBase64)
        } else {
            NetworkResult.Error(response.errorBody()?.string() ?: "No se pudo obtener el QR", response.code())
        }
    } catch (_: IOException) {
        NetworkResult.Error("Error de red al obtener el QR")
    } catch (e: Exception) {
        NetworkResult.Error("Error inesperado: ${e.localizedMessage}")
    }
}
