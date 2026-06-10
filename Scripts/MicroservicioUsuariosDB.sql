CREATE DATABASE MicroservicioUsuariosDB;
GO
USE MicroservicioUsuariosDB;
GO

-- =========================================================================
-- SRV12: Estado de Usuarios (Catálogo interno de tu microservicio)
-- =========================================================================
CREATE TABLE EstadoUsuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);

-- Insertamos los estados básicos por defecto para que puedas probar
INSERT INTO EstadoUsuario (Nombre) VALUES ('Activo'), ('Inactivo'), ('Pendiente_Confirmacion');

-- =========================================================================
-- SRV10, SRV11, SRV13, SRV14: Tabla Central de Usuarios
-- =========================================================================
CREATE TABLE Usuarios (
    Identificacion VARCHAR(100) PRIMARY KEY, 
    Email VARCHAR(200) NOT NULL,
    NombreCompleto VARCHAR(300) NOT NULL,
    ContrasenaEncriptada VARCHAR(500) NOT NULL,

    -- SRV13: Fotografía en Base64
    FotografiaBase64 VARCHAR(MAX) NULL,

    -- SRV11: Datos para controlar el autoregistro
    TokenConfirmacion VARCHAR(100) NULL,
    FechaExpiracionToken DATETIME NULL,

    -- Relación INTERNA (Sí lleva Foreign Key porque la tabla está arriba)
    EstadoId INT NOT NULL FOREIGN KEY REFERENCES EstadoUsuario(Id),

    -- Relaciones EXTERNAS (Guardan el Id, pero NO llevan Foreign Key porque 
    -- esas tablas pertenecen a los microservicios de tus compañeros)
    TipoIdentificacionId INT NOT NULL, -- SRV6
    TipoUsuarioId INT NOT NULL,        -- SRV5
    RolId INT NOT NULL                 -- SRV8
);