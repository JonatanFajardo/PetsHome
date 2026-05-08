/*
=============================================
SCRIPT: Stored Procedures para Perfil Médico de Mascota
AUTOR: Sistema PetsHome
FECHA: 2026-04-26
=============================================
*/

USE PETSHOMEDB
GO

-- =============================================
-- SP 1: PR_Medico_PerfilMedico_FichaMascota
-- Retorna los datos de cabecera de la mascota
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_PerfilMedico_FichaMascota')
    DROP PROCEDURE [Medico].[PR_Medico_PerfilMedico_FichaMascota]
GO

CREATE PROCEDURE [Medico].[PR_Medico_PerfilMedico_FichaMascota]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT TOP 1
        masc.masc_Id,
        masc.masc_Nombre,
        raza.raza_Descripcion                                   AS Raza,
        CAST(masc.masc_Edad AS VARCHAR) + ' año(s)'             AS Edad,
        masc.masc_Sexo                                          AS Sexo,
        CAST(0 AS BIT)                                          AS EsEsterilizada,
        NULL                                                    AS Microchip,
        masc.masc_Peso                                          AS Peso,
        (
            SELECT TOP 1 sol2.sol_Nombres + ' ' + sol2.sol_Apellidos
            FROM   [Refugio].[tbSolicitudes]  sol2
            INNER JOIN [Refugio].[tbAdopciones] adop2
                   ON  adop2.sol_Id         = sol2.sol_Id
            WHERE  sol2.masc_Id             = masc.masc_Id
              AND  adop2.adop_EsAprobado    = 1
              AND  adop2.adop_EsEliminado   = 0
              AND  sol2.sol_EsEliminado     = 0
            ORDER BY adop2.adop_FechaCrea DESC
        )                                                       AS Adoptante,
        refg.refg_Nombre                                        AS Refugio,
        (
            SELECT MAX(c.cita_FechaConsulta)
            FROM   [Medico].[tbCitaMedica] c
            WHERE  c.masc_Id = masc.masc_Id AND c.cita_EsEliminado = 0
        )                                                       AS UltimaVisita,
        CASE
            WHEN EXISTS (
                SELECT 1 FROM [Medico].[tbTratamientos] t
                WHERE  t.masc_Id = masc.masc_Id
                  AND  t.trat_Estado    = 'Activo'
                  AND  t.trat_EsEliminado = 0
            ) THEN 'En Tratamiento'
            ELSE 'Saludable'
        END                                                     AS EstadoSalud,
        (
            SELECT COUNT(*)
            FROM   [Medico].[tbCitaMedica] c
            WHERE  c.masc_Id = masc.masc_Id AND c.cita_EsEliminado = 0
        )                                                       AS TotalCitas,
        (
            SELECT COUNT(*)
            FROM   [Medico].[tbTratamientos] t
            WHERE  t.masc_Id = masc.masc_Id
              AND  t.trat_Estado    = 'Activo'
              AND  t.trat_EsEliminado = 0
        )                                                       AS TratamientosActivos,
        (
            SELECT COUNT(*)
            FROM   [Medico].[tbCitaMedica] c
            WHERE  c.masc_Id      = masc.masc_Id
              AND  c.vac_Id       IS NOT NULL
              AND  c.cita_EsEliminado = 0
              AND  ISNULL(c.cita_ProximaCita, '2099-01-01') > GETDATE()
        )                                                       AS VacunasAlDia
    FROM  [Refugio].[tbMascotas]  AS masc
    LEFT JOIN [Refugio].[tbRazas]    AS raza ON masc.raza_Id = raza.raza_Id
    LEFT JOIN [Refugio].[tbRefugios] AS refg ON masc.refg_Id = refg.refg_Id
    WHERE masc.masc_Id         = @masc_Id
      AND masc.masc_EsEliminado = 0
END
GO

-- =============================================
-- SP 2: PR_Medico_PerfilMedico_UltimasCitas
-- Top 4 citas más recientes (tab Resumen)
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_PerfilMedico_UltimasCitas')
    DROP PROCEDURE [Medico].[PR_Medico_PerfilMedico_UltimasCitas]
GO

