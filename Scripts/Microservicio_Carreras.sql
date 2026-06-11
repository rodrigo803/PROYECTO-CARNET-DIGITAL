IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Carreras] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(200) NOT NULL,
    [Director] nvarchar(200) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [Telefono] nvarchar(20) NOT NULL,
    [Activo] bit NOT NULL,
    [IdInstitucion] int NOT NULL,
    CONSTRAINT [PK_Carreras] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260610071943_InitialCreate', N'10.0.9');

COMMIT;
GO

