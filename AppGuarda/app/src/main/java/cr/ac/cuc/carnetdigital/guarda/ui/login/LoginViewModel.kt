package cr.ac.cuc.carnetdigital.guarda.ui.login

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import cr.ac.cuc.carnetdigital.guarda.core.network.RetrofitHelper
import cr.ac.cuc.carnetdigital.guarda.core.security.TokenManager
import cr.ac.cuc.carnetdigital.guarda.data.remote.GuardaApi
import cr.ac.cuc.carnetdigital.guarda.data.remote.dto.LoginRequestDto
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

sealed class LoginState {
    object Idle : LoginState()
    object Loading : LoginState()
    object Success : LoginState()
    data class Error(val message: String) : LoginState()
}

class LoginViewModel(application: Application) : AndroidViewModel(application) {
    private val api = RetrofitHelper.getRetrofit(application).create(GuardaApi::class.java)
    private val tokenManager = TokenManager(application)

    private val _loginState = MutableStateFlow<LoginState>(LoginState.Idle)
    val loginState: StateFlow<LoginState> = _loginState.asStateFlow()

    fun iniciarSesion(email: String, contrasenaInput: String) {
        if (email.isBlank() || contrasenaInput.isBlank()) {
            _loginState.value = LoginState.Error("Por favor completa todos los campos")
            return
        }

        _loginState.value = LoginState.Loading

        viewModelScope.launch {
            try {
                // Alineado exactamente con las propiedades del modelo LoginRequest de C#
                val request = LoginRequestDto(
                    usuario = email,
                    contrasena = contrasenaInput,
                    tipousuario = "Guarda"
                )

                val response = api.login(request)

                if (response.isSuccessful && response.body() != null) {
                    val body = response.body()!!
                    val token = body.token

                    if (!token.isNullOrEmpty()) {
                        tokenManager.saveToken(token)
                        _loginState.value = LoginState.Success
                    } else {
                        _loginState.value = LoginState.Error(body.mensaje ?: "Token no recibido en la respuesta")
                    }
                } else {
                    // 🔍 CAPTURAMOS EL CUERPO DEL ERROR QUE ENVÍA EL BACKEND (INCLUYENDO LOS [DEBUG ERROR])
                    val errorBodyString = response.errorBody()?.string()

                    val errorMensaje = if (!errorBodyString.isNullOrEmpty()) {
                        // Muestra exactamente el texto de diagnóstico enviado por el servidor C#
                        errorBodyString
                    } else {
                        "Usuario y/o contraseña incorrectos."
                    }

                    _loginState.value = LoginState.Error(errorMensaje)
                }
            } catch (e: Exception) {
                _loginState.value = LoginState.Error("Error de red: ${e.localizedMessage}")
            }
        }
    }
}