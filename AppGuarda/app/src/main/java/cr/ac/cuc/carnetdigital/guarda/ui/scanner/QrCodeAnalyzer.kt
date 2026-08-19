package cr.ac.cuc.carnetdigital.guarda.ui.scanner

import androidx.annotation.OptIn
import androidx.camera.core.ExperimentalGetImage
import androidx.camera.core.ImageAnalysis
import androidx.camera.core.ImageProxy
import com.google.mlkit.vision.barcode.BarcodeScannerOptions
import com.google.mlkit.vision.barcode.BarcodeScanning
import com.google.mlkit.vision.barcode.common.Barcode
import com.google.mlkit.vision.common.InputImage

/**
 * Clase que procesa los fotogramas de la cámara usando Google ML Kit
 * buscando exclusivamente formatos QR para mayor rendimiento.
 */
class QrCodeAnalyzer(
    private val onQrCodeScanned: (String) -> Unit
) : ImageAnalysis.Analyzer {

    // Configuramos ML Kit para que solo busque Códigos QR (ahorra batería y es más rápido)
    private val options = BarcodeScannerOptions.Builder()
        .setBarcodeFormats(Barcode.FORMAT_QR_CODE)
        .build()

    private val scanner = BarcodeScanning.getClient(options)

    @OptIn(ExperimentalGetImage::class)
    override fun analyze(imageProxy: ImageProxy) {
        val mediaImage = imageProxy.image
        if (mediaImage != null) {
            val image = InputImage.fromMediaImage(mediaImage, imageProxy.imageInfo.rotationDegrees)

            scanner.process(image)
                .addOnSuccessListener { barcodes ->
                    // Si encuentra un código de barras/QR, extraemos el texto
                    for (barcode in barcodes) {
                        barcode.rawValue?.let { qrValue ->
                            onQrCodeScanned(qrValue)
                        }
                    }
                }
                .addOnFailureListener {
                    // Aquí podríamos manejar errores, pero en escaneo continuo es normal que falle
                    // cuando no hay un QR en pantalla.
                }
                .addOnCompleteListener {
                    // ES VITAL cerrar el proxy de la imagen para que CameraX envíe el siguiente fotograma
                    imageProxy.close()
                }
        } else {
            imageProxy.close()
        }
    }
}