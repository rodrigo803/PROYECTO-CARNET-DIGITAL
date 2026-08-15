package cr.ac.cuc.carnetdigital.usuario.data.remote

import cr.ac.cuc.carnetdigital.usuario.data.remote.dto.LoginRequestDto
import cr.ac.cuc.carnetdigital.usuario.data.remote.dto.LoginResponseDto
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

/** Contrato Retrofit que representa los endpoints de autenticación expuestos por el Gateway. */
interface AuthApi {
    @POST("gateway/auth/login")
    suspend fun login(@Body request: LoginRequestDto): Response<LoginResponseDto>
}
