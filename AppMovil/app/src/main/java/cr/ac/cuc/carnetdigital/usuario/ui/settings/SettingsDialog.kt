package cr.ac.cuc.carnetdigital.usuario.ui.settings

import android.app.Activity
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.BrightnessAuto
import androidx.compose.material.icons.filled.DarkMode
import androidx.compose.material.icons.filled.LightMode
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import cr.ac.cuc.carnetdigital.usuario.R
import cr.ac.cuc.carnetdigital.usuario.settings.AppSettings
import cr.ac.cuc.carnetdigital.usuario.settings.ThemeMode

/** Diálogo de ajustes (tema e idioma), invocado desde USR1 y USR2. */
@Composable
fun SettingsDialog(
    settings: AppSettings,
    onThemeChanged: (ThemeMode) -> Unit,
    onDismiss: () -> Unit
) {
    val activity = LocalContext.current as Activity

    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text(stringResource(R.string.ajustes_titulo)) },
        text = {
            Column {
                Text(stringResource(R.string.ajustes_tema_label))
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceEvenly
                ) {
                    IconButton(onClick = {
                        settings.themeMode = ThemeMode.SYSTEM
                        onThemeChanged(ThemeMode.SYSTEM)
                    }) {
                        Icon(Icons.Default.BrightnessAuto, contentDescription = stringResource(R.string.ajustes_tema_auto))
                    }
                    IconButton(onClick = {
                        settings.themeMode = ThemeMode.LIGHT
                        onThemeChanged(ThemeMode.LIGHT)
                    }) {
                        Icon(Icons.Default.LightMode, contentDescription = stringResource(R.string.ajustes_tema_claro))
                    }
                    IconButton(onClick = {
                        settings.themeMode = ThemeMode.DARK
                        onThemeChanged(ThemeMode.DARK)
                    }) {
                        Icon(Icons.Default.DarkMode, contentDescription = stringResource(R.string.ajustes_tema_oscuro))
                    }
                }

                Text(stringResource(R.string.ajustes_idioma_label))
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceEvenly
                ) {
                    TextButton(onClick = { settings.setLanguage("es", activity) }) {
                        Text(stringResource(R.string.ajustes_idioma_es))
                    }
                    TextButton(onClick = { settings.setLanguage("en", activity) }) {
                        Text(stringResource(R.string.ajustes_idioma_en))
                    }
                }
            }
        },
        confirmButton = {
            TextButton(onClick = onDismiss) {
                Text(stringResource(R.string.ajustes_cerrar))
            }
        }
    )
}
