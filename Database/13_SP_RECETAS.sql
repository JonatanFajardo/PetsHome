/*
=============================================
SCRIPT: Stored Procedures para tbRecetas
AUTOR: Sistema PetsHome
FECHA: 2025-11-04
=============================================
*/

USE PETSHOMEDB
GO

-- =======================================
-- SP: PR_Medico_Recetas_List
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Recetas_List')
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_List]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Recetas_List]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ROW_NUMBER() OVER(ORDER BY rec.receta_FechaInicio DESC) AS Fila,
        rec.receta_Id,
        rec.cita_Id,
        rec.masc_Id,
        masc.masc_Nombre AS Mascota,
        rec.receta_Medicamento,
        rec.tipoMed_Id,
        tipoMed.tipoMed_Descripcion AS TipoMedicamento,
        rec.viaAdmin_Id,
        viaAdmin.viaAdmin_Descripcion AS ViaAdministracion,
        rec.receta_Dosis,
        rec.receta_Frecuencia,
        rec.receta_Duracion,
        rec.receta_FechaInicio,
        rec.receta_FechaFin,
        rec.receta_Estado
    FROM [Medico].[tbRecetas] AS rec
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON rec.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposMedicamento] AS tipoMed
        ON rec.tipoMed_Id = tipoMed.tipoMed_Id
    LEFT JOIN [Medico].[tbViasAdministracion] AS viaAdmin
        ON rec.viaAdmin_Id = viaAdmin.viaAdmin_Id
    WHERE rec.receta_EsEliminado = 0
    ORDER BY rec.receta_FechaInicio DESC
END
GO

-- =======================================
-- SP: PR_Medico_Recetas_Detail
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Recetas_Detail')
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Detail]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Detail]
    @receta_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        rec.receta_Id,
        rec.cita_Id,
        rec.masc_Id,
        masc.masc_Nombre AS Mascota,
        rec.receta_Medicamento,
        rec.tipoMed_Id,
        tipoMed.tipoMed_Descripcion AS TipoMedicamento,
        rec.viaAdmin_Id,
        viaAdmin.viaAdmin_Descripcion AS ViaAdministracion,
        rec.receta_Dosis,
        rec.receta_Frecuencia,
        rec.receta_Duracion,
        rec.receta_Instrucciones,
        rec.receta_FechaInicio,
        rec.receta_FechaFin,
        rec.receta_Estado,
        usuarioCrea.usu_Nombre AS UsuarioCreacion,
        rec.receta_FechaCrea,
        usuarioModifica.usu_Nombre AS UsuarioModificacion,
        rec.receta_FechaModifica
    FROM [Medico].[tbRecetas] AS rec
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON rec.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposMedicamento] AS tipoMed
        ON rec.tipoMed_Id = tipoMed.tipoMed_Id
    LEFT JOIN [Medico].[tbViasAdministracion] AS viaAdmin
        ON rec.viaAdmin_Id = viaAdmin.viaAdmin_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON rec.receta_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON rec.receta_UsuarioModifica = usuarioModifica.usu_Id
    WHERE rec.receta_Id = @receta_Id
    AND rec.receta_EsEliminado = 0
END
GO

-- =======================================
-- SP: PR_Medico_Recetas_Find
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Recetas_Find')
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Find]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Find]
    @receta_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        rec.receta_Id,
        rec.cita_Id,
        rec.masc_Id,
        rec.receta_Medicamento,
        rec.tipoMed_Id,
        rec.viaAdmin_Id,
        rec.receta_Dosis,
        rec.receta_Frecuencia,
        rec.receta_Duracion,
        rec.receta_Instrucciones,
        rec.receta_FechaInicio,
        rec.receta_FechaFin,
        rec.receta_Estado,
        rec.receta_UsuarioCrea,
        usuarioCrea.usu_Nombre AS usuarioCrea,
        rec.receta_FechaCrea,
        rec.receta_UsuarioModifica,
        usuarioModifica.usu_Nombre AS usuarioModifica,
        rec.receta_FechaModifica
    FROM [Medico].[tbRecetas] AS rec
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON rec.receta_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON rec.receta_UsuarioModifica = usuarioModifica.usu_Id
    WHERE rec.receta_Id = @receta_Id
    AND rec.receta_EsEliminado = 0
END
GO

