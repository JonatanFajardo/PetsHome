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
MascotaNombre  -- NVARCHAR(200)
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
MascotaNombre  -- NVARCHAR(200)
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
PRINT '    PR_Medico_AlertaMedica_SinConsulta'
        PRINT '=========================================='
        GO
