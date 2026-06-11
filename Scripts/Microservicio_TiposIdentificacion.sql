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
CREATE TABLE [TiposIdentificacion] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_TiposIdentificacion] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260610082555_InitialCreate', N'10.0.9');

COMMIT;
GO

