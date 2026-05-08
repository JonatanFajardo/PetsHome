USE PETSHOMEDB
GO

-- ============================================
-- 1. PR_Refugio_ReporteAdopciones_Resumen
-- ============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_ReporteAdopciones_Resumen]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(*)                                                        AS TotalAdopciones,
        SUM(CASE WHEN sol_Estado = 'Pendiente'  THEN 1 ELSE 0 END)     AS SolicitudesPendientes,
        CAST(
            CASE WHEN COUNT(*) = 0 THEN 0
                 ELSE SUM(CASE WHEN sol_Estado = 'Aprobada' THEN 1 ELSE 0 END) * 100 / COUNT(*)
            END
        AS INT)                                                         AS TasaAprobacion,
        ISNULL(
            AVG(DATEDIFF(DAY, sol_FechaCrea, sol_FechaModifica))
        , 0)                                                            AS TiempoPromedio
    FROM [Refugio].[tbSolicitudes]
    WHERE sol_EsEliminado = 0;
END
GO

-- ============================================
-- 2. PR_Refugio_ReporteAdopciones_AdopcionesPorMes
-- ============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_ReporteAdopciones_AdopcionesPorMes]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        FORMAT(sol_FechaCrea, 'yyyy-MM')    AS Mes,
        COUNT(*)                            AS Cantidad
    FROM [Refugio].[tbSolicitudes]
    WHERE sol_EsEliminado = 0
      AND sol_Estado       = 'Aprobada'
    GROUP BY FORMAT(sol_FechaCrea, 'yyyy-MM')
    ORDER BY Mes ASC;
END
GO

-- ============================================
-- 3. PR_Refugio_ReporteAdopciones_EstadoSolicitudes
-- ============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_ReporteAdopciones_EstadoSolicitudes]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ISNULL(sol_Estado, 'Pendiente')     AS Estado,
        COUNT(*)                            AS Cantidad
    FROM [Refugio].[tbSolicitudes]
    WHERE sol_EsEliminado = 0
    GROUP BY sol_Estado
    ORDER BY Cantidad DESC;
END
GO

-- ============================================
-- 4. PR_Refugio_ReporteAdopciones_TopRazas
-- ============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_ReporteAdopciones_TopRazas]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 10
        ISNULL(r.raza_Descripcion, 'Sin raza')  AS Raza,
        COUNT(*)                                 AS CantidadAdopciones
    FROM  [Refugio].[tbSolicitudes]  s
    INNER JOIN [Refugio].[tbMascotas] m ON s.masc_Id = m.masc_Id
    LEFT  JOIN [Refugio].[tbRazas]    r ON m.raza_Id  = r.raza_Id
    WHERE s.sol_EsEliminado = 0
      AND s.sol_Estado       = 'Aprobada'
    GROUP BY r.raza_Descripcion
    ORDER BY CantidadAdopciones DESC;
END
GO

-- ============================================
-- 5. PR_Refugio_ReporteAdopciones_AdopcionesRecientes
-- ============================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_ReporteAdopciones_AdopcionesRecientes]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 10
        m.masc_Nombre                                   AS MascotaNombre,
        ISNULL(r.raza_Descripcion, 'Sin raza')          AS Raza,
        s.sol_Nombres                                   AS Adoptante,
        s.sol_FechaModifica                             AS FechaAdopcion,
        s.sol_Estado                                    AS Estado,
        DATEDIFF(DAY, s.sol_FechaCrea, GETDATE())       AS DiasTranscurridos
    FROM  [Refugio].[tbSolicitudes]  s
    INNER JOIN [Refugio].[tbMascotas] m ON s.masc_Id = m.masc_Id
    LEFT  JOIN [Refugio].[tbRazas]    r ON m.raza_Id  = r.raza_Id
    WHERE s.sol_EsEliminado = 0
    ORDER BY s.sol_FechaCrea DESC;
END
GO

PRINT '=========================================='
PRINT 'SPs ReporteAdopciones CREADOS OK'
PRINT '=========================================='