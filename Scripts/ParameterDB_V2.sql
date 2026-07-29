-- =========================================
-- 1. CREAR BASE DE DATOS ParameterDB_V2
-- =========================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ParameterDB_V2')
BEGIN
    CREATE DATABASE ParameterDB_V2;
END
GO

USE ParameterDB_V2;
GO

-- =========================================
-- 2. TABLA: Parametro
-- =========================================
IF OBJECT_ID('Parametro', 'U') IS NOT NULL
    DROP TABLE Parametro;
GO

CREATE TABLE Parametro (
    Id    NVARCHAR(10)  NOT NULL PRIMARY KEY,
    Valor NVARCHAR(500) NOT NULL
);
GO

ALTER TABLE Parametro
    ADD CONSTRAINT CK_Parametro_Id_NotEmpty CHECK (LEN(LTRIM(RTRIM(Id))) > 0);
GO

ALTER TABLE Parametro
    ADD CONSTRAINT CK_Parametro_Id_Uppercase CHECK (NOT Id LIKE '%[^A-Z]%');
GO

ALTER TABLE Parametro
    ADD CONSTRAINT CK_Parametro_Valor_NotEmpty CHECK (LEN(LTRIM(RTRIM(Valor))) > 0);
GO
