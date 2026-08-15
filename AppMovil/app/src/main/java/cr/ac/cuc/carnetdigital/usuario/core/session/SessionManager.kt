package cr.ac.cuc.carnetdigital.usuario.core.session

import android.content.Context

/** Persiste la sesión activa (tokens del login) siguiendo el mismo patrón de AppSettings del ejemplo del profe. */
class SessionManager(context: Context) {
    private val preferences = context.getSharedPreferences("session", Context.MODE_PRIVATE)

    val accessToken: String?
        get() = preferences.getString(KEY_ACCESS_TOKEN, null)

    val refreshToken: String?
        get() = preferences.getString(KEY_REFRESH_TOKEN, null)

    val usuarioId: Int
        get() = preferences.getInt(KEY_USUARIO_ID, -1)

    fun isLoggedIn(): Boolean = accessToken != null

    fun saveSession(accessToken: String, refreshToken: String, usuarioId: Int) {
        preferences.edit()
            .putString(KEY_ACCESS_TOKEN, accessToken)
            .putString(KEY_REFRESH_TOKEN, refreshToken)
            .putInt(KEY_USUARIO_ID, usuarioId)
            .apply()
    }

    fun clearSession() {
        preferences.edit().clear().apply()
    }

    private companion object {
        const val KEY_ACCESS_TOKEN = "access_token"
        const val KEY_REFRESH_TOKEN = "refresh_token"
        const val KEY_USUARIO_ID = "usuario_id"
    }
}
