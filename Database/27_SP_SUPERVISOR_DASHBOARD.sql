USE PETSHOMEDB
GO

-- ============================================================
-- PR_Supervisor_Dashboard_Pills
-- Zona 1: mascotas activas, solicitudes pendientes, stock bajo, eventos semana
-- ============================================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Supervisor_Dashboard_Pills]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        (SELECT COUNT(*) FROM [Refugio].[tbMascotas]  WHERE masc_EsEliminado=0)                                            AS pill_MascotasTotal,
        (SELECT COUNT(*) FROM [Refugio].[tbSolicitudes] WHERE sol_EsEliminado=0 AND ISNULL(sol_Estado,'Pendiente')='Pendiente') AS pill_SolicitudesPendientes,
        (SELECT COUNT(*) FROM [Inventario].[vw_StockActual] s
                INNER JOIN [Inventario].[tbItems] i ON i.itm_Id=s.itm_Id
                WHERE s.exi_Cantidad <= i.itm_StockMinimo AND i.itm_EsEliminado=0)                                         AS pill_StockBajo,
        (SELECT COUNT(*) FROM [Refugio].[tbEventos]  WHERE eve_EsEliminado=0
                AND eve_Fecha >= CAST(GETDATE() AS DATE)
                AND eve_Fecha <  DATEADD(DAY,7,CAST(GETDATE() AS DATE)))                                                   AS pill_EventosSemana
END
GO

-- ============================================================
-- PR_Supervisor_Dashboard_KPIs
-- Zona 2: mascotas activas, adopciones mes, vacunas vencidas, próximo evento
-- ============================================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Supervisor_Dashboard_KPIs]
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @mascotasActual INT = (SELECT COUNT(*) FROM [Refugio].[tbMascotas] WHERE masc_EsEliminado=0)
    DECLARE @mascotasMesAnt INT = (SELECT COUNT(*) FROM [Refugio].[tbMascotas]
        WHERE masc_EsEliminado=0
          AND ISNULL(masc_FechaCrea,'1900-01-01') < DATEADD(MONTH,-1,GETDATE()))

    DECLARE @adopcionesMes INT = (SELECT COUNT(*) FROM [Refugio].[tbSolicitudes]
        WHERE sol_EsEliminado=0 AND ISNULL(sol_Estado,'')='Aprobada'
          AND YEAR(sol_Fecha)=YEAR(GETDATE()) AND MONTH(sol_Fecha)=MONTH(GETDATE()))

    DECLARE @vacunasVencidas INT = (
        SELECT COUNT(*) FROM (
            SELECT c.masc_Id, c.vac_Id
            FROM [Refugio].[tbCitaMedica] c
            INNER JOIN [Refugio].[tbVacunas] v ON v.vac_Id=c.vac_Id
            WHERE c.vac_Id IS NOT NULL AND c.medic_EsEliminado=0
            GROUP BY c.masc_Id, c.vac_Id, v.vacu_PeriodoRefuerzo
            HAVING DATEADD(DAY,
                CASE WHEN v.vacu_PeriodoRefuerzo LIKE '%3 a%' THEN 1095
                     WHEN v.vacu_PeriodoRefuerzo LIKE '%2 a%' THEN 730
                     WHEN v.vacu_PeriodoRefuerzo LIKE '%6 m%' THEN 180
                     ELSE 365 END,
                MAX(c.medic_FechaConsulta)) < GETDATE()
        ) vv
    )

    SELECT
        @mascotasActual                                             AS kpi_MascotasActivas,
        @mascotasActual - @mascotasMesAnt                          AS kpi_MascotasTendencia,
        @adopcionesMes                                             AS kpi_AdopcionesMes,
        @vacunasVencidas                                           AS kpi_VacunasVencidas,
        (SELECT COUNT(*) FROM [Refugio].[tbEventos]
            WHERE eve_EsEliminado=0 AND eve_Fecha >= CAST(GETDATE() AS DATE))  AS kpi_EventosProximos,
        (SELECT TOP 1 eve_Descripcion FROM [Refugio].[tbEventos]
            WHERE eve_EsEliminado=0 AND eve_Fecha >= CAST(GETDATE() AS DATE)
            ORDER BY eve_Fecha ASC)                                AS kpi_ProximoEventoNombre,
        (SELECT TOP 1 eve_Fecha FROM [Refugio].[tbEventos]
            WHERE eve_EsEliminado=0 AND eve_Fecha >= CAST(GETDATE() AS DATE)
            ORDER BY eve_Fecha ASC)                                AS kpi_ProximoEventoFecha
END
GO

