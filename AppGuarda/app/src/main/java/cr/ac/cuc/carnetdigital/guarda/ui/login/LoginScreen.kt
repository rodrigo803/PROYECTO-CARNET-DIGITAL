package cr.ac.cuc.carnetdigital.guarda.ui.login

import android.app.Activity
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Lock
import androidx.compose.material.icons.filled.Settings
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.viewmodel.compose.viewModel
import cr.ac.cuc.carnetdigital.guarda.settings.AppSettings
import cr.ac.cuc.carnetdigital.guarda.ui.perfil.GatewayIpDialog

/**
 * GRD1: Yo como guarda de seguridad, quiero poder ingresar con mi usuario y
 * contraseña, para acceder a la app de validación.
 */
@Composable
fun LoginScreen(
    onLoginExitoso: () -> Unit,
    viewModel: LoginViewModel = viewModel()
) {
    LaunchedEffect(viewModel.loginExitoso) {
        if (viewModel.loginExitoso) onLoginExitoso()
    }

    val activity = LocalContext.current as Activity
    val settings = remember { AppSettings(activity) }
    var showAjustes by remember { mutableStateOf(false) }

    Box(
        modifier = Modifier
            .fillMaxSize()
            .background(Color(0xFF1F3864)),
        contentAlignment = Alignment.Center
    ) {
        Column(
            modifier = Modifier
                .fillMaxWidth()
                .padding(horizontal = 32.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            // Logo del equipo / CUC. Sustituye este bloque por
            // Image(painterResource(R.drawable.logo_cuc), contentDescription = "CUC")
            // en cuanto tengas el archivo del logo real en res/drawable.
            LogoPlaceholder()

            Spacer(modifier = Modifier.height(12.dp))

            Text(
                text = "App Guarda",
                color = Color.White,
                fontSize = 16.sp,
                fontWeight = FontWeight.Medium
            )

            Spacer(modifier = Modifier.height(40.dp))

            Card(
                shape = RoundedCornerShape(16.dp),
                colors = CardDefaults.cardColors(containerColor = Color.White)
            ) {
                Column(
                    modifier = Modifier.padding(24.dp),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    OutlinedTextField(
                        value = viewModel.email,
                        onValueChange = viewModel::onEmailChange,
                        label = { Text("Email") },
                        singleLine = true,
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
                        modifier = Modifier.fillMaxWidth()
                    )

                    Spacer(modifier = Modifier.height(16.dp))

                    OutlinedTextField(
                        value = viewModel.contrasena,
                        onValueChange = viewModel::onContrasenaChange,
                        label = { Text("Contraseña") },
                        singleLine = true,
                        visualTransformation = PasswordVisualTransformation(),
                        keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Password),
                        modifier = Modifier.fillMaxWidth()
                    )

                    if (viewModel.mensajeError != null) {
                        Spacer(modifier = Modifier.height(12.dp))
                        Text(
                            text = viewModel.mensajeError ?: "",
                            color = MaterialTheme.colorScheme.error,
                            textAlign = TextAlign.Center,
                            fontSize = 13.sp
                        )
                    }

                    Spacer(modifier = Modifier.height(24.dp))

                    Button(
                        onClick = viewModel::ingresar,
                        enabled = !viewModel.cargando,
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(48.dp)
                    ) {
                        if (viewModel.cargando) {
                            CircularProgressIndicator(
                                modifier = Modifier.size(20.dp),
                                color = Color.White,
                                strokeWidth = 2.dp
                            )
                        } else {
                            Text("Ingresar")
                        }
                    }
                }
            }
        }

        IconButton(
            onClick = { showAjustes = true },
            modifier = Modifier.align(Alignment.TopEnd)
        ) {
            Icon(Icons.Filled.Settings, contentDescription = "Ajustes", tint = Color.White)
        }
    }

    if (showAjustes) {
        GatewayIpDialog(
            settings = settings,
            activity = activity,
            onDismiss = { showAjustes = false }
        )
    }
}

@Composable
private fun LogoPlaceholder() {
    Box(
        modifier = Modifier
            .size(88.dp)
            .background(Color.White, shape = RoundedCornerShape(50)),
        contentAlignment = Alignment.Center
    ) {
        Icon(
            imageVector = Icons.Filled.Lock,
            contentDescription = "Logo CUC",
            tint = Color(0xFF1F3864),
            modifier = Modifier.size(40.dp)
        )
    }
}
