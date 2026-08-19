package cr.ac.cuc.carnetdigital.guarda.data.remote.dto

import com.google.gson.annotations.SerializedName

data class UsuarioBackendDto(
    @SerializedName(value = "Identificacion", alternate = ["identificacion"])
    val identificacion: String?,

    @SerializedName(value = "NombreCompleto", alternate = ["nombreCompleto", "nombre"])
    val nombreCompleto: String?,

    @SerializedName(value = "TipoUsuario", alternate = ["tipoUsuario"])
    val tipoUsuario: String?,

    // Según tu C#, parece que usan EstadoId. Asumiremos que 1 es "Activo"
    @SerializedName(value = "EstadoId", alternate = ["estadoId", "estado"])
    val estadoId: Int?
)