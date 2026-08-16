package cr.ac.cuc.carnetdigital.usuario.ui.auth

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import cr.ac.cuc.carnetdigital.usuario.core.network.NetworkResult
import cr.ac.cuc.carnetdigital.usuario.core.session.SessionManager
import cr.ac.cuc.carnetdigital.usuario.domain.repository.AuthRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch

/** Tipos de usuario aceptados por AuthService (columna UsersAuth.UserType, sin catálogo propio). */
val TIPOS_USUARIO = listOf("Admin", "Estudiante", "Funcionario", "Visitante", "Auditor")

/** ViewModel que conserva el estado del formulario de login y coordina la autenticación. */
class LoginViewModel(
    private val repository: AuthRepository,
    private val sessionManager: SessionManager
) : ViewModel() {
    private val _uiState = MutableStateFlow(LoginUiState(tipoUsuario = TIPOS_USUARIO.first()))
    val uiState = _uiState.asStateFlow()

    fun onUsuarioChange(value: String) = _uiState.update { it.copy(usuario = value, error = null) }
    fun onContrasenaChange(value: String) = _uiState.update { it.copy(contrasena = value, error = null) }
    fun onTipoUsuarioChange(value: String) = _uiState.update { it.copy(tipoUsuario = value, error = null) }

    fun login(onSuccess: () -> Unit) {
        val state = _uiState.value
        if (state.usuario.isBlank() || state.contrasena.isBlank()) {
            _uiState.update { it.copy(error = "Complete usuario y contraseña.") }
            return
        }
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, error = null) }
            when (val result = repository.login(state.usuario.trim(), state.contrasena, state.tipoUsuario)) {
                is NetworkResult.Success -> {
                    sessionManager.saveSession(result.data.accessToken, result.data.refreshToken, result.data.usuarioId)
                    _uiState.update { it.copy(isLoading = false) }
                    onSuccess()
                }
                is NetworkResult.Error -> _uiState.update { it.copy(isLoading = false, error = result.message) }
            }
        }
    }
}

/** Estado inmutable que la pantalla de login observa. */
data class LoginUiState(
    val usuario: String = "",
    val contrasena: String = "",
    val tipoUsuario: String = "",
    val isLoading: Boolean = false,
    val error: String? = null
)

/** Fábrica explícita que inyecta el repositorio y el manejador de sesión sin acoplar el ViewModel a Android. */
class LoginViewModelFactory(
    private val repository: AuthRepository,
    private val sessionManager: SessionManager
) : ViewModelProvider.Factory {
    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel> create(modelClass: Class<T>): T = LoginViewModel(repository, sessionManager) as T
}
