package cr.ac.cuc.carnetdigital.usuario.ui.theme

import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable

private val DarkColorScheme = darkColorScheme(
    primary = CucDarkPrimary,
    onPrimary = CucDarkOnPrimary,
    secondary = CucDarkSecondary,
    error = CucDarkError,
    background = CucDarkBackground,
    onBackground = CucDarkOnBackground,
    surface = CucDarkSurface,
    onSurface = CucDarkOnSurface,
    surfaceVariant = CucDarkSurfaceVariant,
    outline = CucDarkOutline
)

private val LightColorScheme = lightColorScheme(
    primary = CucLightPrimary,
    onPrimary = CucLightOnPrimary,
    secondary = CucLightSecondary,
    error = CucLightError,
    background = CucLightBackground,
    onBackground = CucLightOnBackground,
    surface = CucLightSurface,
    onSurface = CucLightOnSurface,
    surfaceVariant = CucLightSurfaceVariant,
    outline = CucLightOutline
)

@Composable
fun AppUsuarioTheme(
    darkTheme: Boolean = isSystemInDarkTheme(),
    content: @Composable () -> Unit
) {
    val colorScheme = if (darkTheme) DarkColorScheme else LightColorScheme

    MaterialTheme(
        colorScheme = colorScheme,
        typography = Typography,
        content = content
    )
}
