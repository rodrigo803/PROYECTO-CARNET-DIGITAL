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
CREATE TABLE [Instituciones] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(200) NOT NULL,
    [Email] nvarchar(200) NOT NULL,
    [Telefono] nvarchar(20) NOT NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Instituciones] PRIMARY KEY ([Id])
);

CREATE TABLE [InstitucionDominios] (
    [Id] int NOT NULL IDENTITY,
    [Dominio] nvarchar(100) NOT NULL,
    [Activo] bit NOT NULL,
    [IdInstitucion] int NOT NULL,
    CONSTRAINT [PK_InstitucionDominios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InstitucionDominios_Instituciones_IdInstitucion] FOREIGN KEY ([IdInstitucion]) REFERENCES [Instituciones] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_InstitucionDominios_IdInstitucion] ON [InstitucionDominios] ([IdInstitucion]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260610062945_InitialCreate', N'10.0.9');

COMMIT;
GO

