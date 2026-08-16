package cr.ac.cuc.carnetdigital.usuario.core.network

import cr.ac.cuc.carnetdigital.usuario.core.session.SessionManager
import okhttp3.Interceptor
import okhttp3.Response

/** Agrega el token guardado a toda request saliente para que las pantallas no repitan este código. */
class AuthInterceptor(private val sessionManager: SessionManager) : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        val original = chain.request()
        val token = sessionManager.accessToken
            ?: return chain.proceed(original)

        val authenticated = original.newBuilder()
            .header("Authorization", "Bearer $token")
            .build()
        return chain.proceed(authenticated)
    }
}
