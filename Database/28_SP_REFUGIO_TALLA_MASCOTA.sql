/*
=============================================
SCRIPT: Tabla tbTallas + SP Dropdown + FK en tbMascotas
AUTOR: Sistema PetsHome
FECHA: 2026-05-11
=============================================
*/

USE PETSHOMEDB
GO

-- =============================================
-- 1. Crear tabla tbTallas
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbTallas' AND schema_id = SCHEMA_ID('Refugio'))
BEGIN
    CREATE TABLE [Refugio].[tbTallas] (
        tall_Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        tall_Descripcion VARCHAR(50)       NOT NULL,
        tall_EsEliminado BIT               NOT NULL DEFAULT 0,
        tall_UsuarioCrea    INT            NOT NULL,
        tall_FechaCrea      DATETIME       NOT NULL DEFAULT GETDATE(),
        tall_UsuarioModifica INT           NULL,
        tall_FechaModifica   DATETIME      NULL
    )
    PRINT 'Tabla tbTallas creada.'
END
GO

-- =============================================
-- 2. Datos semilla
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [Refugio].[tbTallas])
BEGIN
    INSERT INTO [Refugio].[tbTallas] (tall_Descripcion, tall_EsEliminado, tall_UsuarioCrea, tall_FechaCrea)
    VALUES
        (N'Pequeño',      0, 1, GETDATE()),
        (N'Mediano',      0, 1, GETDATE()),
        (N'Grande',       0, 1, GETDATE()),
        (N'Extra Grande', 0, 1, GETDATE())
    PRINT 'Datos semilla de tbTallas insertados.'
END
GO

-- =============================================
-- 3. Agregar FK tall_Id a tbMascotas
-- =============================================
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('[Refugio].[tbMascotas]') AND name = 'tall_Id'
)
BEGIN
    ALTER TABLE [Refugio].[tbMascotas]
        ADD tall_Id INT NULL

    ALTER TABLE [Refugio].[tbMascotas]
        ADD CONSTRAINT FK_tbMascotas_tbTallas
        FOREIGN KEY (tall_Id) REFERENCES [Refugio].[tbTallas](tall_Id)

    PRINT 'Columna tall_Id agregada a tbMascotas.'
END
GO

-- =============================================
-- 4. SP: PR_Refugio_Talla_Dropdown
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Refugio_Talla_Dropdown')
    DROP PROCEDURE [Refugio].[PR_Refugio_Talla_Dropdown]
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Talla_Dropdown]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        tall_Id,
        tall_Descripcion
    FROM [Refugio].[tbTallas]
    WHERE tall_EsEliminado = 0
    ORDER BY tall_Id
END
GO

-- =============================================
-- 5. Actualizar PR_Refugio_Mascotas_Insert
--    (agregar @tall_Id)
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Refugio_Mascotas_Insert')
    DROP PROCEDURE [Refugio].[PR_Refugio_Mascotas_Insert]
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Mascotas_Insert]
    @masc_Imagen        VARBINARY(MAX) = NULL,
    @masc_Nombre        VARCHAR(50),
    @raza_Id            INT            = NULL,
    @masc_Edad          INT            = NULL,
    @masc_Sexo          VARCHAR(10)    = NULL,
    @masc_Peso          DECIMAL(18,2),
    @tall_Id            INT            = NULL,
    @masc_Color         VARCHAR(50)    = NULL,
    @masc_Historia      VARCHAR(500)   = NULL,
    @refg_Id            INT,
    @proc_Id            INT,
    @masc_UsuarioCrea   INT
AS
BEGIN
    SET NOCOUNT ON
    INSERT INTO [Refugio].[tbMascotas]
        (masc_Imagen, masc_Nombre, raza_Id, masc_Edad, masc_Sexo, masc_Peso, tall_Id,
         masc_Color, masc_Historia, refg_Id, proc_Id,
         masc_EsAdoptado, masc_EsReservado, masc_EsEliminado,
         masc_UsuarioCrea, masc_FechaCrea)
    VALUES
        (@masc_Imagen, @masc_Nombre, @raza_Id, @masc_Edad, @masc_Sexo, @masc_Peso, @tall_Id,
         @masc_Color, @masc_Historia, @refg_Id, @proc_Id,
         0, 0, 0,
         @masc_UsuarioCrea, GETDATE())

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS CodeStatus, '' AS MessageStatus
END
GO