-- =======================================
-- SP: PR_Medico_Recetas_Insert
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Recetas_Insert')
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Insert]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Insert]
    @cita_Id INT,
    @masc_Id INT,
    @receta_Medicamento NVARCHAR(200),
    @tipoMed_Id INT = NULL,
    @viaAdmin_Id INT = NULL,
    @receta_Dosis NVARCHAR(100) = NULL,
    @receta_Frecuencia NVARCHAR(100) = NULL,
    @receta_Duracion NVARCHAR(50) = NULL,
    @receta_Instrucciones NVARCHAR(500) = NULL,
    @receta_FechaInicio DATE = NULL,
    @receta_FechaFin DATE = NULL,
    @receta_Estado NVARCHAR(20) = 'Activo',
    @receta_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [Medico].[tbRecetas]
    (
        cita_Id,
        masc_Id,
        receta_Medicamento,
        tipoMed_Id,
        viaAdmin_Id,
        receta_Dosis,
        receta_Frecuencia,
        receta_Duracion,
        receta_Instrucciones,
        receta_FechaInicio,
        receta_FechaFin,
        receta_Estado,
        receta_UsuarioCrea,
        receta_FechaCrea
    )
    VALUES
    (
        @cita_Id,
        @masc_Id,
        @receta_Medicamento,
        @tipoMed_Id,
        @viaAdmin_Id,
        @receta_Dosis,
        @receta_Frecuencia,
        @receta_Duracion,
        @receta_Instrucciones,
        @receta_FechaInicio,
        @receta_FechaFin,
        @receta_Estado,
        @receta_UsuarioCrea,
        GETDATE()
    )

    SELECT SCOPE_IDENTITY() AS receta_Id
END
GO

-- =======================================
-- SP: PR_Medico_Recetas_Update
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Recetas_Update')
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Update]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Update]
    @receta_Id INT,
    @cita_Id INT,
    @masc_Id INT,
    @receta_Medicamento NVARCHAR(200),
    @tipoMed_Id INT = NULL,
    @viaAdmin_Id INT = NULL,
    @receta_Dosis NVARCHAR(100) = NULL,
    @receta_Frecuencia NVARCHAR(100) = NULL,
    @receta_Duracion NVARCHAR(50) = NULL,
    @receta_Instrucciones NVARCHAR(500) = NULL,
    @receta_FechaInicio DATE = NULL,
    @receta_FechaFin DATE = NULL,
    @receta_Estado NVARCHAR(20) = 'Activo',
    @receta_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON

    UPDATE [Medico].[tbRecetas]
    SET
        cita_Id = @cita_Id,
        masc_Id = @masc_Id,
        receta_Medicamento = @receta_Medicamento,
        tipoMed_Id = @tipoMed_Id,
        viaAdmin_Id = @viaAdmin_Id,
        receta_Dosis = @receta_Dosis,
        receta_Frecuencia = @receta_Frecuencia,
        receta_Duracion = @receta_Duracion,
        receta_Instrucciones = @receta_Instrucciones,
        receta_FechaInicio = @receta_FechaInicio,
        receta_FechaFin = @receta_FechaFin,
        receta_Estado = @receta_Estado,
        receta_UsuarioModifica = @receta_UsuarioModifica,
        receta_FechaModifica = GETDATE()
    WHERE receta_Id = @receta_Id
END
GO

-- =======================================
-- SP: PR_Medico_Recetas_Delete
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Recetas_Delete')
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Delete]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Delete]
    @receta_Id INT,
    @receta_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON

    UPDATE [Medico].[tbRecetas]
    SET
        receta_EsEliminado = 1,
        receta_UsuarioModifica = @receta_UsuarioModifica,
        receta_FechaModifica = GETDATE()
    WHERE receta_Id = @receta_Id
END
GO

-- =======================================
-- SP: PR_Medico_Recetas_Dropdown
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Recetas_Dropdown')
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Dropdown]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Dropdown]
    @masc_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        receta_Id,
        CONCAT(receta_Medicamento, ' - ', receta_Estado) AS Descripcion
    FROM [Medico].[tbRecetas]
    WHERE receta_EsEliminado = 0
    AND (@masc_Id IS NULL OR masc_Id = @masc_Id)
    ORDER BY receta_FechaInicio DESC
END
GO

PRINT '=========================================='
PRINT '✅ STORED PROCEDURES RECETAS CREADOS'
PRINT '=========================================='
PRINT 'Total: 7 procedimientos'
PRINT '=========================================='
GO
