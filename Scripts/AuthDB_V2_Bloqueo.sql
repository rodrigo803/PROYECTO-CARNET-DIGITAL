-- =========================================
-- Bloqueo permanente tras 3 intentos fallidos (portado de AuthDB_V3)
-- Aditivo: no renombra ni elimina columnas existentes de UsersAuth
-- =========================================
USE AuthDB_V2;
GO

ALTER TABLE UsersAuth ADD IntentosFallidos INT NOT NULL DEFAULT 0;
GO

ALTER TABLE UsersAuth ADD Bloqueado BIT NOT NULL DEFAULT 0;
GO
