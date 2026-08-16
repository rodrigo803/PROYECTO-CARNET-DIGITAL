package cr.ac.cuc.carnetdigital.usuario.di

import cr.ac.cuc.carnetdigital.usuario.BuildConfig
import cr.ac.cuc.carnetdigital.usuario.core.session.SessionManager
import cr.ac.cuc.carnetdigital.usuario.data.remote.AuthApi
import cr.ac.cuc.carnetdigital.usuario.data.remote.UsuarioApi
import cr.ac.cuc.carnetdigital.usuario.data.repository.AuthRepositoryImpl
import cr.ac.cuc.carnetdigital.usuario.data.repository.UsuarioRepositoryImpl
import cr.ac.cuc.carnetdigital.usuario.domain.repository.AuthRepository
import cr.ac.cuc.carnetdigital.usuario.domain.repository.UsuarioRepository
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

/** Contenedor manual de dependencias para mantener la creación de servicios fuera de la interfaz. */
class AppContainer(sessionManager: SessionManager) {
    private val retrofit = Retrofit.Builder()
        .baseUrl(BuildConfig.GATEWAY_BASE_URL)
        .client(NetworkClientFactory.create(sessionManager))
        .addConverterFactory(GsonConverterFactory.create())
        .build()

    private val authApi: AuthApi = retrofit.create(AuthApi::class.java)
    val authRepository: AuthRepository = AuthRepositoryImpl(authApi)

    private val usuarioApi: UsuarioApi = retrofit.create(UsuarioApi::class.java)
    val usuarioRepository: UsuarioRepository = UsuarioRepositoryImpl(usuarioApi)
}
