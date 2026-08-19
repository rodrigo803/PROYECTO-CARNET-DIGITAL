package cr.ac.cuc.carnetdigital.guarda.core.network

/**
 * Clase sellada para manejar los estados de las peticiones a la API.
 */
sealed class NetworkResult<T> {
    class Success<T>(val data: T) : NetworkResult<T>()
    class Error<T>(val message: String, val code: Int? = null) : NetworkResult<T>()
}