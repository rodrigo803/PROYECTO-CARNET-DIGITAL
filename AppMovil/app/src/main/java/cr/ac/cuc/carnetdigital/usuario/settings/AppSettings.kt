package cr.ac.cuc.carnetdigital.usuario.settings

import android.app.Activity
import android.content.Context

/** Preferencias de apariencia/idioma, separadas de la sesión (SharedPreferences "app_settings"). */
class AppSettings(context: Context) {
    private val preferences = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)

    var themeMode: ThemeMode
        get() = ThemeMode.valueOf(
            preferences.getString(KEY_THEME_MODE, ThemeMode.SYSTEM.name) ?: ThemeMode.SYSTEM.name
        )
        set(value) {
            preferences.edit().putString(KEY_THEME_MODE, value.name).apply()
        }

    fun language(): String = preferences.getString(KEY_LANGUAGE, "") ?: ""

    fun setLanguage(languageTag: String, activity: Activity) {
        preferences.edit().putString(KEY_LANGUAGE, languageTag).apply()
        activity.recreate()
    }

    private companion object {
        const val PREFS_NAME = "app_settings"
        const val KEY_THEME_MODE = "theme_mode"
        const val KEY_LANGUAGE = "language"
    }
}

enum class ThemeMode { SYSTEM, LIGHT, DARK }
