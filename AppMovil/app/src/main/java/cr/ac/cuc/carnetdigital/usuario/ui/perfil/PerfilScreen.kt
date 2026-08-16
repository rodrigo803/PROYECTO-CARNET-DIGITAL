package cr.ac.cuc.carnetdigital.usuario.ui.perfil

import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.util.Base64
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.pager.HorizontalPager
import androidx.compose.foundation.pager.rememberPagerState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ExitToApp
import androidx.compose.material.icons.filled.AccountCircle
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TopAppBar
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import cr.ac.cuc.carnetdigital.usuario.domain.model.Perfil

/** Ruta Compose que conecta el estado del perfil (USR2) con la pantalla. */
@Composable
fun PerfilRoute(viewModel: PerfilViewModel, onLogout: () -> Unit) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    PerfilScreen(state, onLogout)
}

/** Muestra el perfil del usuario y, deslizando, el QR (USR3) — bloqueado si no tiene fotografía. */
@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun PerfilScreen(state: PerfilUiState, onLogout: () -> Unit) {
    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Mi Carnet") },
                actions = {
                    IconButton(onClick = onLogout) {
                        Icon(Icons.AutoMirrored.Filled.ExitToApp, contentDescription = "Cerrar sesión")
                    }
                }
            )
        }
    ) { padding ->
        when {
            state.isLoading -> LoadingContent(Modifier.padding(padding))
            state.perfil == null -> ErrorContent(state.error, Modifier.padding(padding))
            else -> {
                val pagerState = rememberPagerState(pageCount = { 2 })
                HorizontalPager(
                    state = pagerState,
                    userScrollEnabled = state.perfil.tieneFotografia,
                    modifier = Modifier
                        .padding(padding)
                        .fillMaxSize()
                ) { page ->
                    if (page == 0) {
                        PerfilPage(state.perfil, state.fotoBitmap)
                    } else {
                        // USR3: Pantalla real del QR enviando el Base64 y el estado de error
                        QrPage(state.qrBase64, state.qrError)
                    }
                }
            }
        }
    }
}

/** Datos personales del usuario autenticado, con avatar genérico si aún no registró fotografía. */
@Composable
private fun PerfilPage(perfil: Perfil, fotoBitmap: Bitmap?) {
    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(24.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center
    ) {
        if (fotoBitmap != null) {
            Image(
                bitmap = fotoBitmap.asImageBitmap(),
                contentDescription = "Foto de perfil",
                contentScale = ContentScale.Crop,
                modifier = Modifier
                    .size(160.dp)
                    .clip(CircleShape)
            )
        } else {
            Icon(
                Icons.Default.AccountCircle,
                contentDescription = "Avatar genérico",
                modifier = Modifier.size(160.dp),
                tint = MaterialTheme.colorScheme.outline
            )
        }

        Spacer(Modifier.height(16.dp))
        Text(perfil.nombreCompleto, style = MaterialTheme.typography.titleLarge, textAlign = TextAlign.Center)
        Text(perfil.identificacion, style = MaterialTheme.typography.bodyMedium)
        Text(perfil.tipoUsuario, style = MaterialTheme.typography.bodyMedium)
        Text(perfil.carreraOArea, style = MaterialTheme.typography.bodyMedium)

        if (!perfil.tieneFotografia) {
            Spacer(Modifier.height(16.dp))
            Text(
                "No se validará el uso de esta aplicación hasta que haya registrado su fotografía.",
                color = MaterialTheme.colorScheme.error,
                textAlign = TextAlign.Center
            )
        }
    }
}

/**
 * Vista correspondiente a la USR3.
 * Muestra el código QR institucional del usuario o el error detallado si falla.
 */
@Composable
private fun QrPage(qrBase64: String?, qrError: String?) {
    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(24.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center
    ) {
        Text(
            text = "Código QR Institucional",
            style = MaterialTheme.typography.headlineSmall,
            textAlign = TextAlign.Center
        )

        Spacer(Modifier.height(32.dp))

        when {
            !qrBase64.isNullOrEmpty() -> {
                val imageBytes = Base64.decode(qrBase64, Base64.DEFAULT)
                val bitmap = BitmapFactory.decodeByteArray(imageBytes, 0, imageBytes.size)

                Image(
                    bitmap = bitmap.asImageBitmap(),
                    contentDescription = "Código QR para validación por seguridad",
                    contentScale = ContentScale.Fit,
                    modifier = Modifier
                        .size(260.dp)
                        .background(MaterialTheme.colorScheme.surface)
                )

                Spacer(Modifier.height(32.dp))

                Text(
                    text = "Desliza hacia la derecha para regresar a tu perfil",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                    textAlign = TextAlign.Center
                )
            }
            !qrError.isNullOrEmpty() -> {
                // Muestra el error detallado en pantalla si algo falla en la red
                Text(
                    text = "No se pudo cargar el QR:\n$qrError",
                    style = MaterialTheme.typography.bodyLarge,
                    color = MaterialTheme.colorScheme.error,
                    textAlign = TextAlign.Center
                )
            }
            else -> {
                CircularProgressIndicator(
                    modifier = Modifier.size(48.dp),
                    color = MaterialTheme.colorScheme.primary
                )
                Spacer(Modifier.height(16.dp))
                Text(
                    text = "Cargando código QR...",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
        }
    }
}

@Composable
private fun LoadingContent(modifier: Modifier) {
    Box(modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
        CircularProgressIndicator()
    }
}

@Composable
private fun ErrorContent(mensaje: String?, modifier: Modifier) {
    Box(modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
        Text(mensaje ?: "No se pudo cargar el perfil.", textAlign = TextAlign.Center)
    }
}