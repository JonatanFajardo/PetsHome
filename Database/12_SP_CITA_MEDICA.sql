/*
=============================================
SCRIPT: Stored Procedures para tbCitaMedica
AUTOR: Sistema PetsHome
FECHA: 2025-11-04
=============================================
*/

USE PETSHOMEDB
GO

-- =============================================
-- SP: PR_Medico_CitaMedica_List
-- Descripción: Lista todas las citas médicas activas
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_CitaMedica_List')
    DROP PROCEDURE [Medico].[PR_Medico_CitaMedica_List]
GO

CREATE PROCEDURE [Medico].[PR_Medico_CitaMedica_List]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ROW_NUMBER() OVER(ORDER BY cita.cita_FechaConsulta DESC) AS Fila,
        cita.cita_Id,
        cita.masc_Id,
        masc.masc_Nombre AS Mascota,
        cita.cita_FechaConsulta,
        cita.tipoCon_Id,
        tipoCon.tipoCon_Descripcion AS TipoConsulta,
        cita.grav_Id,
        grav.grav_Descripcion AS Gravedad,
        cita.cita_MotivoConsulta,
        cita.cita_Diagnostico,
        cita.cita_Peso,
        cita.cita_Temperatura,
        cita.cita_FrecuenciaCardiaca,
        cita.cita_FrecuenciaRespiratoria,
        cita.cita_ProximaCita
    FROM [Medico].[tbCitaMedica] AS cita
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON cita.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposConsulta] AS tipoCon
        ON cita.tipoCon_Id = tipoCon.tipoCon_Id
    LEFT JOIN [Medico].[tbGravedades] AS grav
        ON cita.grav_Id = grav.grav_Id
    WHERE cita.cita_EsEliminado = 0
    ORDER BY cita.cita_FechaConsulta DESC
END
GO

-- =============================================
-- SP: PR_Medico_CitaMedica_Detail
-- Descripción: Detalle de cita con info de auditoría
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_CitaMedica_Detail')
    DROP PROCEDURE [Medico].[PR_Medico_CitaMedica_Detail]
GO

CREATE PROCEDURE [Medico].[PR_Medico_CitaMedica_Detail]
    @cita_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        cita.cita_Id,
        cita.masc_Id,
        masc.masc_Nombre AS Mascota,
        cita.cita_FechaConsulta,
        cita.tipoCon_Id,
        tipoCon.tipoCon_Descripcion AS TipoConsulta,
        cita.grav_Id,
        grav.grav_Descripcion AS Gravedad,
        cita.cita_MotivoConsulta,
        cita.cita_Diagnostico,
        cita.cita_Peso,
        cita.cita_Temperatura,
        cita.cita_FrecuenciaCardiaca,
        cita.cita_FrecuenciaRespiratoria,
        cita.com_Id,
        com.com_Descripcion AS Comportamiento,
        cita.cita_ProcedimientosRealizados,
        cita.cita_ResultadosExamenes,
        cita.cita_ProximaCita,
        cita.cita_MotivoProximaCita,
        usuarioCrea.usu_Nombre AS UsuarioCreacion,
        cita.cita_FechaCrea,
        usuarioModifica.usu_Nombre AS UsuarioModificacion,
        cita.cita_FechaModifica
    FROM [Medico].[tbCitaMedica] AS cita
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON cita.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposConsulta] AS tipoCon
        ON cita.tipoCon_Id = tipoCon.tipoCon_Id
    LEFT JOIN [Medico].[tbGravedades] AS grav
        ON cita.grav_Id = grav.grav_Id
    LEFT JOIN [Refugio].[tbComportamientos] AS com
        ON cita.com_Id = com.com_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON cita.cita_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON cita.cita_UsuarioModifica = usuarioModifica.usu_Id
    WHERE cita.cita_Id = @cita_Id
    AND cita.cita_EsEliminado = 0
END
GO

-- =============================================
-- SP: PR_Medico_CitaMedica_Find
-- Descripción: Buscar cita por ID para edición
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_CitaMedica_Find')
    DROP PROCEDURE [Medico].[PR_Medico_CitaMedica_Find]
GO