CREATE PROCEDURE [Medico].[PR_Medico_PerfilMedico_UltimasCitas]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT TOP 4
        cita.cita_Id,
        cita.cita_FechaConsulta,
        tipoCon.tipoCon_Descripcion     AS TipoConsulta,
        cita.cita_Diagnostico,
        usu.usu_Nombre                  AS Veterinario
    FROM  [Medico].[tbCitaMedica]       AS cita
    LEFT JOIN [Medico].[tbTiposConsulta] AS tipoCon
           ON  cita.tipoCon_Id = tipoCon.tipoCon_Id
    LEFT JOIN [Seguridad].[tbUsuarios]   AS usu
           ON  cita.cita_UsuarioCrea = usu.usu_Id
    WHERE  cita.masc_Id         = @masc_Id
      AND  cita.cita_EsEliminado = 0
    ORDER BY cita.cita_FechaConsulta DESC
END
GO

-- =============================================
-- SP 3: PR_Medico_PerfilMedico_MedicamentosActivos
-- Recetas activas con progreso y días restantes
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_PerfilMedico_MedicamentosActivos')
    DROP PROCEDURE [Medico].[PR_Medico_PerfilMedico_MedicamentosActivos]
GO

CREATE PROCEDURE [Medico].[PR_Medico_PerfilMedico_MedicamentosActivos]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        receta.receta_Id                                AS trat_Id,
        receta.receta_Medicamento                       AS Medicamento,
        ISNULL(receta.receta_Dosis, '')
            + CASE
                WHEN receta.receta_Frecuencia IS NOT NULL
                THEN ' · ' + receta.receta_Frecuencia
                ELSE ''
              END                                       AS Dosis,
        DATEDIFF(day, GETDATE(), receta.receta_FechaFin) AS DiasRestantes,
        CASE
            WHEN receta.receta_FechaFin IS NULL OR receta.receta_FechaInicio IS NULL THEN 0
            WHEN DATEDIFF(day, receta.receta_FechaInicio, GETDATE()) <= 0            THEN 0
            ELSE CAST(
                CASE
                    WHEN (DATEDIFF(day, receta.receta_FechaInicio, GETDATE()) * 100.0)
                         / NULLIF(DATEDIFF(day, receta.receta_FechaInicio, receta.receta_FechaFin), 0) > 100
                    THEN 100
                    ELSE (DATEDIFF(day, receta.receta_FechaInicio, GETDATE()) * 100.0)
                         / NULLIF(DATEDIFF(day, receta.receta_FechaInicio, receta.receta_FechaFin), 0)
                END AS INT)
        END                                             AS PorcentajeCompletado
    FROM  [Medico].[tbRecetas] AS receta
    WHERE  receta.masc_Id          = @masc_Id
      AND  receta.receta_EsEliminado = 0
      AND  receta.receta_Estado      = 'Activo'
    ORDER BY receta.receta_FechaInicio DESC
END
GO

-- =============================================
-- SP 4: PR_Medico_PerfilMedico_TodasCitas
-- Todas las citas (tab Citas)
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_PerfilMedico_TodasCitas')
    DROP PROCEDURE [Medico].[PR_Medico_PerfilMedico_TodasCitas]
GO

CREATE PROCEDURE [Medico].[PR_Medico_PerfilMedico_TodasCitas]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        cita.cita_Id,
        cita.cita_FechaConsulta,
        tipoCon.tipoCon_Descripcion             AS TipoConsulta,
        cita.cita_Diagnostico,
        usu.usu_Nombre                          AS Veterinario,
        CONVERT(VARCHAR(8), cita.cita_FechaConsulta, 108) AS Hora
    FROM  [Medico].[tbCitaMedica]               AS cita
    LEFT JOIN [Medico].[tbTiposConsulta]         AS tipoCon
           ON  cita.tipoCon_Id = tipoCon.tipoCon_Id
    LEFT JOIN [Seguridad].[tbUsuarios]           AS usu
           ON  cita.cita_UsuarioCrea = usu.usu_Id
    WHERE  cita.masc_Id         = @masc_Id
      AND  cita.cita_EsEliminado = 0
    ORDER BY cita.cita_FechaConsulta DESC
END
GO

