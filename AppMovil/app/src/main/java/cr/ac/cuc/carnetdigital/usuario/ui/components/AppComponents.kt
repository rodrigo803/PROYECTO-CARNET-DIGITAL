package cr.ac.cuc.carnetdigital.usuario.ui.components

import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.size
import androidx.compose.material3.Button
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.VisualTransformation
import androidx.compose.ui.unit.dp

/** Campo de texto reutilizable con etiqueta, validación visual y ancho consistente. */
@Composable
fun AppTextField(
    value: String,
    onValueChange: (String) -> Unit,
    label: String,
    isError: Boolean = false,
    supportingText: String? = null,
    readOnly: Boolean = false,
    visualTransformation: VisualTransformation = VisualTransformation.None,
    modifier: Modifier = Modifier
) {
    OutlinedTextField(
        value = value,
        onValueChange = onValueChange,
        label = { Text(label) },
        isError = isError,
        supportingText = supportingText?.let { { Text(it) } },
        readOnly = readOnly,
        visualTransformation = visualTransformation,
        singleLine = true,
        modifier = modifier.fillMaxWidth()
    )
}

/** Botón principal reutilizable para acciones de envío en formularios. Muestra progreso mientras espera al servidor. */
@Composable
fun PrimaryButton(text: String, enabled: Boolean = true, isLoading: Boolean = false, onClick: () -> Unit) {
    Button(onClick = onClick, enabled = enabled && !isLoading, modifier = Modifier.fillMaxWidth()) {
        if (isLoading) {
            CircularProgressIndicator(modifier = Modifier.size(20.dp))
        } else {
            Text(text)
        }
    }
}
