package cr.ac.cuc.carnetdigital.guarda.data.session

import android.content.Context
import android.content.SharedPreferences

/**
 * Guarda el token de sesión en SharedPreferences. Es el equivalente, en la app
 * móvil, al SessionTokenProvider (Session de ASP.NET Core) del frontend web:
 * un solo lugar donde se guarda y se lee el token entre pantallas.
 */
object SessionManager {

    private const val PREFS_NAME = "guarda_session"
    private const val KEY_TOKEN = "access_token"

    private lateinit var prefs: SharedPreferences

    fun init(context: Context) {
        prefs = context.applicationContext
            .getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
    }

    fun guardarToken(token: String) {
        prefs.edit().putString(KEY_TOKEN, token).apply()
    }

    fun obtenerToken(): String? = prefs.getString(KEY_TOKEN, null)

    fun estaAutenticado(): Boolean = !obtenerToken().isNullOrBlank()

    fun cerrarSesion() {
        prefs.edit().remove(KEY_TOKEN).apply()
    }
}
