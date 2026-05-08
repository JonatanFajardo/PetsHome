        /*
        =============================================
        SCRIPT: Stored Procedures para AlertaMedica (Dashboard)
        GENERADO POR: scaffold_dashboard.py
        FECHA: 2026-04-25
        =============================================
        */

        USE PETSHOMEDB
        GO

            -- ============================================
    -- PR_Medico_AlertaMedica_VacunasVencidas
    -- ============================================
    CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_AlertaMedica_VacunasVencidas]
    AS
    BEGIN
        SET NOCOUNT ON

        SELECT
            masc_Id  -- INT,
MascotaNombre  -- NVARCHAR(200),
Raza  -- NVARCHAR(200),
Edad  -- NVARCHAR(200),
VacunaNombre  -- NVARCHAR(200),
FechaUltimaVacuna  -- DATETIME,
DiasVencida  -- INT
        -- TODO: completar FROM, JOINs y WHERE
        FROM [?].[?]
        WHERE 1=1
        ORDER BY 1
    END
    GO

    -- ============================================
    -- PR_Medico_AlertaMedica_TratamientosPorVencer
    -- ============================================
    CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_AlertaMedica_TratamientosPorVencer]
    AS
    BEGIN
        SET NOCOUNT ON

        SELECT
            trat_Id  -- INT,
masc_Id  -- INT,
MascotaNombre  -- NVARCHAR(200),
Raza  -- NVARCHAR(200),
TratamientoNombre  -- NVARCHAR(200),
trat_ProximaDosis  -- DATETIME,
DiasRestantes  -- INT,
PorcentajeRestante  -- INT
        -- TODO: completar FROM, JOINs y WHERE
        FROM [?].[?]
        WHERE 1=1
        ORDER BY 1
    END
    GO

    -- ============================================
    -- PR_Medico_AlertaMedica_RecetasSinRevision
    -- ============================================
    CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_AlertaMedica_RecetasSinRevision]
    AS
    BEGIN
        SET NOCOUNT ON

        SELECT
            receta_Id  -- INT,
masc_Id  -- INT,
MascotaNombre  -- NVARCHAR(200),
Raza  -- NVARCHAR(200),
receta_Medicamento  -- NVARCHAR(200),
receta_Estado  -- NVARCHAR(200),
DuracionDias  -- INT
        -- TODO: completar FROM, JOINs y WHERE
        FROM [?].[?]
        WHERE 1=1
        ORDER BY 1
    END
    GO

    -- ============================================
    -- PR_Medico_AlertaMedica_SinConsulta
    -- ============================================
    CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_AlertaMedica_SinConsulta]
    AS
    BEGIN
        SET NOCOUNT ON

        SELECT
            masc_Id  -- INT,
MascotaNombre  -- NVARCHAR(200),
Raza  -- NVARCHAR(200),
UltimaVisita  -- DATETIME,
TiempoSinConsulta  -- NVARCHAR(200)
        -- TODO: completar FROM, JOINs y WHERE
        FROM [?].[?]
        WHERE 1=1
        ORDER BY 1
    END
    GO

        PRINT '=========================================='
        PRINT 'SPs AlertaMedica CREADOS'
        PRINT '=========================================='
        PRINT '    PR_Medico_AlertaMedica_VacunasVencidas'
PRINT '    PR_Medico_AlertaMedica_TratamientosPorVencer'
PRINT '    PR_Medico_AlertaMedica_RecetasSinRevision'
PRINT '    PR_Medico_AlertaMedica_SinConsulta'
        PRINT '=========================================='
        GO
