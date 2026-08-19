package cr.ac.cuc.carnetdigital.guarda.data.network

import cr.ac.cuc.carnetdigital.guarda.data.model.FotografiaDto
import cr.ac.cuc.carnetdigital.guarda.data.model.LoginRequest
import cr.ac.cuc.carnetdigital.guarda.data.model.LoginResponse
import cr.ac.cuc.carnetdigital.guarda.data.model.PerfilUsuarioDto
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

/** GRD1: login vía el Gateway (ruta /gateway/auth/... -> AuthService). */
interface AuthApi {
    @POST("gateway/auth/login")
    suspend fun login(@Body request: LoginRequest): Response<LoginResponse>
}

/** GRD2: perfil y fotografía vía el Gateway (ruta /gateway/usuarios/... -> Microservicio.Usuario). */
interface UsuarioApi {
    @GET("gateway/usuarios/perfil")
    suspend fun obtenerPerfil(): Response<PerfilUsuarioDto>

    @GET("gateway/usuarios/fotografia/{identificacion}")
    suspend fun obtenerFotografia(@Path("identificacion") identificacion: String): Response<FotografiaDto>
}
