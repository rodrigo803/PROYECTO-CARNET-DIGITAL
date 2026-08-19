package cr.ac.cuc.carnetdigital.guarda.ui.scanner

import android.app.Application
import android.media.AudioManager
import android.media.ToneGenerator
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import com.google.gson.Gson
import com.google.gson.JsonSyntaxException
import cr.ac.cuc.carnetdigital.guarda.core.network.RetrofitHelper
import cr.ac.cuc.carnetdigital.guarda.data.remote.GuardaApi
import cr.ac.cuc.carnetdigital.guarda.data.remote.dto.QrContenidoDto
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

// Estados posibles de la pantalla
sealed class ScanState {
    object Idle : ScanState() // Esperando leer QR
    object Processing : ScanState() // Procesando/Consultando API
    data class Success(val message: String, val userName: String) : ScanState() // QR Válido
    data class Error(val message: String) : ScanState() // QR Inválido
}

class ScannerViewModel(application: Application) : AndroidViewModel(application) {
    // Pasamos el contexto de la aplicación para que el AuthInterceptor lea el token automáticamente
    private val api = RetrofitHelper.getRetrofit(application).create(GuardaApi::class.java)
    private val gson = Gson()

    // Generador de tonos para cumplir con la validación sonora
    private val toneGen = ToneGenerator(AudioManager.STREAM_MUSIC, 100)

    private val _scanState = MutableStateFlow<ScanState>(ScanState.Idle)
    val scanState: StateFlow<ScanState> = _scanState.asStateFlow()

    fun procesarCodigoQr(qrJsonString: String) {
        // Si ya estamos procesando uno, ignoramos las nuevas lecturas de la cámara
        if (_scanState.value !is ScanState.Idle) return

        _scanState.value = ScanState.Processing

        viewModelScope.launch {
            try {
                // 1. Extraer contenido JSON del QR
                val qrData = gson.fromJson(qrJsonString, QrContenidoDto::class.java)

                // Validación de seguridad: Evitar que falle si el QR no trae identificación
                val idParaBuscar = qrData.identificacion?.trim() ?: ""
                if (idParaBuscar.isEmpty()) {
                    marcarError("El código QR no contiene un número de identificación válido")
                    return@launch
                }

                // 2. Invocar servicio por llave primaria (con el token adjunto de forma automática)
                val response = api.obtenerUsuarioPorId(idParaBuscar)

                if (response.isSuccessful && response.body() != null) {
                    val backendData = response.body()!!

                    // 3. VERIFICAR DATO POR DATO (Regla de negocio principal de la HU)
                    val identificacionQr = qrData.identificacion?.trim() ?: ""
                    val nombreQr = qrData.nombre?.trim() ?: ""
                    val rolQr = qrData.tipo?.trim() ?: ""

                    val identificacionBackend = backendData.identificacion?.trim() ?: ""
                    val nombreBackend = backendData.nombreCompleto?.trim() ?: ""
                    val rolBackend = backendData.tipoUsuario?.trim() ?: ""

                    // Comparación estricta
                    val esMismaIdentificacion = identificacionQr == identificacionBackend
                    val esMismoNombre = nombreQr.equals(nombreBackend, ignoreCase = true)
                    val esMismoRol = rolQr.equals(rolBackend, ignoreCase = true)

                    // Validación del estado
                    val estaActivo = backendData.estadoId == 1

                    if (esMismaIdentificacion && esMismoNombre && esMismoRol && estaActivo) {
                        marcarExito(nombreBackend)
                    } else {
                        marcarError("Los datos del QR no coinciden o usuario inactivo")
                    }
                } else {
                    marcarError("Usuario no encontrado en el sistema")
                }
            } catch (e: JsonSyntaxException) {
                marcarError("Formato QR inválido (No es JSON)")
            } catch (e: Exception) {
                marcarError("Error de conexión: ${e.localizedMessage}")
            }
        }
    }

    private suspend fun marcarExito(nombre: String) {
        // Reproducir sonido de éxito (Beep corto y agudo)
        toneGen.startTone(ToneGenerator.TONE_PROP_ACK, 200)
        _scanState.value = ScanState.Success("Acceso Permitido", nombre)
        reiniciarEscaneo()
    }

    private suspend fun marcarError(motivo: String) {
        // Reproducir sonido de error (Beep largo y grave)
        toneGen.startTone(ToneGenerator.TONE_SUP_ERROR, 500)
        _scanState.value = ScanState.Error(motivo)
        reiniciarEscaneo()
    }

    private suspend fun reiniciarEscaneo() {
        // Muestra el resultado por 3 segundos y vuelve a abrir la lectura sin cerrar la cámara
        delay(3000)
        _scanState.value = ScanState.Idle
    }

    override fun onCleared() {
        super.onCleared()
        toneGen.release() // Liberar memoria del audio
    }
}