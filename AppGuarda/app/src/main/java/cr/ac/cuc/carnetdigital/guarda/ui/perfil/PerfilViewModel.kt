package cr.ac.cuc.carnetdigital.guarda.ui.perfil

import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.util.Base64
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cr.ac.cuc.carnetdigital.guarda.data.model.PerfilUsuarioDto
import cr.ac.cuc.carnetdigital.guarda.data.network.RetrofitClient
import cr.ac.cuc.carnetdigital.guarda.data.network.mensajeDeError
import kotlinx.coroutines.launch
import java.io.IOException

class PerfilViewModel : ViewModel() {

    var cargando by mutableStateOf(true)
        private set

    var perfil by mutableStateOf<PerfilUsuarioDto?>(null)
        private set

    var fotografia by mutableStateOf<Bitmap?>(null)
        private set

    var mensajeError by mutableStateOf<String?>(null)
        private set

    init {
        cargarPerfil()
    }

    fun cargarPerfil() {
        cargando = true
        mensajeError = null

        viewModelScope.launch {
            try {
                val respuesta = RetrofitClient.usuarioApi.obtenerPerfil()

                if (respuesta.isSuccessful && respuesta.body() != null) {
                    val datos = respuesta.body()!!
                    perfil = datos

                    // GRD2: solo se pide la fotografía si el usuario ya tiene una registrada;
                    // si no, se muestra el avatar + la leyenda de advertencia.
                    if (datos.tieneFotografia) {
                        cargarFotografia(datos.identificacion)
                    }
                } else {
                    mensajeError = respuesta.mensajeDeError(
                        "No fue posible cargar tu información."
                    )
                }
            } catch (e: IOException) {
                mensajeError = "No fue posible conectar con el servidor. Verifica tu conexión."
            } catch (e: Exception) {
                mensajeError = "Ocurrió un error inesperado."
            } finally {
                cargando = false
            }
        }
    }

    private suspend fun cargarFotografia(identificacion: String) {
        try {
            val respuesta = RetrofitClient.usuarioApi.obtenerFotografia(identificacion)
            val base64 = respuesta.body()?.fotoBase64

            if (respuesta.isSuccessful && !base64.isNullOrBlank()) {
                val bytes = Base64.decode(base64, Base64.DEFAULT)
                fotografia = BitmapFactory.decodeByteArray(bytes, 0, bytes.size)
            }
        } catch (e: Exception) {
            // No se rompe la pantalla de perfil si solo falla traer la foto;
            // simplemente se queda mostrando el avatar por defecto.
        }
    }
}
