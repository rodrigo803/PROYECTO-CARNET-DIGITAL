package cr.ac.cuc.carnetdigital.usuario.domain.repository

import cr.ac.cuc.carnetdigital.usuario.core.network.NetworkResult
import cr.ac.cuc.carnetdigital.usuario.domain.model.Sesion

/** Abstracción de las operaciones de autenticación disponibles para la capa de presentación. */
interface AuthRepository {
    suspend fun login(usuario: String, contrasena: String, tipoUsuario: String): NetworkResult<Sesion>
}
