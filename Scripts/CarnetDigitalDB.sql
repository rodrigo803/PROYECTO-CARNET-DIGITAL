CREATE DATABASE CarnetDigitalDB;
GO
USE CarnetDigitalDB;
GO

-- =========================================================================
-- SRV15: Parámetros del Sistema
-- =========================================================================
-- El identificador debe ser texto, máximo 10 caracteres y solo letras en mayúscula[cite: 107].
-- El valor debe ser de máximo 500 caracteres[cite: 107].
CREATE TABLE Parametro (
    Identificador VARCHAR(10) PRIMARY KEY CHECK (Identificador NOT LIKE '%[^A-Z]%'),
    Valor VARCHAR(500) NOT NULL
);

-- =========================================================================
-- SRV6: Tipos de Identificación
-- =========================================================================
-- Requiere identificador y nombre[cite: 83, 85].
CREATE TABLE TipoIdentificacion (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

-- =========================================================================
-- SRV5: Tipos de Usuario
-- =========================================================================
-- Requiere identificador y nombre (ej. funcionario, estudiante o administrador)[cite: 83].
CREATE TABLE TipoUsuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

-- =========================================================================
-- SRV2: Instituciones
-- =========================================================================
-- Requiere identificador, nombre, email y teléfono[cite: 74].
CREATE TABLE Institucion (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(200) NOT NULL,
    Email VARCHAR(200) NOT NULL,
    Telefono VARCHAR(20) NOT NULL
);

-- Una institución puede tener múltiples dominios (ej: cuc.ac.cr y cuc.cr)[cite: 74].
CREATE TABLE InstitucionDominio (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InstitucionId INT NOT NULL FOREIGN KEY REFERENCES Institucion(Id),
    Dominio VARCHAR(100) NOT NULL
);

-- =========================================================================
-- SRV3: Carreras
-- =========================================================================
-- Requiere identificador, nombre, director (email y teléfono) e institución[cite: 77].
CREATE TABLE Carrera (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(200) NOT NULL,
    DirectorEmail VARCHAR(200) NOT NULL,
    DirectorTelefono VARCHAR(20) NOT NULL,
    InstitucionId INT NOT NULL FOREIGN KEY REFERENCES Institucion(Id)
);

-- =========================================================================
-- SRV4: Áreas de Trabajo
-- =========================================================================
-- Requiere identificador, nombre e institución[cite: 79].
CREATE TABLE AreaTrabajo (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(200) NOT NULL,
    InstitucionId INT NOT NULL FOREIGN KEY REFERENCES Institucion(Id)
);

-- =========================================================================
-- SRV7: Pantallas
-- =========================================================================
-- Requiere identificador, nombre, descripción y ruta de acceso[cite: 85, 89].
CREATE TABLE Pantalla (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(250) NOT NULL,
    RutaAcceso VARCHAR(200) NOT NULL
);

-- =========================================================================
-- SRV8: Roles
-- =========================================================================
-- Requiere identificador y nombre[cite: 89].
CREATE TABLE Rol (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

-- Al crear o modificar un rol se deben indicar las pantallas a las que tiene acceso[cite: 89].
CREATE TABLE RolPantalla (
    RolId INT NOT NULL FOREIGN KEY REFERENCES Rol(Id),
    PantallaId INT NOT NULL FOREIGN KEY REFERENCES Pantalla(Id),
    PRIMARY KEY (RolId, PantallaId)
);

-- =========================================================================
-- SRV12: Estado de Usuarios
-- =========================================================================
-- Los estados básicos son activo e inactivo, pero debe permitirse agregar nuevos[cite: 100].
CREATE TABLE EstadoUsuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);

-- =========================================================================
-- SRV10 & SRV13: Usuarios (Tu asignación principal)
-- =========================================================================
-- El Email será la identificación única del usuario[cite: 95].
-- Debe almacenar la contraseña encriptada y la fotografía del usuario (que se maneja en Base 64)[cite: 95, 100, 103].
CREATE TABLE Usuario (
    Email VARCHAR(200) PRIMARY KEY, 
    TipoIdentificacionId INT NOT NULL FOREIGN KEY REFERENCES TipoIdentificacion(Id),
    Identificacion VARCHAR(100) NOT NULL,
    NombreCompleto VARCHAR(300) NOT NULL,
    ContrasenaEncriptada VARCHAR(500) NOT NULL,
    TipoUsuarioId INT NOT NULL FOREIGN KEY REFERENCES TipoUsuario(Id),
    RolId INT NOT NULL FOREIGN KEY REFERENCES Rol(Id),
    EstadoId INT NOT NULL FOREIGN KEY REFERENCES EstadoUsuario(Id),
    FotografiaBase64 VARCHAR(MAX) NULL 
);

-- =========================================================================
-- Tablas Intermedias de Usuario (Relaciones de muchos a muchos) [cite: 95]
-- =========================================================================

-- Institución(es) asociadas[cite: 95].
CREATE TABLE UsuarioInstitucion (
    UsuarioEmail VARCHAR(200) NOT NULL FOREIGN KEY REFERENCES Usuario(Email),
    InstitucionId INT NOT NULL FOREIGN KEY REFERENCES Institucion(Id),
    PRIMARY KEY (UsuarioEmail, InstitucionId)
);

-- Carreras asociadas (puede ser más de una, si es estudiante)[cite: 95].
CREATE TABLE UsuarioCarrera (
    UsuarioEmail VARCHAR(200) NOT NULL FOREIGN KEY REFERENCES Usuario(Email),
    CarreraId INT NOT NULL FOREIGN KEY REFERENCES Carrera(Id),
    PRIMARY KEY (UsuarioEmail, CarreraId)
);

-- Áreas asociadas (puede ser más de una, si es funcionario)[cite: 95].
CREATE TABLE UsuarioArea (
    UsuarioEmail VARCHAR(200) NOT NULL FOREIGN KEY REFERENCES Usuario(Email),
    AreaTrabajoId INT NOT NULL FOREIGN KEY REFERENCES AreaTrabajo(Id),
    PRIMARY KEY (UsuarioEmail, AreaTrabajoId)
);

-- Teléfono(s) de contacto, los cuales no son obligatorios[cite: 95].
CREATE TABLE UsuarioTelefono (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioEmail VARCHAR(200) NOT NULL FOREIGN KEY REFERENCES Usuario(Email),
    Telefono VARCHAR(20) NOT NULL
);

-- =========================================================================
-- SRV9: Bitácoras
-- =========================================================================
-- Requiere el usuario que ejecuta la acción, descripción de la acción y fecha[cite: 92].
CREATE TABLE Bitacora (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioEmail VARCHAR(200) NOT NULL FOREIGN KEY REFERENCES Usuario(Email),
    Descripcion VARCHAR(500) NOT NULL,
    FechaHora DATETIME NOT NULL DEFAULT GETDATE()
);
GO