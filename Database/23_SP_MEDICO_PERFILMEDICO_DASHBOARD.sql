        /*
        =============================================
        SCRIPT: Stored Procedures para PerfilMedico (Dashboard)
        GENERADO POR: scaffold_dashboard.py
        FECHA: 2026-04-25
        =============================================
        */

        USE PETSHOMEDB
        GO

                -- ============================================
        -- PR_Medico_PerfilMedico_FichaMascota
        -- ============================================
        CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_PerfilMedico_FichaMascota]
@masc_Id INT

        AS
        BEGIN
            SET NOCOUNT ON

            SELECT
                masc_Id  -- INT,
    masc_Nombre  -- NVARCHAR(200),
    Raza  -- NVARCHAR(200),
    Edad  -- NVARCHAR(200),
    Sexo  -- NVARCHAR(200),
    EsEsterilizada  -- BIT,
    Microchip  -- NVARCHAR(200),
    Peso  -- DECIMAL(10,2),
    Adoptante  -- NVARCHAR(200),
    Refugio  -- NVARCHAR(200),
    UltimaVisita  -- DATETIME,
    EstadoSalud  -- NVARCHAR(200),
    TotalCitas  -- INT,
    TratamientosActivos  -- INT,
    VacunasAlDia  -- INT
            -- TODO: completar FROM, JOINs y WHERE
            -- TODO: filtrar por parametro(s):
            -- AND t.masc_Id = @masc_Id
            FROM [?].[?]
            WHERE 1=1
            ORDER BY 1
        END
        GO

        -- ============================================
        -- PR_Medico_PerfilMedico_UltimasCitas
        -- ============================================
        CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_PerfilMedico_UltimasCitas]
@masc_Id INT

        AS
        BEGIN
            SET NOCOUNT ON

            SELECT
                cita_Id  -- INT,
    cita_FechaConsulta  -- DATETIME,
    TipoConsulta  -- NVARCHAR(200),
    cita_Diagnostico  -- NVARCHAR(200),
    Veterinario  -- NVARCHAR(200)
            -- TODO: completar FROM, JOINs y WHERE
            -- TODO: filtrar por parametro(s):
            -- AND t.masc_Id = @masc_Id
            FROM [?].[?]
            WHERE 1=1
            ORDER BY 1
        END
        GO

        -- ============================================
        -- PR_Medico_PerfilMedico_MedicamentosActivos
        -- ============================================
        CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_PerfilMedico_MedicamentosActivos]
@masc_Id INT

        AS
        BEGIN
            SET NOCOUNT ON

            SELECT
                trat_Id  -- INT,
    Medicamento  -- NVARCHAR(200),
    Dosis  -- NVARCHAR(200),
    DiasRestantes  -- INT,
    PorcentajeCompletado  -- INT
            -- TODO: completar FROM, JOINs y WHERE
            -- TODO: filtrar por parametro(s):
            -- AND t.masc_Id = @masc_Id
            FROM [?].[?]
            WHERE 1=1
            ORDER BY 1
        END
        GO

        -- ============================================
        -- PR_Medico_PerfilMedico_TodasCitas
        -- ============================================
        CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_PerfilMedico_TodasCitas]
@masc_Id INT

        AS
        BEGIN
            SET NOCOUNT ON

            SELECT
                cita_Id  -- INT,
    cita_FechaConsulta  -- DATETIME,
    TipoConsulta  -- NVARCHAR(200),
    cita_Diagnostico  -- NVARCHAR(200),
    Veterinario  -- NVARCHAR(200),
    Hora  -- NVARCHAR(200)
            -- TODO: completar FROM, JOINs y WHERE
            -- TODO: filtrar por parametro(s):
            -- AND t.masc_Id = @masc_Id
            FROM [?].[?]
            WHERE 1=1
            ORDER BY 1
        END
        GO

        -- ============================================
        -- PR_Medico_PerfilMedico_Tratamientos
        -- ============================================
        CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_PerfilMedico_Tratamientos]
@masc_Id INT

        AS
        BEGIN
            SET NOCOUNT ON

            SELECT
                trat_Id  -- INT,
    NombreTratamiento  -- NVARCHAR(200),
    Medicamento  -- NVARCHAR(200),
    trat_FechaInicio  -- DATETIME,
    trat_FechaFin  -- DATETIME,
    PorcentajeCompletado  -- INT,
    EstadoTratamiento  -- NVARCHAR(200)
            -- TODO: completar FROM, JOINs y WHERE
            -- TODO: filtrar por parametro(s):
            -- AND t.masc_Id = @masc_Id
            FROM [?].[?]
            WHERE 1=1
            ORDER BY 1
        END
        GO

        -- ============================================
        -- PR_Medico_PerfilMedico_Vacunas
        -- ============================================
        CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_PerfilMedico_Vacunas]
@masc_Id INT

        AS
        BEGIN
            SET NOCOUNT ON

            SELECT
                vac_Id  -- INT,
    VacunaNombre  -- NVARCHAR(200),
    FechaAplicada  -- DATETIME,
    FechaProxima  -- DATETIME,
    EstadoVacuna  -- NVARCHAR(200)
            -- TODO: completar FROM, JOINs y WHERE
            -- TODO: filtrar por parametro(s):
            -- AND t.masc_Id = @masc_Id
            FROM [?].[?]
            WHERE 1=1
            ORDER BY 1
        END
        GO

        PRINT '=========================================='
        PRINT 'SPs PerfilMedico CREADOS'
        PRINT '=========================================='
        PRINT '    PR_Medico_PerfilMedico_FichaMascota'
PRINT '    PR_Medico_PerfilMedico_UltimasCitas'
PRINT '    PR_Medico_PerfilMedico_MedicamentosActivos'
PRINT '    PR_Medico_PerfilMedico_TodasCitas'
PRINT '    PR_Medico_PerfilMedico_Tratamientos'
PRINT '    PR_Medico_PerfilMedico_Vacunas'
        PRINT '=========================================='
        GO
