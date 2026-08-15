package cr.ac.cuc.carnetdigital.usuario.ui.perfil

import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.util.Base64
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import cr.ac.cuc.carnetdigital.usuario.core.network.NetworkResult
import cr.ac.cuc.carnetdigital.usuario.domain.model.Perfil
import cr.ac.cuc.carnetdigital.usuario.domain.repository.UsuarioRepository
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

/** ViewModel que carga el perfil del usuario autenticado (USR2) y, si tiene, su fotografía. */
class PerfilViewModel(private val repository: UsuarioRepository) : ViewModel() {
    private val _uiState = MutableStateFlow(PerfilUiState())
    val uiState = _uiState.asStateFlow()

    init { cargarPerfil() }

    fun cargarPerfil() {
        viewModelScope.launch {
            _uiState.update { it.copy(isLoading = true, error = null) }
            when (val result = repository.obtenerPerfil()) {
                is NetworkResult.Success -> {
                    _uiState.update { it.copy(isLoading = false, perfil = result.data) }
                    if (result.data.tieneFotografia) cargarFotografia(result.data.identificacion)
                }
                is NetworkResult.Error -> _uiState.update { it.copy(isLoading = false, error = result.message) }
            }
        }
    }

    private fun cargarFotografia(identificacion: String) {
        viewModelScope.launch {
            val result = repository.obtenerFotografiaBase64(identificacion)
            if (result is NetworkResult.Success) {
                val bitmap = withContext(Dispatchers.Default) { decodificarBase64(result.data) }
                _uiState.update { it.copy(fotoBitmap = bitmap) }
            }
            // Si falla la descarga o el decode, se queda con el avatar genérico; no bloquea la pantalla.
        }
    }

    private fun decodificarBase64(base64: String): Bitmap? = try {
        val bytes = Base64.decode(base64, Base64.DEFAULT)
        BitmapFactory.decodeByteArray(bytes, 0, bytes.size)
    } catch (_: Exception) {
        null
    }
}

/** Estado inmutable que la pantalla de perfil observa. */
data class PerfilUiState(
    val isLoading: Boolean = true,
    val perfil: Perfil? = null,
    val fotoBitmap: Bitmap? = null,
    val error: String? = null
)

/** Fábrica explícita que inyecta el repositorio sin acoplar el ViewModel a Android. */
class PerfilViewModelFactory(private val repository: UsuarioRepository) : ViewModelProvider.Factory {
    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel> create(modelClass: Class<T>): T = PerfilViewModel(repository) as T
}
