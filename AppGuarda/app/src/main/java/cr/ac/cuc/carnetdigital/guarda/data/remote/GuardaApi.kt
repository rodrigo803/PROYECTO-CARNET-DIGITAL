package cr.ac.cuc.carnetdigital.guarda.data.remote

import cr.ac.cuc.carnetdigital.guarda.data.remote.dto.LoginRequestDto
import cr.ac.cuc.carnetdigital.guarda.data.remote.dto.LoginResponseDto
import cr.ac.cuc.carnetdigital.guarda.data.remote.dto.UsuarioBackendDto
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

interface GuardaApi {
    // Apunta al microservicio Auth (puerto 7181 a través del Gateway)
    @POST("gateway/auth/login")
    suspend fun login(@Body request: LoginRequestDto): Response<LoginResponseDto>

    @GET("gateway/usuarios/{identificacion}")
    suspend fun obtenerUsuarioPorId(@Path("identificacion") identificacion: String): Response<UsuarioBackendDto>
}

