package cr.ac.cuc.carnetdigital.guarda.data.model

import com.google.gson.annotations.SerializedName

data class QrContenidoDto(
    @SerializedName("Identificacion")
    val identificacion: String?,

    @SerializedName("Nombre")
    val nombre: String?,

    @SerializedName("Tipo")
    val tipo: String?,

    @SerializedName("Institucion")
    val institucion: String?,

    @SerializedName("CarrerasAreas")
    val carrerasAreas: String?,

    @SerializedName("Vencimiento")
    val vencimiento: String?
)