-- ============================================================
-- PR_Supervisor_Dashboard_Solicitudes
-- Zona 3 col 1: últimas 6 solicitudes
-- ============================================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Supervisor_Dashboard_Solicitudes]
AS
BEGIN
    SET NOCOUNT ON
    SELECT TOP 6
        s.sol_Id,
        s.sol_Nombres + ' ' + s.sol_Apellidos   AS sol_NombreCompleto,
        LEFT(s.sol_Nombres,1) + LEFT(s.sol_Apellidos,1) AS sol_Iniciales,
        s.sol_Correo,
        m.masc_Nombre,
        r.raza_TipoAnimal                        AS masc_Especie,
        r.raza_Descripcion                       AS masc_Raza,
        ISNULL(s.sol_Estado,'Pendiente')         AS sol_Estado,
        s.sol_Fecha
    FROM [Refugio].[tbSolicitudes] s
    INNER JOIN [Refugio].[tbMascotas] m ON m.masc_Id = s.masc_Id
    INNER JOIN [Refugio].[tbRazas]    r ON r.raza_Id = m.raza_Id
    WHERE s.sol_EsEliminado = 0
    ORDER BY s.sol_Fecha DESC
END
GO

-- ============================================================
-- PR_Supervisor_Dashboard_EstadoMascotas
-- Zona 3 col 2: donut de estado de mascotas
-- ============================================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Supervisor_Dashboard_EstadoMascotas]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        COUNT(*)                                                            AS est_Total,
        SUM(CASE WHEN masc_EsAdoptado=0 AND masc_EsReservado=0 THEN 1 ELSE 0 END) AS est_Disponibles,
        SUM(CASE WHEN masc_EsReservado=1 THEN 1 ELSE 0 END)                AS est_EnProceso,
        SUM(CASE WHEN masc_EsAdoptado=1 THEN 1 ELSE 0 END)                 AS est_Adoptadas
    FROM [Refugio].[tbMascotas]
    WHERE masc_EsEliminado = 0
END
GO

-- ============================================================
-- PR_Supervisor_Dashboard_Eventos
-- Zona 4 col 1: próximos 5 eventos
-- ============================================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Supervisor_Dashboard_Eventos]
AS
BEGIN
    SET NOCOUNT ON
    SELECT TOP 5
        e.eve_Id,
        e.eve_Descripcion,
        e.eve_Fecha,
        e.eve_HoraInicio,
        r.refg_Nombre                       AS eve_Lugar,
        (SELECT COUNT(*) FROM [Refugio].[tbEventos_tbVoluntarios] ev
            WHERE ev.eve_Id = e.eve_Id)     AS eve_CantidadVoluntarios
    FROM [Refugio].[tbEventos] e
    INNER JOIN [Refugio].[tbRefugios] r ON r.refg_Id = e.refg_Id
    WHERE e.eve_EsEliminado = 0
      AND e.eve_Fecha >= CAST(GETDATE() AS DATE)
    ORDER BY e.eve_Fecha ASC
END
GO

-- ============================================================
-- PR_Supervisor_Dashboard_MovimientosInventario
-- Zona 4 col 2: últimas 5 recepciones
-- ============================================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Supervisor_Dashboard_MovimientosInventario]
AS
BEGIN
    SET NOCOUNT ON
    SELECT TOP 5
        r.recep_Id,
        r.recep_Descripcion,
        r.recep_Fecha,
        r.recep_NumeroDocumento,
        (SELECT TOP 1 i.itm_Descripcion
            FROM [Inventario].[tbRecepcionesDetalles] rd
            INNER JOIN [Inventario].[tbItems] i ON i.itm_Id = rd.itm_Id
            WHERE rd.recep_Id = r.recep_Id AND rd.recdet_EsEliminado=0
            ORDER BY rd.recdet_Id ASC)          AS mov_PrimerItem,
        (SELECT SUM(rd2.recdet_Cantidad)
            FROM [Inventario].[tbRecepcionesDetalles] rd2
            WHERE rd2.recep_Id = r.recep_Id AND rd2.recdet_EsEliminado=0) AS mov_TotalUnidades,
        rf.refg_Nombre                          AS mov_Refugio
    FROM [Inventario].[tbRecepcionesMercancia] r
    INNER JOIN [Refugio].[tbRefugios] rf ON rf.refg_Id = r.refg_Id
    WHERE r.recep_EsEliminado = 0
    ORDER BY r.recep_Fecha DESC
END
GO

PRINT '=========================================='
PRINT 'SPs Dashboard Supervisor creados OK'
PRINT '  PR_Supervisor_Dashboard_Pills'
PRINT '  PR_Supervisor_Dashboard_KPIs'
PRINT '  PR_Supervisor_Dashboard_Solicitudes'
PRINT '  PR_Supervisor_Dashboard_EstadoMascotas'
PRINT '  PR_Supervisor_Dashboard_Eventos'
PRINT '  PR_Supervisor_Dashboard_MovimientosInventario'
PRINT '=========================================='
GO
