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

El propósito principal es solucionar el inconveniente que experimentan los estudiantes y funcionarios cuando olvidan su carnet físico, evitando trámites administrativos lentos que retrasan el acceso y sobrecargan al personal de seguridad. La solución completa contempla una aplicación multiplataforma y un backend robusto de servicios REST que valide las identidades de forma ágil y segura.

### 🎯 Alcance Actual (Segundo Entregable)
Este repositorio contiene el **Segundo Alcance** del proyecto. En esta fase, el enfoque principal es el **Consumo de Servicios REST a través de pantallas Web**, permitiendo la interacción directa de los usuarios y administradores con los microservicios backend.

Se ha implementado una interfaz gráfica (Frontend) completa que abarca las siguientes áreas:
* **Autenticación y Seguridad:** Pantalla de Login con validaciones, bloqueo tras 3 intentos fallidos y un sistema de autoregistro con confirmación de correo electrónico.
* **Plantilla Global:** Interfaz estandarizada con menú lateral de opciones, perfil de usuario logueado y navegación fluida entre módulos.
* **Mantenimientos y Administración (CRUDs):** Gestión integral de Usuarios, Carreras, Tipos de Usuario, Tipos de Identificación, Áreas de Trabajo, Instituciones, Roles, Módulos y Parámetros del Sistema.
* **Gestión de Carnet Digital:** Administración de fotografías de usuarios (formato Base64, relación 4:3) y generación de Códigos QR con la información de la identificación.
* **Auditoría del Sistema:** Módulo de consulta de bitácoras para registrar y auditar transacciones o errores, con filtros por fecha, usuario y acción.

---

## 🛠️ Tecnologías Utilizadas
* **Backend:** .NET 10.0 (C#)
* **Frontend:** Tecnologías Web (Framework a elección del equipo)
* **IDE de Desarrollo:** Visual Studio 2026
* **Base de Datos:** SQL Server
* **Autenticación:** JSON Web Tokens (JWT) & Refresh Tokens transaccionales

---

## 📦 Entregables del Segundo Alcance
Como parte de esta iteración, el proyecto incluye los siguientes entregables estructurados:
1. **Documentación:** Análisis y diseño enfocado en las HU atendidas, incluyendo diagramas de Base de Datos, casos de uso, diagramas de clases y la matriz de pruebas técnicas de los servicios.
2. **Base de Datos:** Script completo y actualizado.
3. **Código Fuente:** Servicios REST y componentes de las pantallas web integradas de todos los subgrupos.
4. **Gestión Ágil:** Historias de usuario documentadas y control de versiones gestionado mediante Pull Requests hacia la rama principal.

---

## 🌿 Estructura y Flujo de Ramas (Git Flow)
Para el desarrollo ordenado del software se sigue estrictamente la siguiente topología de ramas de Git:

* `main`: Código completamente estable y probado, representativo del producto en producción.
* `develop`: Rama principal de desarrollo donde se integran las características terminadas.
* `feature/*`: Ramas temporales creadas para desarrollar una Historia de Usuario específica (ej. `feature/Web1-login`). Se integran a `develop` mediante Pull Requests.
* `releases`: Gestión y preparación de las versiones de entrega.
* `bugfixes`: Correcciones rápidas dirigidas a resolver fallos detectados.

---

## 🚀 Configuración y Ejecución Local

### Prerrequisitos
1. Tener instalado el SDK de **.NET 10.0**.
2. Contar con una instancia activa de **SQL Server**.
3. Entorno de desarrollo para la tecnología Web seleccionada para consumir las APIs.

### Pasos para levantar el proyecto
1. Clonar el repositorio de forma local:
   ```bash
   git clone https://github.com/rodrigo803/PROYECTO-CARNET-DIGITAL.git
   ```
2. Cambiar a la rama de desarrollo principal (o a la rama de su feature actual):
   ```bash
   git checkout develop
   ```
3. Configurar las cadenas de conexión en el archivo de configuración del proyecto Backend (.NET) apuntando a su instancia de SQL Server.
4. Ejecutar el script actualizado de Base de Datos proveído en la documentación para estructurar las tablas e insertar los datos iniciales necesarios.
5. Compilar y ejecutar el proyecto Backend para levantar los microservicios REST.
6. Levantar el proyecto Frontend Web, asegurando que las peticiones apunten correctamente al endpoint local del backend.
