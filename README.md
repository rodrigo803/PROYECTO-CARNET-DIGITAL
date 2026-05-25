PROYECTO-CARNET-DIGITAL

## 📋 Información del Curso
* **Institución:** Colegio Universitario de Cartago (CUC)
* **Curso:** Programación V
* **Profesor:** Ing. Gabriel González Solano
* **Período:** II Cuatrimestre, 2026

## 👥 Integrantes del Equipo
* Integrante 1 Rodrigo Rene Elias Ramirez / rodrigo.elias.ramirez@cuc.cr
* Integrante 2 Sergio Alejandro Monge Moya / 305500184@cuc.cr
* Integrante 3 Dennis Segura Badilla / sergio.segura.badilla@cuc.cr

---

## 📝 Descripción del Proyecto
Este sistema nace por directriz de la decanatura del CUC con el objetivo de fortalecer las medidas de seguridad en el ingreso a la institución mediante el uso del carnet institucional de forma digital. 

El propósito principal es solucionar el inconveniente que experimentan los estudiantes y funcionarios cuando olvidan su carnet físico, evitando trámites administrativos lentos que retrasan el acceso y sobrecargan al personal de seguridad. La solución completa contempla una aplicación móvil multiplataforma (en Flutter) y un backend robusto de servicios REST que valide las identidades de forma ágil y segura.

### 🎯 Alcance Actual (Primer Entregable)
Este repositorio contiene el **Primer Alcance** del proyecto, enfocado exclusivamente en el diseño de la base de datos y la programación de los **Servicios REST (Microservicios Backend)** que darán soporte a toda la plataforma.

---

## 🛠️ Tecnologías Utilizadas
* **Backend:** .NET 10.0 (C#)
* **IDE de Desarrollo:** Visual Studio 2022
* **Base de Datos:** SQL Server
* **Autenticación:** JSON Web Tokens (JWT) & Refresh Tokens transaccionales

---

## 🌿 Estructura y Flujo de Ramas (Git Flow)
Para el desarrollo ordenado del software se sigue estrictamente la siguiente topología de ramas de Git:

* `main`: Código completamente estable y probado, representativo del producto en producción.
* `develop`: Rama principal de desarrollo donde se integran las características terminadas.
* `feature/*`: Ramas temporales creadas para desarrollar una Historia de Usuario específica (ej. `feature/SRV1-login`). Se integran a `develop` mediante Pull Requests.
* `releases`: Gestión y preparación de las versiones de entrega.
* `bugfixes`: Correcciones rápidas dirigidas a resolver fallos detectados.

---

## 🚀 Configuración y Ejecución Local

### Prerrequisitos
1. Tener instalado el SDK de **.NET 10.0**.
2. Contar con una instancia activa de **SQL Server Express u otra**.

### Pasos para levantar el proyecto
1. Clonar el repositorio de forma local:
   ```bash
   git clone https://github.com/rodrigo803/PROYECTO-CARNET-DIGITAL.git
