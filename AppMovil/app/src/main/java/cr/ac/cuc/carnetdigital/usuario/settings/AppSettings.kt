package cr.ac.cuc.carnetdigital.usuario.settings

import android.app.Activity
import android.content.Context
import android.content.Intent

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

    fun gatewayIp(): String = preferences.getString(KEY_GATEWAY_IP, DEFAULT_GATEWAY_IP) ?: DEFAULT_GATEWAY_IP

    fun setGatewayIp(ip: String, activity: Activity) {
        preferences.edit().putString(KEY_GATEWAY_IP, ip).commit()

        // recreate() no alcanza acá: el ViewModelStore sobrevive a recreate() y los
        // ViewModels ya creados siguen usando el Retrofit viejo (con la IP vieja).
        // Reiniciamos el proceso completo para que todo se reconstruya de cero.
        val intent = activity.packageManager.getLaunchIntentForPackage(activity.packageName)
        intent?.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK)
        activity.startActivity(intent)
        Runtime.getRuntime().exit(0)
    }

    private companion object {
        const val PREFS_NAME = "app_settings"
        const val KEY_THEME_MODE = "theme_mode"
        const val KEY_LANGUAGE = "language"
        const val KEY_GATEWAY_IP = "gateway_ip"
        const val DEFAULT_GATEWAY_IP = "192.168.18.75"
    }
}

enum class ThemeMode { SYSTEM, LIGHT, DARK }
