package cr.ac.cuc.carnetdigital.usuario.ui.auth

import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.ExposedDropdownMenuAnchorType
import androidx.compose.material3.ExposedDropdownMenuBox
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.unit.dp
import androidx.lifecycle.compose.collectAsStateWithLifecycle
import cr.ac.cuc.carnetdigital.usuario.R
import cr.ac.cuc.carnetdigital.usuario.ui.components.AppTextField
import cr.ac.cuc.carnetdigital.usuario.ui.components.PrimaryButton

/** Ruta Compose que conecta el estado del login con la pantalla y navega a USR2 en caso de éxito. */
@Composable
fun LoginRoute(viewModel: LoginViewModel, onLoginSuccess: () -> Unit) {
    val state by viewModel.uiState.collectAsStateWithLifecycle()
    LoginScreen(
        state = state,
        onUsuarioChange = viewModel::onUsuarioChange,
        onContrasenaChange = viewModel::onContrasenaChange,
        onTipoUsuarioChange = viewModel::onTipoUsuarioChange,
        onLoginClick = { viewModel.login(onSuccess = onLoginSuccess) }
    )
}

/** Formulario de ingreso: email, contraseña y tipo de usuario, con el logo institucional. */
@Composable
private fun LoginScreen(
    state: LoginUiState,
    onUsuarioChange: (String) -> Unit,
    onContrasenaChange: (String) -> Unit,
    onTipoUsuarioChange: (String) -> Unit,
    onLoginClick: () -> Unit
) {
    Scaffold { padding ->
        Column(
            modifier = Modifier
                .padding(padding)
                .fillMaxSize()
                .padding(24.dp),
            verticalArrangement = Arrangement.Center,
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Image(
                painter = painterResource(R.drawable.logo_cuc),
                contentDescription = stringResource(R.string.app_name),
                modifier = Modifier
                    .fillMaxWidth(0.7f)
                    .padding(bottom = 32.dp)
            )

            AppTextField(
                value = state.usuario,
                onValueChange = onUsuarioChange,
                label = "Correo electrónico"
            )
            Spacer(Modifier.height(12.dp))

            AppTextField(
                value = state.contrasena,
                onValueChange = onContrasenaChange,
                label = "Contraseña",
                visualTransformation = PasswordVisualTransformation()
            )
            Spacer(Modifier.height(12.dp))

            TipoUsuarioDropdown(
                selected = state.tipoUsuario,
                onSelected = onTipoUsuarioChange
            )
            Spacer(Modifier.height(12.dp))

            if (state.error != null) {
                Text(state.error, color = MaterialTheme.colorScheme.error)
                Spacer(Modifier.height(12.dp))
            }

            PrimaryButton(
                text = "Ingresar",
                isLoading = state.isLoading,
                onClick = onLoginClick
            )
        }
    }
}

/** Selector desplegable de tipo de usuario, requerido para autenticar contra AuthService. */
@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun TipoUsuarioDropdown(selected: String, onSelected: (String) -> Unit) {
    var expanded by remember { mutableStateOf(false) }
    ExposedDropdownMenuBox(expanded = expanded, onExpandedChange = { expanded = it }) {
        AppTextField(
            value = selected,
            onValueChange = {},
            label = "Tipo de usuario",
            readOnly = true,
            modifier = Modifier.menuAnchor(ExposedDropdownMenuAnchorType.PrimaryNotEditable, true)
        )
        ExposedDropdownMenu(expanded = expanded, onDismissRequest = { expanded = false }) {
            TIPOS_USUARIO.forEach { tipo ->
                DropdownMenuItem(text = { Text(tipo) }, onClick = { onSelected(tipo); expanded = false })
            }
        }
    }
}