-- =============================================
-- 6. Actualizar PR_Refugio_Mascotas_Update
--    (agregar @tall_Id)
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Refugio_Mascotas_Update')
    DROP PROCEDURE [Refugio].[PR_Refugio_Mascotas_Update]
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Mascotas_Update]
    @masc_Id            INT,
    @masc_Imagen        VARBINARY(MAX) = NULL,
    @masc_Nombre        VARCHAR(50),
    @raza_Id            INT            = NULL,
    @masc_Edad          INT            = NULL,
    @masc_Sexo          VARCHAR(10)    = NULL,
    @masc_Peso          DECIMAL(18,2),
    @tall_Id            INT            = NULL,
    @masc_Color         VARCHAR(50)    = NULL,
    @masc_Historia      VARCHAR(500)   = NULL,
    @refg_Id            INT,
    @proc_Id            INT,
    @masc_EsAdoptado    BIT,
    @masc_EsReservado   BIT,
    @masc_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    UPDATE [Refugio].[tbMascotas]
    SET
        masc_Imagen          = ISNULL(@masc_Imagen, masc_Imagen),
        masc_Nombre          = @masc_Nombre,
        raza_Id              = @raza_Id,
        masc_Edad            = @masc_Edad,
        masc_Sexo            = @masc_Sexo,
        masc_Peso            = @masc_Peso,
        tall_Id              = @tall_Id,
        masc_Color           = @masc_Color,
        masc_Historia        = @masc_Historia,
        refg_Id              = @refg_Id,
        proc_Id              = @proc_Id,
        masc_EsAdoptado      = @masc_EsAdoptado,
        masc_EsReservado     = @masc_EsReservado,
        masc_UsuarioModifica = @masc_UsuarioModifica,
        masc_FechaModifica   = GETDATE()
    WHERE masc_Id = @masc_Id

    SELECT 1 AS CodeStatus, '' AS MessageStatus
END
GO

-- =============================================
-- 7. Actualizar PR_Refugio_Mascotas_Find
--    (reemplaza masc_Talla por tall_Id + tall_Descripcion)
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Refugio_Mascotas_Find')
    DROP PROCEDURE [Refugio].[PR_Refugio_Mascotas_Find]
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Mascotas_Find]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        m.masc_Id,
        m.masc_Imagen,
        m.masc_Nombre,
        m.raza_Id,
        r.raza_Descripcion,
        m.masc_Edad,
        m.masc_Sexo,
        m.masc_Peso,
        m.tall_Id,
        t.tall_Descripcion,
        m.masc_Color,
        m.masc_Historia,
        m.refg_Id,
        rf.refg_Nombre,
        m.proc_Id,
        p.proc_Descripcion,
        m.masc_EsAdoptado,
        m.masc_EsReservado,
        m.masc_UsuarioCrea,
        uc.usu_Nombre AS usuarioCrea,
        m.masc_FechaCrea,
        m.masc_UsuarioModifica,
        um.usu_Nombre AS usuarioModifica,
        m.masc_FechaModifica
    FROM [Refugio].[tbMascotas] m
    LEFT JOIN [Refugio].[tbRazas]       r  ON r.raza_Id  = m.raza_Id
    LEFT JOIN [Refugio].[tbTallas]      t  ON t.tall_Id  = m.tall_Id
    LEFT JOIN [Refugio].[tbRefugios]    rf ON rf.refg_Id = m.refg_Id
    LEFT JOIN [Refugio].[tbProcedencias] p ON p.proc_Id  = m.proc_Id
    LEFT JOIN [Seguridad].[tbUsuarios]  uc ON uc.usu_Id  = m.masc_UsuarioCrea
    LEFT JOIN [Seguridad].[tbUsuarios]  um ON um.usu_Id  = m.masc_UsuarioModifica
    WHERE m.masc_Id = @masc_Id
      AND m.masc_EsEliminado = 0
END
GO
