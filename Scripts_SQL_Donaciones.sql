-- =============================================
-- Scripts SQL para CRUD de Donaciones
-- Proyecto: PetsHome - Sistema de Gestión de Refugios
-- =============================================

-- Crear esquema de Refugio si no existe
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Refugio')
BEGIN
    EXEC('CREATE SCHEMA [Refugio]')
END
GO

-- =============================================
-- Tabla: tbDonaciones
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbDonaciones' AND xtype='U')
BEGIN
    CREATE TABLE [Refugio].[tbDonaciones] (
        [dona_Id] INT IDENTITY(1,1) NOT NULL,
        [dona_TipoDonacion] NVARCHAR(50) NOT NULL,
        [dona_NombreDonante] NVARCHAR(100) NOT NULL,
        [dona_TelefonoDonante] NVARCHAR(15) NULL,
        [dona_EmailDonante] NVARCHAR(100) NULL,
        [dona_MontoMonetario] DECIMAL(18,2) NULL,
        [dona_DescripcionArticulos] NVARCHAR(500) NULL,
        [dona_ValorEstimado] DECIMAL(18,2) NULL,
        [dona_FechaDonacion] DATE NOT NULL,
        [dona_Estado] NVARCHAR(30) NOT NULL,
        [dona_Observaciones] NVARCHAR(1000) NULL,
        [refg_Id] INT NOT NULL,
        [dona_EsEliminado] BIT NOT NULL DEFAULT 0,
        [dona_UsuarioCrea] INT NOT NULL,
        [dona_FechaCrea] DATETIME NOT NULL DEFAULT GETDATE(),
        [dona_UsuarioModifica] INT NULL,
        [dona_FechaModifica] DATETIME NULL,

        CONSTRAINT [PK_tbDonaciones] PRIMARY KEY CLUSTERED ([dona_Id] ASC),
        CONSTRAINT [FK_tbDonaciones_tbRefugios] FOREIGN KEY ([refg_Id]) REFERENCES [Refugio].[tbRefugios]([refg_Id]),
        CONSTRAINT [FK_tbDonaciones_tbUsuarios_Crea] FOREIGN KEY ([dona_UsuarioCrea]) REFERENCES [Seguridad].[tbUsuarios]([user_Id]),
        CONSTRAINT [FK_tbDonaciones_tbUsuarios_Modifica] FOREIGN KEY ([dona_UsuarioModifica]) REFERENCES [Seguridad].[tbUsuarios]([user_Id])
    )
END
GO

-- =============================================
-- Procedimiento: PR_Refugio_Donaciones_List
-- Descripción: Obtiene la lista de todas las donaciones
-- =============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Donaciones_List]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.dona_Id,
        d.dona_TipoDonacion,
        d.dona_NombreDonante,
        d.dona_TelefonoDonante,
        d.dona_EmailDonante,
        d.dona_MontoMonetario,
        d.dona_DescripcionArticulos,
        d.dona_ValorEstimado,
        d.dona_FechaDonacion,
        d.dona_Estado,
        d.dona_Observaciones,
        r.refg_Nombre,
        d.dona_FechaCrea,
        uc.user_Nombre AS dona_NombreUsuarioCrea
    FROM [Refugio].[tbDonaciones] d
    INNER JOIN [Refugio].[tbRefugios] r ON d.refg_Id = r.refg_Id
    INNER JOIN [Seguridad].[tbUsuarios] uc ON d.dona_UsuarioCrea = uc.user_Id
    WHERE d.dona_EsEliminado = 0
    ORDER BY d.dona_FechaDonacion DESC, d.dona_FechaCrea DESC;
END
GO

-- =============================================
-- Procedimiento: PR_Refugio_Donaciones_Find
-- Descripción: Busca una donación por ID para edición
-- =============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Donaciones_Find]
    @dona_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.dona_Id,
        d.dona_TipoDonacion,
        d.dona_NombreDonante,
        d.dona_TelefonoDonante,
        d.dona_EmailDonante,
        d.dona_MontoMonetario,
        d.dona_DescripcionArticulos,
        d.dona_ValorEstimado,
        d.dona_FechaDonacion,
        d.dona_Estado,
        d.dona_Observaciones,
        d.refg_Id,
        d.dona_UsuarioCrea,
        d.dona_FechaCrea,
        d.dona_UsuarioModifica,
        d.dona_FechaModifica
    FROM [Refugio].[tbDonaciones] d
    WHERE d.dona_Id = @dona_Id 
      AND d.dona_EsEliminado = 0;
END
GO

