package cr.ac.cuc.carnetdigital.guarda.data.network

import cr.ac.cuc.carnetdigital.guarda.data.session.SessionManager
import okhttp3.Interceptor
import okhttp3.Response

/**
 * Reinyecta automáticamente "Authorization: Bearer <token>" en cada request,
 * igual que BearerTokenHandler en CarnetDigital.Frontend. Así ni LoginScreen
 * ni PerfilScreen tienen que preocuparse por adjuntar el token a mano.
 */
class AuthInterceptor : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        val original = chain.request()
        val token = SessionManager.obtenerToken()

        val request = if (!token.isNullOrBlank()) {
            original.newBuilder()
                .addHeader("Authorization", "Bearer $token")
                .build()
        } else {
            original
        }

        return chain.proceed(request)
    }
}