CREATE PROCEDURE [Medico].[PR_Medico_CitaMedica_Find]
    @cita_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        cita.cita_Id,
        cita.masc_Id,
        cita.cita_FechaConsulta,
        cita.tipoCon_Id,
        cita.grav_Id,
        cita.cita_MotivoConsulta,
        cita.cita_Diagnostico,
        cita.cita_Peso,
        cita.cita_Temperatura,
        cita.cita_FrecuenciaCardiaca,
        cita.cita_FrecuenciaRespiratoria,
        cita.com_Id,
        cita.vac_Id,
        cita.cita_ProcedimientosRealizados,
        cita.cita_ResultadosExamenes,
        cita.cita_ProximaCita,
        cita.cita_MotivoProximaCita,
        cita.cita_UsuarioCrea,
        usuarioCrea.usu_Nombre AS usuarioCrea,
        cita.cita_FechaCrea,
        cita.cita_UsuarioModifica,
        usuarioModifica.usu_Nombre AS usuarioModifica,
        cita.cita_FechaModifica
    FROM [Medico].[tbCitaMedica] AS cita
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON cita.cita_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON cita.cita_UsuarioModifica = usuarioModifica.usu_Id
    WHERE cita.cita_Id = @cita_Id
    AND cita.cita_EsEliminado = 0
END
GO

-- =============================================
-- SP: PR_Medico_CitaMedica_Insert
-- Descripción: Insertar nueva cita médica
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_CitaMedica_Insert')
    DROP PROCEDURE [Medico].[PR_Medico_CitaMedica_Insert]
GO

CREATE PROCEDURE [Medico].[PR_Medico_CitaMedica_Insert]
    @masc_Id INT,
    @cita_FechaConsulta DATETIME,
    @tipoCon_Id INT = NULL,
    @grav_Id INT = NULL,
    @cita_MotivoConsulta NVARCHAR(500) = NULL,
    @cita_Diagnostico NVARCHAR(500) = NULL,
    @cita_Peso DECIMAL(5,2) = NULL,
    @cita_Temperatura DECIMAL(4,2) = NULL,
    @cita_FrecuenciaCardiaca INT = NULL,
    @cita_FrecuenciaRespiratoria INT = NULL,
    @com_Id INT = NULL,
    @vac_Id INT = NULL,
    @cita_ProcedimientosRealizados NVARCHAR(500) = NULL,
    @cita_ResultadosExamenes NVARCHAR(500) = NULL,
    @cita_ProximaCita DATETIME = NULL,
    @cita_MotivoProximaCita NVARCHAR(200) = NULL,
    @cita_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [Medico].[tbCitaMedica]
    (
        masc_Id,
        cita_FechaConsulta,
        tipoCon_Id,
        grav_Id,
        cita_MotivoConsulta,
        cita_Diagnostico,
        cita_Peso,
        cita_Temperatura,
        cita_FrecuenciaCardiaca,
        cita_FrecuenciaRespiratoria,
        com_Id,
        vac_Id,
        cita_ProcedimientosRealizados,
        cita_ResultadosExamenes,
        cita_ProximaCita,
        cita_MotivoProximaCita,
        cita_UsuarioCrea,
        cita_FechaCrea
    )
    VALUES
    (
        @masc_Id,
        @cita_FechaConsulta,
        @tipoCon_Id,
        @grav_Id,
        @cita_MotivoConsulta,
        @cita_Diagnostico,
        @cita_Peso,
        @cita_Temperatura,
        @cita_FrecuenciaCardiaca,
        @cita_FrecuenciaRespiratoria,
        @com_Id,
        @vac_Id,
        @cita_ProcedimientosRealizados,
        @cita_ResultadosExamenes,
        @cita_ProximaCita,
        @cita_MotivoProximaCita,
        @cita_UsuarioCrea,
        GETDATE()
    )

    SELECT SCOPE_IDENTITY() AS cita_Id
END
GO

-- =============================================
-- SP: PR_Medico_CitaMedica_Update
-- Descripción: Actualizar cita médica
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_CitaMedica_Update')
    DROP PROCEDURE [Medico].[PR_Medico_CitaMedica_Update]
GO

