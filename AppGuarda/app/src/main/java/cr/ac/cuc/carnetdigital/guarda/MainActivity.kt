package cr.ac.cuc.carnetdigital.guarda

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.runtime.*
import cr.ac.cuc.carnetdigital.guarda.core.security.TokenManager
import cr.ac.cuc.carnetdigital.guarda.ui.login.LoginScreen
import cr.ac.cuc.carnetdigital.guarda.ui.scanner.ScannerScreen
import cr.ac.cuc.carnetdigital.guarda.ui.theme.AppGuardaTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val tokenManager = TokenManager(this)

        setContent {
            AppGuardaTheme {
                // Si ya existe un token guardado, salta directo al escáner; si no, muestra el login
                var isLoggedIn by remember { mutableStateOf(!tokenManager.getToken().isNullOrEmpty()) }

                if (isLoggedIn) {
                    ScannerScreen()
                } else {
                    LoginScreen(
                        onLoginSuccess = {
                            isLoggedIn = true
                        }
                    )
                }
            }
        }
    }
}