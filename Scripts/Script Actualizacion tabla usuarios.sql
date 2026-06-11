USE [MicroservicioUsuariosDB];
GO

-- 1. Eliminar la tabla antigua si existe
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Usuarios];
END
GO

-- 2. Crear la nueva tabla con la Arquitectura Corregida
CREATE TABLE [dbo].[Usuarios] (
    -- La Identificación ahora es la primera columna y no permite nulos
    [Identificacion] NVARCHAR(50) NOT NULL,
    
    [Email] NVARCHAR(150) NOT NULL,
    [NombreCompleto] NVARCHAR(200) NOT NULL,
    [ContrasenaEncriptada] NVARCHAR(MAX) NOT NULL,
    [FotografiaBase64] NVARCHAR(MAX) NULL,
    [TokenConfirmacion] NVARCHAR(255) NULL,
    [FechaExpiracionToken] DATETIME2 NULL,
    [EstadoId] INT NOT NULL,
    [TipoIdentificacionId] INT NOT NULL,
    [TipoUsuarioId] INT NOT NULL,
    [RolId] INT NOT NULL,
    [TipoIdentificacion] NVARCHAR(100) NULL,
    [TipoUsuario] NVARCHAR(100) NULL,

    -- LA MAGIA OCURRE AQUÍ:
    -- 1. Asignamos la Cédula como la Llave Primaria (Primary Key)
    CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([Identificacion] ASC),

    -- 2. Obligamos a que el correo sea único en todo el sistema
    CONSTRAINT [UQ_Usuarios_Email] UNIQUE ([Email]),

    -- 3. Mantenemos tu relación intacta con la tabla EstadoUsuario
    CONSTRAINT [FK_Usuarios_EstadoUsuario] FOREIGN KEY ([EstadoId]) REFERENCES [dbo].[EstadoUsuario] ([Id])
);
GO