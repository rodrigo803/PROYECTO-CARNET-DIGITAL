package cr.ac.cuc.carnetdigital.guarda.core.network

import okhttp3.Interceptor
import okhttp3.Response

class MockAuthInterceptor : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        // RECUERDA: Cuando vayas a probar el API real, cambia este texto por
        // un JWT válido que te devuelva Swagger o Postman al iniciar sesión.
        val tokenTemporal = "eyJhbGciOiJIUzI1NiIsInR5cCI6..."

        val request = chain.request().newBuilder()
            .addHeader("Authorization", "Bearer $tokenTemporal")
            .build()

        return chain.proceed(request)
    }
}