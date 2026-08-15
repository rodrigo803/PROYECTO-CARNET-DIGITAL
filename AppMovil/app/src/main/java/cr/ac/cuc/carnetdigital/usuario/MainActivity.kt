package cr.ac.cuc.carnetdigital.usuario

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.lifecycle.viewmodel.compose.viewModel
import cr.ac.cuc.carnetdigital.usuario.core.session.SessionManager
import cr.ac.cuc.carnetdigital.usuario.di.AppContainer
import cr.ac.cuc.carnetdigital.usuario.ui.auth.LoginRoute
import cr.ac.cuc.carnetdigital.usuario.ui.auth.LoginViewModel
import cr.ac.cuc.carnetdigital.usuario.ui.auth.LoginViewModelFactory
import cr.ac.cuc.carnetdigital.usuario.ui.perfil.PerfilRoute
import cr.ac.cuc.carnetdigital.usuario.ui.perfil.PerfilViewModel
import cr.ac.cuc.carnetdigital.usuario.ui.perfil.PerfilViewModelFactory
import cr.ac.cuc.carnetdigital.usuario.ui.theme.AppUsuarioTheme

/** Activity principal: arma las dependencias y decide entre Login y Home según haya sesión activa. */
class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            val sessionManager = remember { SessionManager(applicationContext) }
            val container = remember { AppContainer(sessionManager) }
            var loggedIn by remember { mutableStateOf(sessionManager.isLoggedIn()) }

            AppUsuarioTheme {
                if (loggedIn) {
                    val perfilViewModel: PerfilViewModel = viewModel(
                        factory = PerfilViewModelFactory(container.usuarioRepository)
                    )
                    PerfilRoute(
                        viewModel = perfilViewModel,
                        onLogout = {
                            sessionManager.clearSession()
                            loggedIn = false
                        }
                    )
                } else {
                    val viewModel: LoginViewModel = viewModel(
                        factory = LoginViewModelFactory(container.authRepository, sessionManager)
                    )
                    LoginRoute(viewModel = viewModel, onLoginSuccess = { loggedIn = true })
                }
            }
        }
    }
}
