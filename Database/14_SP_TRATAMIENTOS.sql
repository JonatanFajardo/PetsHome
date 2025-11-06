/*
=============================================
SCRIPT: Stored Procedures para tbTratamientos
AUTOR: Sistema PetsHome
FECHA: 2025-11-04
=============================================
*/

USE PETSHOMEDB
GO

-- =======================================
-- SP: PR_Medico_Tratamientos_List
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Tratamientos_List')
    DROP PROCEDURE [Medico].[PR_Medico_Tratamientos_List]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Tratamientos_List]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ROW_NUMBER() OVER(ORDER BY trat.trat_FechaAplicacion DESC) AS Fila,
        trat.trat_Id,
        trat.masc_Id,
        masc.masc_Nombre AS Mascota,
        trat.tipoPar_Id,
        tipoPar.tipoPar_Descripcion AS TipoParasito,
        tipoPar.tipoPar_Categoria AS CategoriaParasito,
        trat.trat_ParasitoDetectado,
        trat.trat_Medicamento,
        trat.tipoMed_Id,
        tipoMed.tipoMed_Descripcion AS TipoMedicamento,
        trat.viaAdmin_Id,
        viaAdmin.viaAdmin_Descripcion AS ViaAdministracion,
        trat.trat_FechaAplicacion,
        trat.trat_ProximaDosis,
        trat.trat_Estado
    FROM [Medico].[tbTratamientos] AS trat
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON trat.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposParasito] AS tipoPar
        ON trat.tipoPar_Id = tipoPar.tipoPar_Id
    LEFT JOIN [Medico].[tbTiposMedicamento] AS tipoMed
        ON trat.tipoMed_Id = tipoMed.tipoMed_Id
    LEFT JOIN [Medico].[tbViasAdministracion] AS viaAdmin
        ON trat.viaAdmin_Id = viaAdmin.viaAdmin_Id
    WHERE trat.trat_EsEliminado = 0
    ORDER BY trat.trat_FechaAplicacion DESC
END
GO

-- =======================================
-- SP: PR_Medico_Tratamientos_Detail
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Tratamientos_Detail')
    DROP PROCEDURE [Medico].[PR_Medico_Tratamientos_Detail]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Tratamientos_Detail]
    @trat_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        trat.trat_Id,
        trat.masc_Id,
        masc.masc_Nombre AS Mascota,
        trat.cita_Id,
        trat.receta_Id,
        trat.tipoPar_Id,
        tipoPar.tipoPar_Descripcion AS TipoParasito,
        tipoPar.tipoPar_Categoria AS CategoriaParasito,
        trat.trat_ParasitoDetectado,
        trat.trat_Medicamento,
        trat.tipoMed_Id,
        tipoMed.tipoMed_Descripcion AS TipoMedicamento,
        trat.viaAdmin_Id,
        viaAdmin.viaAdmin_Descripcion AS ViaAdministracion,
        trat.trat_FechaAplicacion,
        trat.trat_AplicadoPor,
        trat.trat_ProximaDosis,
        trat.trat_Estado,
        trat.trat_Observaciones,
        usuarioCrea.usu_Nombre AS UsuarioCreacion,
        trat.trat_FechaCrea,
        usuarioModifica.usu_Nombre AS UsuarioModificacion,
        trat.trat_FechaModifica
    FROM [Medico].[tbTratamientos] AS trat
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON trat.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposParasito] AS tipoPar
        ON trat.tipoPar_Id = tipoPar.tipoPar_Id
    LEFT JOIN [Medico].[tbTiposMedicamento] AS tipoMed
        ON trat.tipoMed_Id = tipoMed.tipoMed_Id
    LEFT JOIN [Medico].[tbViasAdministracion] AS viaAdmin
        ON trat.viaAdmin_Id = viaAdmin.viaAdmin_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON trat.trat_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON trat.trat_UsuarioModifica = usuarioModifica.usu_Id
    WHERE trat.trat_Id = @trat_Id
    AND trat.trat_EsEliminado = 0
END
GO

-- =======================================
-- SP: PR_Medico_Tratamientos_Find
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Tratamientos_Find')
    DROP PROCEDURE [Medico].[PR_Medico_Tratamientos_Find]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Tratamientos_Find]
    @trat_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        trat.trat_Id,
        trat.masc_Id,
        trat.cita_Id,
        trat.receta_Id,
        trat.tipoPar_Id,
        trat.trat_ParasitoDetectado,
        trat.trat_Medicamento,
        trat.tipoMed_Id,
        trat.viaAdmin_Id,
        trat.trat_FechaAplicacion,
        trat.trat_AplicadoPor,
        trat.trat_ProximaDosis,
        trat.trat_Estado,
        trat.trat_Observaciones,
        trat.trat_UsuarioCrea,
        usuarioCrea.usu_Nombre AS usuarioCrea,
        trat.trat_FechaCrea,
        trat.trat_UsuarioModifica,
        usuarioModifica.usu_Nombre AS usuarioModifica,
        trat.trat_FechaModifica
    FROM [Medico].[tbTratamientos] AS trat
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON trat.trat_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON trat.trat_UsuarioModifica = usuarioModifica.usu_Id
    WHERE trat.trat_Id = @trat_Id
    AND trat.trat_EsEliminado = 0
END
GO

