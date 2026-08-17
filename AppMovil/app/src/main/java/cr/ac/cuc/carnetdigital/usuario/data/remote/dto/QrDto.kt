package cr.ac.cuc.carnetdigital.usuario.data.remote.dto

import com.google.gson.annotations.SerializedName

/** Dto que envuelve la respuesta del código QR proveniente del Gateway. */
data class QrDto(
    @SerializedName(
        value = "QrImagenBase64",
        alternate = ["qrImagenBase64", "qr", "base64"]
    )
    val qrImagenBase64: String? = null
)