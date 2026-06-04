-- =========================================
-- 1. CREAR BASE DE DATOS AuthDB_V2
-- =========================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AuthDB_V2')
BEGIN
    CREATE DATABASE AuthDB_V2;
END
GO

USE AuthDB_V2;
GO

-- =========================================
-- 2. TABLA: UsersAuth
-- =========================================
IF OBJECT_ID('UsersAuth', 'U') IS NOT NULL
    DROP TABLE UsersAuth;
GO

CREATE TABLE UsersAuth (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    UserType NVARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =========================================
-- 3. TABLA: RefreshTokens
-- =========================================
IF OBJECT_ID('RefreshTokens', 'U') IS NOT NULL
    DROP TABLE RefreshTokens;
GO

CREATE TABLE RefreshTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    Expiration DATETIME NOT NULL,
    IsRevoked BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_RefreshTokens_User
        FOREIGN KEY (UserId) REFERENCES UsersAuth(Id)
        ON DELETE CASCADE
);
GO

-- =========================================
-- 4. ÍNDICES
-- =========================================
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_UsersAuth_Email')
    CREATE INDEX IX_UsersAuth_Email ON UsersAuth(Email);
GO

IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_RefreshTokens_Token')
    CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
GO

USE AuthDB_V2;
GO

-- ==============================================================
-- INSERTS DE PRUEBA (Contraseña para ambos: 123456)
-- Hash BCrypt real para '123456': $2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxep68pdd6k82Zk1q
-- ==============================================================

INSERT INTO UsersAuth (Email, PasswordHash, UserType, IsActive, CreatedAt)
VALUES 
(
    'rodrigo_actualizado@correo.com', 
    '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxep68pdd6k82Zk1q', 
    'Admin', 
    1, 
    GETDATE()
);

INSERT INTO UsersAuth (Email, PasswordHash, UserType, IsActive, CreatedAt)
VALUES 
(
    'estudiante@cuc.cr', 
    '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxep68pdd6k82Zk1q', 
    'Estudiante', 
    1, 
    GETDATE()
);

-- Revisamos que se hayan insertado correctamente
SELECT * FROM UsersAuth;
GO