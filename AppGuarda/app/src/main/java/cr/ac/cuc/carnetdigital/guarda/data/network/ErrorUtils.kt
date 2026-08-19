package cr.ac.cuc.carnetdigital.guarda.data.network

import com.google.gson.Gson
import cr.ac.cuc.carnetdigital.guarda.data.model.ErrorResponse
import retrofit2.Response

/**
 * Los endpoints de AuthController devuelven errores como { "mensaje": "..." }.
 * Esto los extrae de forma segura, con un mensaje genérico si el cuerpo no
 * viene en el formato esperado (o si no hay conexión al Gateway).
 */
fun <T> Response<T>.mensajeDeError(default: String = "Ocurrió un error inesperado."): String {
    val cuerpo = errorBody()?.string()
    if (cuerpo.isNullOrBlank()) return default

    return try {
        Gson().fromJson(cuerpo, ErrorResponse::class.java)?.mensaje ?: default
    } catch (e: Exception) {
        default
    }
}
