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
import androidx.compose.material.icons.filled.Settings
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
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import cr.ac.cuc.carnetdigital.usuario.R
import cr.ac.cuc.carnetdigital.usuario.domain.model.Perfil
import cr.ac.cuc.carnetdigital.usuario.settings.AppSettings
import cr.ac.cuc.carnetdigital.usuario.settings.ThemeMode
import cr.ac.cuc.carnetdigital.usuario.ui.settings.SettingsDialog

/** Ruta Compose que conecta el estado del perfil (USR2) con la pantalla. */
@Composable
fun PerfilRoute(
    viewModel: PerfilViewModel,
    onLogout: () -> Unit,
    settings: AppSettings,
    onThemeChanged: (ThemeMode) -> Unit
) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    PerfilScreen(state, onLogout, settings, onThemeChanged)
}

/** Muestra el perfil del usuario y, deslizando, el QR (USR3) — bloqueado si no tiene fotografía. */
@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun PerfilScreen(
    state: PerfilUiState,
    onLogout: () -> Unit,
    settings: AppSettings,
    onThemeChanged: (ThemeMode) -> Unit
) {
    var showSettings by remember { mutableStateOf(false) }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(stringResource(R.string.perfil_titulo)) },
                actions = {
                    IconButton(onClick = { showSettings = true }) {
                        Icon(Icons.Default.Settings, contentDescription = stringResource(R.string.cd_ajustes))
                    }
                    IconButton(onClick = onLogout) {
                        Icon(Icons.AutoMirrored.Filled.ExitToApp, contentDescription = stringResource(R.string.perfil_cerrar_sesion))
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

    if (showSettings) {
        SettingsDialog(
            settings = settings,
            onThemeChanged = onThemeChanged,
            onDismiss = { showSettings = false }
        )
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
                contentDescription = stringResource(R.string.perfil_foto_desc),
                contentScale = ContentScale.Crop,
                modifier = Modifier
                    .size(160.dp)
                    .clip(CircleShape)
            )
        } else {
            Icon(
                Icons.Default.AccountCircle,
                contentDescription = stringResource(R.string.perfil_avatar_generico),
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
                stringResource(R.string.perfil_sin_foto),
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
            text = stringResource(R.string.qr_titulo),
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
                    contentDescription = stringResource(R.string.qr_imagen_desc),
                    contentScale = ContentScale.Fit,
                    modifier = Modifier
                        .size(260.dp)
                        .background(MaterialTheme.colorScheme.surface)
                )

                Spacer(Modifier.height(32.dp))

                Text(
                    text = stringResource(R.string.qr_swipe_hint),
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant,
                    textAlign = TextAlign.Center
                )
            }
            !qrError.isNullOrEmpty() -> {
                // Muestra el error detallado en pantalla si algo falla en la red
                Text(
                    text = stringResource(R.string.qr_error_prefix, qrError),
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
                    text = stringResource(R.string.qr_cargando),
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
        Text(mensaje ?: stringResource(R.string.perfil_error_default), textAlign = TextAlign.Center)
    }
}