-- =============================================
-- SP 5: PR_Medico_PerfilMedico_Tratamientos
-- Todos los tratamientos con progreso calculado
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_PerfilMedico_Tratamientos')
    DROP PROCEDURE [Medico].[PR_Medico_PerfilMedico_Tratamientos]
GO

CREATE PROCEDURE [Medico].[PR_Medico_PerfilMedico_Tratamientos]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        trat.trat_Id,
        ISNULL(tipoMed.tipoMed_Descripcion, trat.trat_Medicamento) AS NombreTratamiento,
        trat.trat_Medicamento
            + ISNULL(' · ' + viaAdmin.viaAdmin_Descripcion, '')    AS Medicamento,
        trat.trat_FechaAplicacion                                   AS trat_FechaInicio,
        trat.trat_ProximaDosis                                      AS trat_FechaFin,
        CASE
            WHEN trat.trat_ProximaDosis IS NULL OR trat.trat_FechaAplicacion IS NULL THEN
                CASE WHEN trat.trat_Estado = 'Activo' THEN 50 ELSE 100 END
            WHEN DATEDIFF(day, trat.trat_FechaAplicacion, GETDATE()) <= 0 THEN 0
            ELSE CAST(
                CASE
                    WHEN (DATEDIFF(day, trat.trat_FechaAplicacion, GETDATE()) * 100.0)
                         / NULLIF(DATEDIFF(day, trat.trat_FechaAplicacion, trat.trat_ProximaDosis), 0) > 100
                    THEN 100
                    ELSE (DATEDIFF(day, trat.trat_FechaAplicacion, GETDATE()) * 100.0)
                         / NULLIF(DATEDIFF(day, trat.trat_FechaAplicacion, trat.trat_ProximaDosis), 0)
                END AS INT)
        END                                                         AS PorcentajeCompletado,
        trat.trat_Estado                                            AS EstadoTratamiento
    FROM  [Medico].[tbTratamientos]             AS trat
    LEFT JOIN [Medico].[tbTiposMedicamento]      AS tipoMed
           ON  trat.tipoMed_Id = tipoMed.tipoMed_Id
    LEFT JOIN [Medico].[tbViasAdministracion]    AS viaAdmin
           ON  trat.viaAdmin_Id = viaAdmin.viaAdmin_Id
    WHERE  trat.masc_Id         = @masc_Id
      AND  trat.trat_EsEliminado = 0
    ORDER BY
        CASE WHEN trat.trat_Estado = 'Activo' THEN 0 ELSE 1 END,
        trat.trat_FechaAplicacion DESC
END
GO

-- =============================================
-- SP 6: PR_Medico_PerfilMedico_Vacunas
-- Cartilla de vacunación: última aplicación por vacuna
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_PerfilMedico_Vacunas')
    DROP PROCEDURE [Medico].[PR_Medico_PerfilMedico_Vacunas]
GO

CREATE PROCEDURE [Medico].[PR_Medico_PerfilMedico_Vacunas]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        vac.vac_Id,
        vac.vac_Descripcion                     AS VacunaNombre,
        cita.cita_FechaConsulta                 AS FechaAplicada,
        cita.cita_ProximaCita                   AS FechaProxima,
        CASE
            WHEN cita.cita_ProximaCita IS NULL                          THEN 'Al día'
            WHEN cita.cita_ProximaCita < GETDATE()                      THEN 'Vencida'
            WHEN DATEDIFF(day, GETDATE(), cita.cita_ProximaCita) <= 30  THEN 'Próxima'
            ELSE 'Al día'
        END                                     AS EstadoVacuna
    FROM (
        -- Última cita por vacuna para esta mascota
        SELECT   vac_Id, MAX(cita_Id) AS last_cita_Id
        FROM     [Medico].[tbCitaMedica]
        WHERE    masc_Id          = @masc_Id
          AND    vac_Id           IS NOT NULL
          AND    cita_EsEliminado = 0
        GROUP BY vac_Id
    ) AS ultima
    INNER JOIN [Medico].[tbCitaMedica]  AS cita ON cita.cita_Id  = ultima.last_cita_Id
    INNER JOIN [Refugio].[tbVacunas]    AS vac  ON vac.vac_Id    = ultima.vac_Id
    WHERE  vac.vac_EsEliminado = 0
    ORDER BY cita.cita_FechaConsulta DESC
END
GO