CREATE PROCEDURE [Medico].[PR_Medico_CitaMedica_Update]
    @cita_Id INT,
    @masc_Id INT,
    @cita_FechaConsulta DATETIME,
    @tipoCon_Id INT = NULL,
    @grav_Id INT = NULL,
    @cita_MotivoConsulta NVARCHAR(500) = NULL,
    @cita_Diagnostico NVARCHAR(500) = NULL,
    @cita_Peso DECIMAL(5,2) = NULL,
    @cita_Temperatura DECIMAL(4,2) = NULL,
    @cita_FrecuenciaCardiaca INT = NULL,
    @cita_FrecuenciaRespiratoria INT = NULL,
    @com_Id INT = NULL,
    @vac_Id INT = NULL,
    @cita_ProcedimientosRealizados NVARCHAR(500) = NULL,
    @cita_ResultadosExamenes NVARCHAR(500) = NULL,
    @cita_ProximaCita DATETIME = NULL,
    @cita_MotivoProximaCita NVARCHAR(200) = NULL,
    @cita_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON

    UPDATE [Medico].[tbCitaMedica]
    SET
        masc_Id = @masc_Id,
        cita_FechaConsulta = @cita_FechaConsulta,
        tipoCon_Id = @tipoCon_Id,
        grav_Id = @grav_Id,
        cita_MotivoConsulta = @cita_MotivoConsulta,
        cita_Diagnostico = @cita_Diagnostico,
        cita_Peso = @cita_Peso,
        cita_Temperatura = @cita_Temperatura,
        cita_FrecuenciaCardiaca = @cita_FrecuenciaCardiaca,
        cita_FrecuenciaRespiratoria = @cita_FrecuenciaRespiratoria,
        com_Id = @com_Id,
        vac_Id = @vac_Id,
        cita_ProcedimientosRealizados = @cita_ProcedimientosRealizados,
        cita_ResultadosExamenes = @cita_ResultadosExamenes,
        cita_ProximaCita = @cita_ProximaCita,
        cita_MotivoProximaCita = @cita_MotivoProximaCita,
        cita_UsuarioModifica = @cita_UsuarioModifica,
        cita_FechaModifica = GETDATE()
    WHERE cita_Id = @cita_Id
END
GO

-- =============================================
-- SP: PR_Medico_CitaMedica_Delete
-- Descripción: Eliminación lógica de cita médica
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_CitaMedica_Delete')
    DROP PROCEDURE [Medico].[PR_Medico_CitaMedica_Delete]
GO

CREATE PROCEDURE [Medico].[PR_Medico_CitaMedica_Delete]
    @cita_Id INT,
    @cita_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON

    UPDATE [Medico].[tbCitaMedica]
    SET
        cita_EsEliminado = 1,
        cita_UsuarioModifica = @cita_UsuarioModifica,
        cita_FechaModifica = GETDATE()
    WHERE cita_Id = @cita_Id
END
GO

-- =============================================
-- SP: PR_Medico_CitaMedica_Dropdown
-- Descripción: Lista para dropdowns (citas de una mascota)
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_CitaMedica_Dropdown')
    DROP PROCEDURE [Medico].[PR_Medico_CitaMedica_Dropdown]
GO

CREATE PROCEDURE [Medico].[PR_Medico_CitaMedica_Dropdown]
    @masc_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        cita.cita_Id,
        CONCAT('#', cita.cita_Id, ' - ', FORMAT(cita.cita_FechaConsulta, 'dd/MM/yyyy'),
               ' - ', ISNULL(tipoCon.tipoCon_Descripcion, 'Sin tipo')) AS Descripcion
    FROM [Medico].[tbCitaMedica] AS cita
    LEFT JOIN [Medico].[tbTiposConsulta] AS tipoCon
        ON cita.tipoCon_Id = tipoCon.tipoCon_Id
    WHERE cita.cita_EsEliminado = 0
    AND (@masc_Id IS NULL OR cita.masc_Id = @masc_Id)
    ORDER BY cita.cita_FechaConsulta DESC
END
GO

PRINT '=========================================='
PRINT '✅ STORED PROCEDURES CITA MÉDICA CREADOS'
PRINT '=========================================='
PRINT 'Total: 7 procedimientos'
PRINT '- PR_Medico_CitaMedica_List'
PRINT '- PR_Medico_CitaMedica_Detail'
PRINT '- PR_Medico_CitaMedica_Find'
PRINT '- PR_Medico_CitaMedica_Insert'
PRINT '- PR_Medico_CitaMedica_Update'
PRINT '- PR_Medico_CitaMedica_Delete'
PRINT '- PR_Medico_CitaMedica_Dropdown'
PRINT '=========================================='
GO