-- =============================================
-- Procedimiento: PR_Refugio_Donaciones_Detail
-- Descripción: Obtiene el detalle completo de una donación
-- =============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Donaciones_Detail]
    @dona_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.dona_Id,
        d.dona_TipoDonacion,
        d.dona_NombreDonante,
        d.dona_TelefonoDonante,
        d.dona_EmailDonante,
        d.dona_MontoMonetario,
        d.dona_DescripcionArticulos,
        d.dona_ValorEstimado,
        d.dona_FechaDonacion,
        d.dona_Estado,
        d.dona_Observaciones,
        r.refg_Nombre,
        r.refg_Ubicacion,
        r.refg_Telefono,
        d.dona_FechaCrea,
        uc.user_Nombre AS dona_NombreUsuarioCrea,
        d.dona_FechaModifica,
        um.user_Nombre AS dona_NombreUsuarioModifica
    FROM [Refugio].[tbDonaciones] d
    INNER JOIN [Refugio].[tbRefugios] r ON d.refg_Id = r.refg_Id
    INNER JOIN [Seguridad].[tbUsuarios] uc ON d.dona_UsuarioCrea = uc.user_Id
    LEFT JOIN [Seguridad].[tbUsuarios] um ON d.dona_UsuarioModifica = um.user_Id
    WHERE d.dona_Id = @dona_Id 
      AND d.dona_EsEliminado = 0;
END
GO

-- =============================================
-- Procedimiento: PR_Refugio_Donaciones_Insert
-- Descripción: Inserta una nueva donación
-- =============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Donaciones_Insert]
    @dona_TipoDonacion NVARCHAR(50),
    @dona_NombreDonante NVARCHAR(100),
    @dona_TelefonoDonante NVARCHAR(15) = NULL,
    @dona_EmailDonante NVARCHAR(100) = NULL,
    @dona_MontoMonetario DECIMAL(18,2) = NULL,
    @dona_DescripcionArticulos NVARCHAR(500) = NULL,
    @dona_ValorEstimado DECIMAL(18,2) = NULL,
    @dona_FechaDonacion DATE,
    @dona_Estado NVARCHAR(30),
    @dona_Observaciones NVARCHAR(1000) = NULL,
    @refg_Id INT,
    @dona_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que el refugio existe
        IF NOT EXISTS (SELECT 1 FROM [Refugio].[tbRefugios] WHERE refg_Id = @refg_Id AND refg_EsEliminado = 0)
        BEGIN
            RAISERROR('El refugio especificado no existe o está eliminado.', 16, 1);
            RETURN;
        END
        
        -- Validar que el usuario existe
        IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE user_Id = @dona_UsuarioCrea)
        BEGIN
            RAISERROR('El usuario especificado no existe.', 16, 1);
            RETURN;
        END
        
        -- Insertar la donación
        INSERT INTO [Refugio].[tbDonaciones] (
            dona_TipoDonacion,
            dona_NombreDonante,
            dona_TelefonoDonante,
            dona_EmailDonante,
            dona_MontoMonetario,
            dona_DescripcionArticulos,
            dona_ValorEstimado,
            dona_FechaDonacion,
            dona_Estado,
            dona_Observaciones,
            refg_Id,
            dona_UsuarioCrea,
            dona_FechaCrea,
            dona_EsEliminado
        )
        VALUES (
            @dona_TipoDonacion,
            @dona_NombreDonante,
            @dona_TelefonoDonante,
            @dona_EmailDonante,
            @dona_MontoMonetario,
            @dona_DescripcionArticulos,
            @dona_ValorEstimado,
            @dona_FechaDonacion,
            @dona_Estado,
            @dona_Observaciones,
            @refg_Id,
            @dona_UsuarioCrea,
            GETDATE(),
            0
        );
        
        COMMIT TRANSACTION;
        
        SELECT 0 AS Resultado, 'Donación insertada correctamente.' AS Mensaje;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        SELECT 1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- =============================================