-- =======================================
-- SP: PR_Medico_Tratamientos_Insert
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Tratamientos_Insert')
    DROP PROCEDURE [Medico].[PR_Medico_Tratamientos_Insert]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Tratamientos_Insert]
    @masc_Id INT,
    @cita_Id INT = NULL,
    @receta_Id INT = NULL,
    @tipoPar_Id INT = NULL,
    @trat_ParasitoDetectado NVARCHAR(200) = NULL,
    @trat_Medicamento NVARCHAR(200) = NULL,
    @tipoMed_Id INT = NULL,
    @viaAdmin_Id INT = NULL,
    @trat_FechaAplicacion DATETIME,
    @trat_AplicadoPor NVARCHAR(100) = NULL,
    @trat_ProximaDosis DATE = NULL,
    @trat_Estado NVARCHAR(20) = 'Iniciado',
    @trat_Observaciones NVARCHAR(500) = NULL,
    @trat_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [Medico].[tbTratamientos]
    (
        masc_Id,
        cita_Id,
        receta_Id,
        tipoPar_Id,
        trat_ParasitoDetectado,
        trat_Medicamento,
        tipoMed_Id,
        viaAdmin_Id,
        trat_FechaAplicacion,
        trat_AplicadoPor,
        trat_ProximaDosis,
        trat_Estado,
        trat_Observaciones,
        trat_UsuarioCrea,
        trat_FechaCrea
    )
    VALUES
    (
        @masc_Id,
        @cita_Id,
        @receta_Id,
        @tipoPar_Id,
        @trat_ParasitoDetectado,
        @trat_Medicamento,
        @tipoMed_Id,
        @viaAdmin_Id,
        @trat_FechaAplicacion,
        @trat_AplicadoPor,
        @trat_ProximaDosis,
        @trat_Estado,
        @trat_Observaciones,
        @trat_UsuarioCrea,
        GETDATE()
    )

    SELECT SCOPE_IDENTITY() AS trat_Id
END
GO

-- =======================================
-- SP: PR_Medico_Tratamientos_Update
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Tratamientos_Update')
    DROP PROCEDURE [Medico].[PR_Medico_Tratamientos_Update]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Tratamientos_Update]
    @trat_Id INT,
    @masc_Id INT,
    @cita_Id INT = NULL,
    @receta_Id INT = NULL,
    @tipoPar_Id INT = NULL,
    @trat_ParasitoDetectado NVARCHAR(200) = NULL,
    @trat_Medicamento NVARCHAR(200) = NULL,
    @tipoMed_Id INT = NULL,
    @viaAdmin_Id INT = NULL,
    @trat_FechaAplicacion DATETIME,
    @trat_AplicadoPor NVARCHAR(100) = NULL,
    @trat_ProximaDosis DATE = NULL,
    @trat_Estado NVARCHAR(20) = 'Iniciado',
    @trat_Observaciones NVARCHAR(500) = NULL,
    @trat_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON

    UPDATE [Medico].[tbTratamientos]
    SET
        masc_Id = @masc_Id,
        cita_Id = @cita_Id,
        receta_Id = @receta_Id,
        tipoPar_Id = @tipoPar_Id,
        trat_ParasitoDetectado = @trat_ParasitoDetectado,
        trat_Medicamento = @trat_Medicamento,
        tipoMed_Id = @tipoMed_Id,
        viaAdmin_Id = @viaAdmin_Id,
        trat_FechaAplicacion = @trat_FechaAplicacion,
        trat_AplicadoPor = @trat_AplicadoPor,
        trat_ProximaDosis = @trat_ProximaDosis,
        trat_Estado = @trat_Estado,
        trat_Observaciones = @trat_Observaciones,
        trat_UsuarioModifica = @trat_UsuarioModifica,
        trat_FechaModifica = GETDATE()
    WHERE trat_Id = @trat_Id
END
GO

-- =======================================
-- SP: PR_Medico_Tratamientos_Delete
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Tratamientos_Delete')
    DROP PROCEDURE [Medico].[PR_Medico_Tratamientos_Delete]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Tratamientos_Delete]
    @trat_Id INT,
    @trat_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON

    UPDATE [Medico].[tbTratamientos]
    SET
        trat_EsEliminado = 1,
        trat_UsuarioModifica = @trat_UsuarioModifica,
        trat_FechaModifica = GETDATE()
    WHERE trat_Id = @trat_Id
END
GO

-- =======================================
-- SP: PR_Medico_Tratamientos_Dropdown
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_Tratamientos_Dropdown')
    DROP PROCEDURE [Medico].[PR_Medico_Tratamientos_Dropdown]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Tratamientos_Dropdown]
    @masc_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        trat_Id,
        CONCAT(trat_Medicamento, ' - ', FORMAT(trat_FechaAplicacion, 'dd/MM/yyyy')) AS Descripcion
    FROM [Medico].[tbTratamientos]
    WHERE trat_EsEliminado = 0
    AND (@masc_Id IS NULL OR masc_Id = @masc_Id)
    ORDER BY trat_FechaAplicacion DESC
END
GO

PRINT '=========================================='
PRINT '✅ STORED PROCEDURES TRATAMIENTOS CREADOS'
PRINT '=========================================='
PRINT 'Total: 7 procedimientos'
PRINT '=========================================='
GO
