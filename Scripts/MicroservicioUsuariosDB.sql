USE master;
GO
DROP DATABASE IF EXISTS MicroservicioUsuariosDB;
GO

CREATE DATABASE MicroservicioUsuariosDB;
GO

USE MicroservicioUsuariosDB;
GO

-- =========================================================================
-- SRV12: Estado de Usuarios
-- =========================================================================
CREATE TABLE EstadoUsuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);

-- Insertamos los estados básicos
INSERT INTO EstadoUsuario (Nombre) VALUES ('Activo'), ('Inactivo'), ('Pendiente_Confirmacion');

-- =========================================================================
-- SRV10, SRV11, SRV13, SRV14: Tabla Central de Usuarios
-- =========================================================================
CREATE TABLE Usuarios (
    Identificacion VARCHAR(100) PRIMARY KEY, 
    Email VARCHAR(200) NOT NULL,
    NombreCompleto VARCHAR(300) NOT NULL,
    ContrasenaEncriptada VARCHAR(500) NOT NULL,

    -- SRV13: Fotografía
    FotografiaBase64 VARCHAR(MAX) NULL,

    -- SRV11: Datos para controlar el autoregistro
    TokenConfirmacion VARCHAR(100) NULL,
    FechaExpiracionToken DATETIME NULL,

    -- Relación INTERNA
    EstadoId INT NOT NULL FOREIGN KEY REFERENCES EstadoUsuario(Id),

    -- Relaciones EXTERNAS (IDs base)
    TipoIdentificacionId INT NOT NULL, 
    TipoUsuarioId INT NOT NULL,        
    RolId INT NOT NULL,

    -- NUEVAS COLUMNAS PARA MICROSERVICIOS (Almacenan listas como JSON)
    -- Aunque en C# los manejas como List<int>, en SQL son VARCHAR(MAX)
    InstitucionesIds VARCHAR(MAX) NOT NULL DEFAULT '[]',
    CarrerasIds VARCHAR(MAX) NOT NULL DEFAULT '[]',
    AreasIds VARCHAR(MAX) NOT NULL DEFAULT '[]'
);
GO