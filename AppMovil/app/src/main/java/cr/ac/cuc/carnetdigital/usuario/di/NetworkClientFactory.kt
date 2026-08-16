package cr.ac.cuc.carnetdigital.usuario.di

import cr.ac.cuc.carnetdigital.usuario.BuildConfig
import cr.ac.cuc.carnetdigital.usuario.core.network.AuthInterceptor
import cr.ac.cuc.carnetdigital.usuario.core.session.SessionManager
import okhttp3.OkHttpClient
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.HostnameVerifier
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager

/** Crea el cliente HTTP y limita la aceptación del certificado local no confiable a compilaciones debug. */
object NetworkClientFactory {
    fun create(sessionManager: SessionManager): OkHttpClient {
        val builder = OkHttpClient.Builder()
            .addInterceptor(AuthInterceptor(sessionManager))

        if (!BuildConfig.DEBUG || !BuildConfig.GATEWAY_BASE_URL.contains("10.0.2.2")) {
            return builder.build()
        }

        val trustManager = object : X509TrustManager {
            override fun checkClientTrusted(chain: Array<X509Certificate>, authType: String) = Unit
            override fun checkServerTrusted(chain: Array<X509Certificate>, authType: String) = Unit
            override fun getAcceptedIssuers(): Array<X509Certificate> = emptyArray()
        }
        val sslContext = SSLContext.getInstance("TLS").apply {
            init(null, arrayOf<TrustManager>(trustManager), SecureRandom())
        }
        return builder
            .sslSocketFactory(sslContext.socketFactory, trustManager)
            .hostnameVerifier(HostnameVerifier { _, _ -> true })
            .build()
    }
}
