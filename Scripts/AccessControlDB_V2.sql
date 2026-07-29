-- Script de Dennis (AccessControlDB_V2.sql), ajustado para ser idempotente
-- (igual que AuditLogDB.sql / ParameterDB_V2.sql) - el original no tenía
-- guard en CREATE DATABASE ni en CREATE TABLE Pantalla, así que fallaba
-- si se corría más de una vez. El esquema (tablas, columnas, tipos, FKs)
-- es el mismo que envió Dennis, sin cambios.

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AccessControlDB_V2')
BEGIN
    CREATE DATABASE AccessControlDB_V2;
END
GO

USE AccessControlDB_V2;
GO

-- =========================
-- TABLA PANTALLA
-- =========================
IF OBJECT_ID('Pantalla', 'U') IS NOT NULL
    DROP TABLE Pantalla;
GO

CREATE TABLE Pantalla (
    Id NVARCHAR(50) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(300) NOT NULL,
    Ruta NVARCHAR(200) NOT NULL
);
GO

-- =========================
-- TABLA ROL
-- =========================
IF OBJECT_ID('Rol', 'U') IS NOT NULL
    DROP TABLE Rol;
GO

CREATE TABLE Rol (
    Id NVARCHAR(50) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL
);
GO

-- =========================
-- TABLA RELACION ROL-PANTALLA
-- =========================
IF OBJECT_ID('RolPantalla', 'U') IS NOT NULL
    DROP TABLE RolPantalla;
GO

CREATE TABLE RolPantalla (
    RolId NVARCHAR(50) NOT NULL,
    PantallaId NVARCHAR(50) NOT NULL,

    CONSTRAINT FK_RolPantalla_Rol
        FOREIGN KEY (RolId) REFERENCES Rol(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_RolPantalla_Pantalla
        FOREIGN KEY (PantallaId) REFERENCES Pantalla(Id)
        ON DELETE CASCADE
);
GO
