package cr.ac.cuc.carnetdigital.guarda.settings

import android.app.Activity
import android.content.Context
import android.content.Intent

/** Preferencias de la app (hoy solo la IP del Gateway), en SharedPreferences propias. */
class AppSettings(context: Context) {
    private val preferences = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)

    fun gatewayIp(): String = preferences.getString(KEY_GATEWAY_IP, DEFAULT_GATEWAY_IP) ?: DEFAULT_GATEWAY_IP

    fun setGatewayIp(ip: String, activity: Activity) {
        // .commit() (síncrono): con .apply() el exit(0) de abajo puede matar el
        // proceso antes de que la escritura a disco termine, perdiendo la IP.
        preferences.edit().putString(KEY_GATEWAY_IP, ip).commit()

        // RetrofitClient es un object singleton que arma la baseUrl una sola vez;
        // reiniciar el proceso completo es la única forma de que tome la IP nueva
        // (recreate() no alcanza: no limpia el ViewModelStore ni reinicializa objects).
        val intent = activity.packageManager.getLaunchIntentForPackage(activity.packageName)
        intent?.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK)
        activity.startActivity(intent)
        Runtime.getRuntime().exit(0)
    }

    private companion object {
        const val PREFS_NAME = "app_settings"
        const val KEY_GATEWAY_IP = "gateway_ip"
        const val DEFAULT_GATEWAY_IP = "192.168.18.75"
    }
}
