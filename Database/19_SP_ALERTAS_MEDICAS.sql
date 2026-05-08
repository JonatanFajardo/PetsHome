/*
=============================================
SCRIPT: Stored Procedures para Alertas Medicas
DESCRIPCION: Dashboard de alertas medicas (solo lectura)
             - Vacunas vencidas
             - Tratamientos por vencer
             - Recetas sin revision
             - Mascotas sin consulta reciente
=============================================
*/

USE PETSHOMEDB
GO

-- =======================================
-- SP: PR_Medico_AlertaMedica_VacunasVencidas
-- Mascotas cuya vacuna mas reciente supero 12 meses
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_AlertaMedica_VacunasVencidas')
    DROP PROCEDURE [Medico].[PR_Medico_AlertaMedica_VacunasVencidas]
GO

CREATE PROCEDURE [Medico].[PR_Medico_AlertaMedica_VacunasVencidas]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ult.masc_Id,
        masc.masc_Nombre AS MascotaNombre,
        ISNULL(raza.raza_Descripcion, 'Sin raza') AS Raza,
        ISNULL(CAST(masc.masc_Edad AS NVARCHAR(10)), '?') AS Edad,
        vac.vac_Descripcion AS VacunaNombre,
        ult.FechaUltimaVacuna,
        DATEDIFF(DAY, ult.FechaUltimaVacuna, GETDATE()) - 365 AS DiasVencida
    FROM (
        -- Ultima cita con vacuna por mascota
        SELECT
            c.masc_Id,
            c.vac_Id,
            MAX(c.cita_FechaConsulta) AS FechaUltimaVacuna
        FROM [Medico].[tbCitaMedica] AS c
        WHERE c.vac_Id IS NOT NULL
          AND c.cita_EsEliminado = 0
        GROUP BY c.masc_Id, c.vac_Id
    ) AS ult
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON ult.masc_Id = masc.masc_Id
    INNER JOIN [Refugio].[tbVacunas] AS vac
        ON ult.vac_Id = vac.vac_Id
    LEFT JOIN [Refugio].[tbRazas] AS raza
        ON masc.raza_Id = raza.raza_Id
    WHERE DATEDIFF(DAY, ult.FechaUltimaVacuna, GETDATE()) > 365
      AND masc.masc_EsEliminado = 0
    ORDER BY DiasVencida DESC
END
GO

-- =======================================
-- SP: PR_Medico_AlertaMedica_TratamientosPorVencer
-- Tratamientos activos cuya proxima dosis es en los proximos 15 dias
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_AlertaMedica_TratamientosPorVencer')
    DROP PROCEDURE [Medico].[PR_Medico_AlertaMedica_TratamientosPorVencer]
GO

CREATE PROCEDURE [Medico].[PR_Medico_AlertaMedica_TratamientosPorVencer]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        trat.trat_Id,
        trat.masc_Id,
        masc.masc_Nombre AS MascotaNombre,
        ISNULL(raza.raza_Descripcion, 'Sin raza') AS Raza,
        ISNULL(CAST(masc.masc_Edad AS NVARCHAR(10)), '?') AS Edad,
        ISNULL(trat.trat_Medicamento, tipoPar.tipoPar_Descripcion) AS TratamientoNombre,
        trat.trat_ProximaDosis,
        DATEDIFF(DAY, GETDATE(), trat.trat_ProximaDosis) AS DiasRestantes,
        -- Porcentaje de progreso: 0% = hoy vence, 100% = queda todo (15 dias)
        CASE
            WHEN DATEDIFF(DAY, GETDATE(), trat.trat_ProximaDosis) <= 0 THEN 0
            WHEN DATEDIFF(DAY, GETDATE(), trat.trat_ProximaDosis) >= 15  THEN 100
            ELSE CAST((DATEDIFF(DAY, GETDATE(), trat.trat_ProximaDosis) * 100.0 / 15) AS INT)
        END AS PorcentajeRestante
    FROM [Medico].[tbTratamientos] AS trat
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON trat.masc_Id = masc.masc_Id
    LEFT JOIN [Refugio].[tbRazas] AS raza
        ON masc.raza_Id = raza.raza_Id
    LEFT JOIN [Medico].[tbTiposParasito] AS tipoPar
        ON trat.tipoPar_Id = tipoPar.tipoPar_Id
    WHERE trat.trat_EsEliminado = 0
      AND trat.trat_ProximaDosis IS NOT NULL
      AND trat.trat_ProximaDosis <= DATEADD(DAY, 15, GETDATE())
      AND masc.masc_EsEliminado = 0
    ORDER BY trat.trat_ProximaDosis ASC
