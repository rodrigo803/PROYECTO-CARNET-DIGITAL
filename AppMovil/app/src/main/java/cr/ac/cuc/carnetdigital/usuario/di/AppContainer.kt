package cr.ac.cuc.carnetdigital.usuario.di

import cr.ac.cuc.carnetdigital.usuario.BuildConfig
import cr.ac.cuc.carnetdigital.usuario.core.session.SessionManager
import cr.ac.cuc.carnetdigital.usuario.data.remote.AuthApi
import cr.ac.cuc.carnetdigital.usuario.data.remote.UsuarioApi
import cr.ac.cuc.carnetdigital.usuario.data.repository.AuthRepositoryImpl
import cr.ac.cuc.carnetdigital.usuario.data.repository.UsuarioRepositoryImpl
import cr.ac.cuc.carnetdigital.usuario.domain.repository.AuthRepository
import cr.ac.cuc.carnetdigital.usuario.domain.repository.UsuarioRepository
import cr.ac.cuc.carnetdigital.usuario.settings.AppSettings
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

/** Contenedor manual de dependencias para mantener la creación de servicios fuera de la interfaz. */
class AppContainer(sessionManager: SessionManager, appSettings: AppSettings) {
    // emulator: URL fija (10.0.2.2) con bypass TLS. device: IP editable desde Ajustes,
    // siempre por HTTP plano en el puerto 5000 del Gateway.
    private val baseUrl = if (BuildConfig.USE_TLS_BYPASS) {
        BuildConfig.GATEWAY_BASE_URL
    } else {
        "http://${appSettings.gatewayIp()}:5000/"
    }

    private val retrofit = Retrofit.Builder()
        .baseUrl(baseUrl)
        .client(NetworkClientFactory.create(sessionManager))
        .addConverterFactory(GsonConverterFactory.create())
        .build()

    private val authApi: AuthApi = retrofit.create(AuthApi::class.java)
    val authRepository: AuthRepository = AuthRepositoryImpl(authApi)

    private val usuarioApi: UsuarioApi = retrofit.create(UsuarioApi::class.java)
    val usuarioRepository: UsuarioRepository = UsuarioRepositoryImpl(usuarioApi)
}
