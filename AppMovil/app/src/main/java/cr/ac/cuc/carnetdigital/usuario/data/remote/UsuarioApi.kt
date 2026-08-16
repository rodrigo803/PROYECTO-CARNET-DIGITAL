package cr.ac.cuc.carnetdigital.usuario.data.remote

import cr.ac.cuc.carnetdigital.usuario.data.remote.dto.FotografiaDto
import cr.ac.cuc.carnetdigital.usuario.data.remote.dto.PerfilDto
import cr.ac.cuc.carnetdigital.usuario.data.remote.dto.QrDto
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Path

/** Contrato Retrofit para los endpoints de perfil del usuario autenticado, expuestos por el Gateway. */
interface UsuarioApi {
    @GET("gateway/usuarios/perfil")
    suspend fun obtenerPerfil(): Response<PerfilDto>

    @GET("gateway/usuarios/fotografia/{identificacion}")
    suspend fun obtenerFotografia(@Path("identificacion") identificacion: String): Response<FotografiaDto>

    // Ruta correcta que cruza el Gateway de YARP hacia el microservicio
    @GET("gateway/usuarios/qr/{identificacion}")
    suspend fun obtenerQr(@Path("identificacion") identificacion: String): Response<QrDto>
}
