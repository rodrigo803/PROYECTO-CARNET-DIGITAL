package cr.ac.cuc.carnetdigital.usuario.domain.repository

import cr.ac.cuc.carnetdigital.usuario.core.network.NetworkResult
import cr.ac.cuc.carnetdigital.usuario.domain.model.Perfil

interface UsuarioRepository {
    suspend fun obtenerPerfil(): NetworkResult<Perfil>
    suspend fun obtenerFotografiaBase64(identificacion: String): NetworkResult<String>
    // Nueva función requerida para la USR3
    suspend fun obtenerQrBase64(identificacion: String): NetworkResult<String>
}
