package cr.ac.cuc.carnetdigital.guarda

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Surface
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import cr.ac.cuc.carnetdigital.guarda.data.session.SessionManager
import cr.ac.cuc.carnetdigital.guarda.ui.login.LoginScreen
import cr.ac.cuc.carnetdigital.guarda.ui.perfil.PerfilScreen

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        SessionManager.init(applicationContext)

        setContent {
            MaterialTheme {
                Surface(modifier = Modifier) {
                    AppGuardaNavHost()
                }
            }
        }
    }
}

/**
 * Navegación simple entre las dos pantallas de este alcance (GRD1 y GRD2).
 * No usa Navigation Compose a propósito, para no agregar una dependencia
 * nueva que el resto del proyecto (AppUsuario) no trae — si tu equipo ya la
 * usa en otra app, aquí es fácil migrar a un NavHost real.
 */
@androidx.compose.runtime.Composable
private fun AppGuardaNavHost() {
    var autenticado by remember { mutableStateOf(SessionManager.estaAutenticado()) }

    if (autenticado) {
        PerfilScreen()
    } else {
        LoginScreen(onLoginExitoso = { autenticado = true })
    }
}
