package cr.ac.cuc.carnetdigital.guarda.ui.perfil

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.CameraAlt
import androidx.compose.material.icons.filled.Person
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel

/**
 * GRD2: Yo como guarda autenticado, quiero ver mis datos personales básicos,
 * para saber que ingresé correctamente con mi cuenta.
 *
 * Nota: el ícono de cámara lleva a GRD3 (escaneo de QR), que no forma parte
 * de este alcance — se deja como punto de entrada (onEscanearQr) para que
 * quien construya GRD3 solo tenga que conectar la navegación ahí.
 */
@Composable
fun PerfilScreen(
    onEscanearQr: () -> Unit = {},
    viewModel: PerfilViewModel = viewModel()
) {
    val perfil = viewModel.perfil

    Box(modifier = Modifier.fillMaxSize()) {
        when {
            viewModel.cargando -> {
                CircularProgressIndicator(modifier = Modifier.align(Alignment.Center))
            }

            viewModel.mensajeError != null -> {
                Column(
                    modifier = Modifier
                        .align(Alignment.Center)
                        .padding(24.dp),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    Text(
                        text = viewModel.mensajeError ?: "",
                        color = MaterialTheme.colorScheme.error,
                        textAlign = TextAlign.Center
                    )
                    Spacer(modifier = Modifier.height(16.dp))
                    Button(onClick = { viewModel.cargarPerfil() }) {
                        Text("Reintentar")
                    }
                }
            }

            perfil != null -> {
                Column(
                    modifier = Modifier
                        .fillMaxSize()
                        .padding(24.dp),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    Spacer(modifier = Modifier.height(24.dp))

                    FotoOAvatar(
                        fotoBase64Bitmap = viewModel.fotografia,
                        tieneFotografia = perfil.tieneFotografia
                    )

                    Spacer(modifier = Modifier.height(20.dp))

                    Text(
                        text = perfil.nombreCompleto,
                        fontSize = 20.sp,
                        fontWeight = FontWeight.Bold,
                        textAlign = TextAlign.Center
                    )

                    Spacer(modifier = Modifier.height(4.dp))

                    Text(
                        text = perfil.identificacion,
                        fontSize = 15.sp,
                        color = Color.Gray
                    )

                    Spacer(modifier = Modifier.height(4.dp))

                    // GRD2: solo tipo de usuario. A propósito NO se muestra
                    // carreraOArea aquí (esa parte es exclusiva de USR2, no de GRD2).
                    AssistChip(
                        onClick = {},
                        label = { Text(perfil.tipoUsuario) }
                    )

                    if (!perfil.tieneFotografia) {
                        Spacer(modifier = Modifier.height(16.dp))
                        Text(
                            text = "No se validará el uso de esta aplicación hasta que haya " +
                                "registrado su fotografía.",
                            color = MaterialTheme.colorScheme.error,
                            textAlign = TextAlign.Center,
                            fontSize = 13.sp,
                            modifier = Modifier.padding(horizontal = 16.dp)
                        )
                    }

                    Spacer(modifier = Modifier.weight(1f))

                    // Ícono de cámara -> GRD3 (fuera de este alcance).
                    IconButton(
                        onClick = onEscanearQr,
                        modifier = Modifier
                            .size(64.dp)
                            .background(Color(0xFF1F3864), shape = CircleShape)
                    ) {
                        Icon(
                            imageVector = Icons.Filled.CameraAlt,
                            contentDescription = "Escanear código QR",
                            tint = Color.White
                        )
                    }

                    Spacer(modifier = Modifier.height(24.dp))
                }
            }
        }
    }
}

@Composable
private fun FotoOAvatar(
    fotoBase64Bitmap: android.graphics.Bitmap?,
    tieneFotografia: Boolean
) {
    Box(
        modifier = Modifier
            .size(120.dp)
            .clip(CircleShape)
            .background(Color.LightGray),
        contentAlignment = Alignment.Center
    ) {
        if (tieneFotografia && fotoBase64Bitmap != null) {
            Image(
                bitmap = fotoBase64Bitmap.asImageBitmap(),
                contentDescription = "Fotografía del usuario",
                modifier = Modifier
                    .size(120.dp)
                    .clip(CircleShape)
            )
        } else {
            // Avatar por defecto cuando el usuario no tiene fotografía registrada.
            Icon(
                imageVector = Icons.Filled.Person,
                contentDescription = "Avatar",
                modifier = Modifier.size(64.dp),
                tint = Color.DarkGray
            )
        }
    }
}
