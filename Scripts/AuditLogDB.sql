-- =========================================
-- 1. CREAR BASE DE DATOS
-- =========================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AuditLogDB')
BEGIN
    CREATE DATABASE AuditLogDB;
END
GO

USE AuditLogDB;
GO

-- =========================================
-- 2. TABLA: Bitacora
-- =========================================
IF OBJECT_ID('Bitacora', 'U') IS NOT NULL
    DROP TABLE Bitacora;
GO

CREATE TABLE Bitacora (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =========================================
-- 3. ÍNDICES (recomendado)
-- =========================================
CREATE INDEX IX_Bitacora_UsuarioId ON Bitacora(UsuarioId);
GO

CREATE INDEX IX_Bitacora_Fecha ON Bitacora(Fecha);
GO

-- =========================================
-- FIN
-- =========================================

SELECT * FROM Bitacora;