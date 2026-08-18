package cr.ac.cuc.carnetdigital.usuario

import android.content.Context
import android.content.res.Configuration
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.lifecycle.viewmodel.compose.viewModel
import cr.ac.cuc.carnetdigital.usuario.core.session.SessionManager
import cr.ac.cuc.carnetdigital.usuario.di.AppContainer
import cr.ac.cuc.carnetdigital.usuario.settings.AppSettings
import cr.ac.cuc.carnetdigital.usuario.settings.ThemeMode
import cr.ac.cuc.carnetdigital.usuario.ui.auth.LoginRoute
import cr.ac.cuc.carnetdigital.usuario.ui.auth.LoginViewModel
import cr.ac.cuc.carnetdigital.usuario.ui.auth.LoginViewModelFactory
import cr.ac.cuc.carnetdigital.usuario.ui.perfil.PerfilRoute
import cr.ac.cuc.carnetdigital.usuario.ui.perfil.PerfilViewModel
import cr.ac.cuc.carnetdigital.usuario.ui.perfil.PerfilViewModelFactory
import cr.ac.cuc.carnetdigital.usuario.ui.theme.AppUsuarioTheme
import java.util.Locale

/** Activity principal: arma las dependencias y decide entre Login y Home según haya sesión activa. */
class MainActivity : ComponentActivity() {

    override fun attachBaseContext(newBase: Context) {
        val languageTag = AppSettings(newBase).language()
        if (languageTag.isEmpty()) {
            super.attachBaseContext(newBase)
            return
        }
        val locale = Locale.forLanguageTag(languageTag)
        Locale.setDefault(locale)
        val configuration = Configuration(newBase.resources.configuration)
        configuration.setLocale(locale)
        super.attachBaseContext(newBase.createConfigurationContext(configuration))
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            val sessionManager = remember { SessionManager(applicationContext) }
            val container = remember { AppContainer(sessionManager) }
            var loggedIn by remember { mutableStateOf(sessionManager.isLoggedIn()) }

            val settings = remember { AppSettings(this) }
            var themeMode by remember { mutableStateOf(settings.themeMode) }
            val darkTheme = when (themeMode) {
                ThemeMode.SYSTEM -> isSystemInDarkTheme()
                ThemeMode.LIGHT -> false
                ThemeMode.DARK -> true
            }

            AppUsuarioTheme(darkTheme = darkTheme) {
                if (loggedIn) {
                    val perfilViewModel: PerfilViewModel = viewModel(
                        factory = PerfilViewModelFactory(container.usuarioRepository)
                    )
                    PerfilRoute(
                        viewModel = perfilViewModel,
                        onLogout = {
                            sessionManager.clearSession()
                            loggedIn = false
                        },
                        settings = settings,
                        onThemeChanged = { themeMode = it }
                    )
                } else {
                    val viewModel: LoginViewModel = viewModel(
                        factory = LoginViewModelFactory(container.authRepository, sessionManager)
                    )
                    LoginRoute(
                        viewModel = viewModel,
                        onLoginSuccess = { loggedIn = true },
                        settings = settings,
                        onThemeChanged = { themeMode = it }
                    )
                }
            }
        }
    }
}
