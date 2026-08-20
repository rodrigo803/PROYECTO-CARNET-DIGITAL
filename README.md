# 🪪 PROYECTO-CARNET-DIGITAL

## 📋 Información del Curso
* **Institución:** Colegio Universitario de Cartago (CUC)
* **Curso:** Programación V
* **Profesor:** Ing. Gabriel González Solano
* **Período:** II Cuatrimestre, 2026

## 👥 Integrantes del Equipo
* **Integrante 1:** Rodrigo Rene Elias Ramirez / rodrigo.elias.ramirez@cuc.cr
* **Integrante 2:** Sergio Alejandro Monge Moya / 305500184@cuc.cr
* **Integrante 3:** Dennis Segura Badilla / sergio.segura.badilla@cuc.cr

---

## 📝 Descripción del Proyecto
Este sistema nace por directriz de la decanatura del CUC con el objetivo de fortalecer las medidas de seguridad en el ingreso a la institución mediante el uso del carnet institucional de forma digital. 

El propósito principal es solucionar el inconveniente que experimentan los estudiantes y funcionarios cuando olvidan su carnet físico, evitando trámites administrativos lentos que retrasan el acceso y sobrecargan al personal de seguridad. La solución completa contempla una aplicación móvil nativa para Android y un backend robusto de servicios REST que valide las identidades de forma ágil y segura.

### 🎯 Alcance Actual (Tercer Entregable)
Este repositorio contiene el **Tercer Alcance** del proyecto. En esta fase decisiva, el enfoque principal es el desarrollo e integración de las **Aplicaciones Móviles** consumiendo los servicios a través del Gateway.

La aplicación móvil se divide en dos grandes perfiles de uso:

#### 1. Aplicación para Usuarios (Estudiantes / Funcionarios)
* **Autenticación (USR1):** Inicio de sesión con credenciales (email, contraseña y tipo de usuario) mediante los servicios del Gateway.
* **Perfil de Usuario (USR2):** Visualización de datos personales básicos, carrera/área y fotografía. Si el usuario no tiene fotografía, el sistema bloquea la validación y muestra la advertencia: *"No se validará el uso de esta aplicación hasta que haya registrado su fotografía"*.
* **Carnet Digital QR (USR3):** Accesible mediante un desplazamiento (swipe) hacia la izquierda desde el perfil. Muestra el código QR institucional único para ser escaneado en los accesos.

#### 2. Aplicación para Guardas de Seguridad
* **Acceso de Seguridad (GRD1 y GRD2):** Login exclusivo para el rol de seguridad. Muestra los datos del oficial logueado y habilita las herramientas de control.
* **Escáner de Validación (GRD3):** Integración con la cámara del dispositivo para leer de forma continua múltiples códigos QR. 
* **Flujo de Validación:** Al escanear, la app lee el JSON del QR, invoca al backend (vía Gateway) para cotejar la información del usuario mediante su llave primaria, y emite retroalimentación visual y sonora dependiendo de si el acceso es válido o inválido.

---

## 🛠️ Tecnologías Utilizadas
* **Frontend Móvil:** Android Nativo (Kotlin)
* **Backend:** .NET 10.0 (C#) con API Gateway
* **IDE de Desarrollo:** Visual Studio 2026 para backend / Android Studio para móvil
* **Base de Datos:** SQL Server
* **Autenticación:** JSON Web Tokens (JWT)
* **Control de Versiones:** Git (GitHub/GitLab/Azure DevOps)

---

## 📦 Entregables del Tercer Alcance (Valor: 30%)
Como parte de esta iteración final, el proyecto incluye de manera exhaustiva:
1. **Documentación de Análisis y Diseño:** Portada, introducción, diagrama de base de datos consolidado, casos de uso, diagrama de clases, matrices de pruebas técnicas (con pantallazos de evidencias), conclusiones y bibliografía.
2. **Base de Datos:** Script completo y definitivo con los ajustes aprobados por el profesor.
3. **Código Fuente Móvil y Web:** Servicios REST completos y pantallas desarrolladas, integrados en la rama `develop`.
4. **Gestión Ágil:** Historias de usuario actualizadas y Pull Requests formales hacia la rama `main` o rama final de producción.

---

## 🌿 Estructura y Flujo de Ramas (Git Flow)
Para el desarrollo ordenado del software se sigue estrictamente la topología de ramas de Git:

* `main`: Código completamente estable y probado, representativo del producto en producción.
* `develop`: Rama principal de desarrollo donde se integran las características terminadas.
* `feature/*`: Ramas temporales creadas para desarrollar una Historia de Usuario específica (ej. `feature/USR1-login-movil`). Se integran a `develop` mediante Pull Requests.
* `releases`: Gestión y preparación de las versiones de entrega.
* `bugfixes`: Correcciones rápidas dirigidas a resolver fallos detectados.

---

## 🚀 Configuración y Ejecución Local

### Prerrequisitos
1. Tener instalado el SDK de **.NET 10.0** y **SQL Server**.
2. Tener instalado **Android Studio** con el SDK de Android actualizado y un emulador (AVD) o dispositivo físico configurado.

### Pasos para levantar el proyecto
1. Clonar el repositorio de forma local:
   ```bash
   git clone https://github.com/rodrigo803/PROYECTO-CARNET-DIGITAL.git
   ```
2. Cambiar a la rama de desarrollo principal:
   ```bash
   git checkout develop
   ```
3. Ejecutar el script actualizado de Base de Datos para estructurar las tablas e insertar los datos iniciales.
4. Levantar los microservicios Backend y el **API Gateway**. *Nota: Asegúrese de que las cadenas de conexión apunten correctamente a su red local (por ejemplo, usar la IP `10.0.2.2` en el emulador de Android para acceder al `localhost` de su computadora).*
5. Abrir la carpeta del proyecto móvil en **Android Studio**:
   * Esperar a que Gradle sincronice todas las dependencias (`Sync Project with Gradle Files`).
   * Ejecutar la aplicación pulsando **Run** (`Shift + F10`) seleccionando el emulador o el dispositivo físico conectado por depuración USB/Wi-Fi.
6. Probar la aplicación validando ambos flujos: el inicio de sesión de estudiantes y el escaneo de seguridad mediante cámara.