-- Procedimiento: PR_Refugio_Donaciones_Update
-- Descripción: Actualiza una donación existente
-- =============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Donaciones_Update]
    @dona_Id INT,
    @dona_TipoDonacion NVARCHAR(50),
    @dona_NombreDonante NVARCHAR(100),
    @dona_TelefonoDonante NVARCHAR(15) = NULL,
    @dona_EmailDonante NVARCHAR(100) = NULL,
    @dona_MontoMonetario DECIMAL(18,2) = NULL,
    @dona_DescripcionArticulos NVARCHAR(500) = NULL,
    @dona_ValorEstimado DECIMAL(18,2) = NULL,
    @dona_FechaDonacion DATE,
    @dona_Estado NVARCHAR(30),
    @dona_Observaciones NVARCHAR(1000) = NULL,
    @refg_Id INT,
    @dona_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que la donación existe
        IF NOT EXISTS (SELECT 1 FROM [Refugio].[tbDonaciones] WHERE dona_Id = @dona_Id AND dona_EsEliminado = 0)
        BEGIN
            RAISERROR('La donación especificada no existe o está eliminada.', 16, 1);
            RETURN;
        END
        
        -- Validar que el refugio existe
        IF NOT EXISTS (SELECT 1 FROM [Refugio].[tbRefugios] WHERE refg_Id = @refg_Id AND refg_EsEliminado = 0)
        BEGIN
            RAISERROR('El refugio especificado no existe o está eliminado.', 16, 1);
            RETURN;
        END
        
        -- Validar que el usuario existe
        IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE user_Id = @dona_UsuarioModifica)
        BEGIN
            RAISERROR('El usuario especificado no existe.', 16, 1);
            RETURN;
        END
        
        -- Actualizar la donación
        UPDATE [Refugio].[tbDonaciones]
        SET 
            dona_TipoDonacion = @dona_TipoDonacion,
            dona_NombreDonante = @dona_NombreDonante,
            dona_TelefonoDonante = @dona_TelefonoDonante,
            dona_EmailDonante = @dona_EmailDonante,
            dona_MontoMonetario = @dona_MontoMonetario,
            dona_DescripcionArticulos = @dona_DescripcionArticulos,
            dona_ValorEstimado = @dona_ValorEstimado,
            dona_FechaDonacion = @dona_FechaDonacion,
            dona_Estado = @dona_Estado,
            dona_Observaciones = @dona_Observaciones,
            refg_Id = @refg_Id,
            dona_UsuarioModifica = @dona_UsuarioModifica,
            dona_FechaModifica = GETDATE()
        WHERE dona_Id = @dona_Id;
        
        COMMIT TRANSACTION;
        
        SELECT 0 AS Resultado, 'Donación actualizada correctamente.' AS Mensaje;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        SELECT 1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- =============================================
-- Procedimiento: PR_Refugio_Donaciones_Delete
-- Descripción: Elimina lógicamente una donación
-- =============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Donaciones_Delete]
    @dona_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que la donación existe
        IF NOT EXISTS (SELECT 1 FROM [Refugio].[tbDonaciones] WHERE dona_Id = @dona_Id AND dona_EsEliminado = 0)
        BEGIN
            RAISERROR('La donación especificada no existe o ya está eliminada.', 16, 1);
            RETURN;
        END
        
        -- Eliminar lógicamente la donación
        UPDATE [Refugio].[tbDonaciones]
        SET 
            dona_EsEliminado = 1,
            dona_FechaModifica = GETDATE()
        WHERE dona_Id = @dona_Id;
        
        COMMIT TRANSACTION;
        
        SELECT 0 AS Resultado, 'Donación eliminada correctamente.' AS Mensaje;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        SELECT 1 AS Resultado, ERROR_MESSAGE() AS Mensaje;
    END CATCH
END
GO

-- =============================================
-- Datos de ejemplo para testing (opcional)
-- =============================================
/*
-- Insertar algunas donaciones de ejemplo
EXEC [Refugio].[PR_Refugio_Donaciones_Insert]
    @dona_TipoDonacion = 'Monetaria',
    @dona_NombreDonante = 'Juan Pérez',
    @dona_TelefonoDonante = '555-0123',
    @dona_EmailDonante = 'juan.perez@email.com',
    @dona_MontoMonetario = 500.00,
    @dona_FechaDonacion = '2024-01-15',
    @dona_Estado = 'Recibida',
    @dona_Observaciones = 'Donación para alimentación de mascotas',
    @refg_Id = 1,
    @dona_UsuarioCrea = 1;

EXEC [Refugio].[PR_Refugio_Donaciones_Insert]
    @dona_TipoDonacion = 'Artículos',
    @dona_NombreDonante = 'María González',
    @dona_TelefonoDonante = '555-0456',
    @dona_EmailDonante = 'maria.gonzalez@email.com',
    @dona_DescripcionArticulos = 'Alimento para perros (20 kg), juguetes varios, mantas',
    @dona_ValorEstimado = 150.00,
    @dona_FechaDonacion = '2024-01-20',
    @dona_Estado = 'Procesada',
    @refg_Id = 1,
    @dona_UsuarioCrea = 1;
*/

PRINT 'Scripts SQL para CRUD de Donaciones ejecutados correctamente.';
GO