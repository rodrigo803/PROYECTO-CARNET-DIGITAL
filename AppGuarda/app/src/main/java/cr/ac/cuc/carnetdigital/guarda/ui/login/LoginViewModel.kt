package cr.ac.cuc.carnetdigital.guarda.ui.login

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import cr.ac.cuc.carnetdigital.guarda.data.network.RetrofitClient
import cr.ac.cuc.carnetdigital.guarda.data.network.mensajeDeError
import cr.ac.cuc.carnetdigital.guarda.data.model.LoginRequest
import cr.ac.cuc.carnetdigital.guarda.data.session.SessionManager
import kotlinx.coroutines.launch
import java.io.IOException

/**
 * GRD1: el tipo de usuario para esta app siempre es "GUARDA" — no se le pide
 * al usuario, es fijo (esta app es exclusiva para guardas de seguridad).
 * Si tu equipo definió otro código para el tipo de usuario "guarda" en la
 * base de datos, cambia únicamente esta constante.
 */
private const val TIPO_USUARIO_GUARDA = "GUARDA"

class LoginViewModel : ViewModel() {

    var email by mutableStateOf("")
        private set

    var contrasena by mutableStateOf("")
        private set

    var cargando by mutableStateOf(false)
        private set

    var mensajeError by mutableStateOf<String?>(null)
        private set

    var loginExitoso by mutableStateOf(false)
        private set

    fun onEmailChange(value: String) {
        email = value
        mensajeError = null
    }

    fun onContrasenaChange(value: String) {
        contrasena = value
        mensajeError = null
    }

    fun ingresar() {
        if (email.isBlank() || contrasena.isBlank()) {
            mensajeError = "Debe ingresar email y contraseña."
            return
        }

        cargando = true
        mensajeError = null

        viewModelScope.launch {
            try {
                val respuesta = RetrofitClient.authApi.login(
                    LoginRequest(
                        usuario = email.trim(),
                        contrasena = contrasena,
                        tipousuario = TIPO_USUARIO_GUARDA
                    )
                )

                if (respuesta.isSuccessful && respuesta.body() != null) {
                    SessionManager.guardarToken(respuesta.body()!!.access_token)
                    loginExitoso = true
                } else if (respuesta.code() == 401) {
                    // Mensaje exacto exigido por la HU GRD1.
                    mensajeError = "Usuario y/o contraseña incorrectos."
                } else if (respuesta.code() == 403) {
                    mensajeError = respuesta.mensajeDeError(
                        "Su usuario ha sido bloqueado por exceder el número de intentos permitidos."
                    )
                } else {
                    mensajeError = respuesta.mensajeDeError()
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
}
