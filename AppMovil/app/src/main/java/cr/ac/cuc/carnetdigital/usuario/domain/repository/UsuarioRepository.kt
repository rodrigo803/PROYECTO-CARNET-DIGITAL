package cr.ac.cuc.carnetdigital.usuario.domain.repository

import cr.ac.cuc.carnetdigital.usuario.core.network.NetworkResult
import cr.ac.cuc.carnetdigital.usuario.domain.model.Perfil

/** Abstracción de las operaciones sobre el usuario autenticado disponibles para la capa de presentación. */
interface UsuarioRepository {
    suspend fun obtenerPerfil(): NetworkResult<Perfil>
    suspend fun obtenerFotografiaBase64(identificacion: String): NetworkResult<String>
}