END
GO

-- =======================================
-- SP: PR_Medico_AlertaMedica_RecetasSinRevision
-- Recetas activas aun vigentes
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_AlertaMedica_RecetasSinRevision')
    DROP PROCEDURE [Medico].[PR_Medico_AlertaMedica_RecetasSinRevision]
GO

CREATE PROCEDURE [Medico].[PR_Medico_AlertaMedica_RecetasSinRevision]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        rec.receta_Id,
        rec.masc_Id,
        masc.masc_Nombre AS MascotaNombre,
        ISNULL(raza.raza_Descripcion, 'Sin raza') AS Raza,
        ISNULL(CAST(masc.masc_Edad AS NVARCHAR(10)), '?') AS Edad,
        rec.receta_Medicamento,
        rec.receta_Duracion,
        rec.receta_FechaInicio,
        rec.receta_FechaFin,
        rec.receta_Estado,
        DATEDIFF(DAY, rec.receta_FechaInicio, ISNULL(rec.receta_FechaFin, GETDATE())) AS DuracionDias
    FROM [Medico].[tbRecetas] AS rec
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON rec.masc_Id = masc.masc_Id
    LEFT JOIN [Refugio].[tbRazas] AS raza
        ON masc.raza_Id = raza.raza_Id
    WHERE rec.receta_EsEliminado = 0
      AND rec.receta_Estado IN ('Activo', 'Activa')
      AND (rec.receta_FechaFin IS NULL OR rec.receta_FechaFin >= GETDATE())
      AND masc.masc_EsEliminado = 0
    ORDER BY rec.receta_FechaInicio DESC
END
GO

-- =======================================
-- SP: PR_Medico_AlertaMedica_SinConsulta
-- Mascotas sin consulta medica en los ultimos 6 meses
-- =======================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_AlertaMedica_SinConsulta')
    DROP PROCEDURE [Medico].[PR_Medico_AlertaMedica_SinConsulta]
GO

CREATE PROCEDURE [Medico].[PR_Medico_AlertaMedica_SinConsulta]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        masc.masc_Id,
        masc.masc_Nombre AS MascotaNombre,
        ISNULL(raza.raza_Descripcion, 'Sin raza') AS Raza,
        ISNULL(CAST(masc.masc_Edad AS NVARCHAR(10)), '?') AS Edad,
        ult.UltimaVisita,
        CASE
            WHEN ult.UltimaVisita IS NULL THEN 'Sin registro de consulta'
            WHEN DATEDIFF(MONTH, ult.UltimaVisita, GETDATE()) = 1 THEN 'Hace 1 mes'
            ELSE 'Hace ' + CAST(DATEDIFF(MONTH, ult.UltimaVisita, GETDATE()) AS NVARCHAR(10)) + ' meses'
        END AS TiempoSinConsulta
    FROM [Refugio].[tbMascotas] AS masc
    LEFT JOIN (
        SELECT masc_Id, MAX(cita_FechaConsulta) AS UltimaVisita
        FROM [Medico].[tbCitaMedica]
        WHERE cita_EsEliminado = 0
        GROUP BY masc_Id
    ) AS ult ON masc.masc_Id = ult.masc_Id
    LEFT JOIN [Refugio].[tbRazas] AS raza
        ON masc.raza_Id = raza.raza_Id
    WHERE masc.masc_EsEliminado = 0
      AND masc.masc_EsAdoptado = 0
      AND (ult.UltimaVisita IS NULL OR ult.UltimaVisita < DATEADD(MONTH, -6, GETDATE()))
    ORDER BY ult.UltimaVisita ASC
END
GO

PRINT '=========================================='
PRINT '✅ SPs ALERTAS MEDICAS CREADOS'
PRINT '=========================================='
PRINT 'PR_Medico_AlertaMedica_VacunasVencidas'
PRINT 'PR_Medico_AlertaMedica_TratamientosPorVencer'
PRINT 'PR_Medico_AlertaMedica_RecetasSinRevision'
PRINT 'PR_Medico_AlertaMedica_SinConsulta'
PRINT '=========================================='
GO
