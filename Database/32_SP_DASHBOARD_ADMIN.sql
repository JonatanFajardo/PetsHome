-- =============================================================================
-- Dashboard Admin — Stored Procedures (v2 corregido)
-- Schema: General (lectura transversal de todos los schemas)
-- Ejecutar en PETSHOMEDB
-- =============================================================================

-- ==========================================================
-- 1. KPIs Principales (devuelve 1 fila con todos los conteos)
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_KPIs]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Hoy     DATE = CAST(GETDATE() AS DATE);
    DECLARE @MesAct  INT  = MONTH(GETDATE());
    DECLARE @AnioAct INT  = YEAR(GETDATE());
    DECLARE @MesAnt  INT  = MONTH(DATEADD(MONTH, -1, GETDATE()));
    DECLARE @AnioAnt INT  = YEAR(DATEADD(MONTH, -1, GETDATE()));

    SELECT
        -- Mascotas
        (SELECT COUNT(*) FROM Refugio.tbMascotas
            WHERE masc_EsEliminado = 0)                                             AS TotalMascotas,
        (SELECT COUNT(*) FROM Refugio.tbMascotas
            WHERE masc_EsEliminado = 0
              AND MONTH(masc_FechaCrea) = @MesAct AND YEAR(masc_FechaCrea) = @AnioAct) AS MascotasMesActual,
        (SELECT COUNT(*) FROM Refugio.tbMascotas
            WHERE masc_EsEliminado = 0
              AND MONTH(masc_FechaCrea) = @MesAnt AND YEAR(masc_FechaCrea) = @AnioAnt) AS MascotasMesAnterior,
        -- Adopciones pendientes (via solicitudes pendientes sin adopcion aprobada)
        (SELECT COUNT(*) FROM Refugio.tbAdopciones
            WHERE adop_EsEliminado = 0 AND adop_Estado = 'Pendiente')               AS AdopcionesPendientes,
        (SELECT COUNT(*) FROM Refugio.tbAdopciones
            WHERE adop_EsEliminado = 0 AND adop_Estado = 'Pendiente'
              AND MONTH(adop_FechaCrea) = @MesAnt AND YEAR(adop_FechaCrea) = @AnioAnt) AS AdopcionesPendientesAnterior,
        -- Citas hoy (historial medico registrado hoy)
        (SELECT COUNT(*) FROM Medico.tbCitaMedica
            WHERE cita_EsEliminado = 0
              AND CAST(cita_FechaConsulta AS DATE) = @Hoy)                          AS CitasHoy,
        -- Alertas: mascotas con proxima cita en los proximos 7 dias
        (SELECT COUNT(*) FROM Medico.tbCitaMedica
            WHERE cita_EsEliminado = 0
              AND cita_ProximaCita IS NOT NULL
              AND CAST(cita_ProximaCita AS DATE) BETWEEN @Hoy AND DATEADD(DAY, 7, @Hoy)) AS AlertasActivas,
        -- Donaciones del mes
        ISNULL((SELECT SUM(dona_MontoMonetario) FROM Refugio.tbDonaciones
            WHERE dona_EsEliminado = 0
              AND MONTH(dona_FechaCrea) = @MesAct AND YEAR(dona_FechaCrea) = @AnioAct), 0) AS DonacionesMesActual,
        ISNULL((SELECT SUM(dona_MontoMonetario) FROM Refugio.tbDonaciones
            WHERE dona_EsEliminado = 0
              AND MONTH(dona_FechaCrea) = @MesAnt AND YEAR(dona_FechaCrea) = @AnioAnt), 0) AS DonacionesMesAnterior;
END
GO

-- ==========================================================
-- 2. Tendencias 6 Meses (area chart)
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_Tendencias]
AS
BEGIN
    SET NOCOUNT ON;

    WITH Meses AS (
        SELECT
            MONTH(DATEADD(MONTH, -n, GETDATE()))  AS NumMes,
            YEAR(DATEADD(MONTH,  -n, GETDATE()))  AS NumAnio,
            FORMAT(DATEADD(MONTH, -n, GETDATE()), 'MMM yyyy', 'es-HN') AS EtiquetaMes,
            n AS Orden
        FROM (VALUES(5),(4),(3),(2),(1),(0)) v(n)
    )
    SELECT
        m.EtiquetaMes,
        m.NumMes,
        m.NumAnio,
        ISNULL((
            SELECT COUNT(*) FROM Refugio.tbMascotas
            WHERE masc_EsEliminado = 0
              AND MONTH(masc_FechaCrea) = m.NumMes AND YEAR(masc_FechaCrea) = m.NumAnio
        ), 0) AS Ingresos,
        ISNULL((
            SELECT COUNT(*) FROM Refugio.tbAdopciones
            WHERE adop_EsEliminado = 0 AND adop_Estado = 'Aprobado'
              AND MONTH(adop_FechaCrea) = m.NumMes AND YEAR(adop_FechaCrea) = m.NumAnio
        ), 0) AS Adopciones,
        ISNULL((
            SELECT ISNULL(SUM(dona_MontoMonetario), 0) FROM Refugio.tbDonaciones
            WHERE dona_EsEliminado = 0
              AND MONTH(dona_FechaCrea) = m.NumMes AND YEAR(dona_FechaCrea) = m.NumAnio
        ), 0) AS Donaciones
    FROM Meses m
    ORDER BY m.Orden DESC;
END
GO

-- ==========================================================
-- 3. Mascotas por Estado (donut chart)
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_MascotasEstado]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 'Disponibles' AS Estado, COUNT(*) AS Cantidad, 1 AS Orden
    FROM Refugio.tbMascotas
    WHERE masc_EsEliminado = 0 AND masc_EsAdoptado = 0 AND masc_EsReservado = 0
    UNION ALL
    SELECT 'Reservadas', COUNT(*), 2
    FROM Refugio.tbMascotas
    WHERE masc_EsEliminado = 0 AND masc_EsReservado = 1
    UNION ALL
    SELECT 'Adoptadas', COUNT(*), 3
    FROM Refugio.tbMascotas
    WHERE masc_EsEliminado = 0 AND masc_EsAdoptado = 1
    ORDER BY Orden;
END
GO

-- ==========================================================
-- 4. Top Razas Adoptadas (horizontal bar)
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_TopRazas]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 7
        ISNULL(r.raza_Descripcion, 'Sin raza') AS raza_Descripcion,
        COUNT(*) AS Total
    FROM Refugio.tbMascotas m
    LEFT JOIN Refugio.tbRazas r ON r.raza_Id = m.raza_Id
    WHERE m.masc_EsEliminado = 0
      AND m.masc_EsAdoptado  = 1
    GROUP BY r.raza_Descripcion
    ORDER BY Total DESC;
END
GO

-- ==========================================================
-- 5. Citas de Hoy — vista global admin
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_CitasHoy]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 20
        m.masc_Nombre,
        ISNULL(tc.tipoCon_Descripcion, 'Consulta general') AS cita_TipoConsulta,
        c.cita_FechaConsulta,
        'Atendida' AS cita_Estado
    FROM   Medico.tbCitaMedica   c
    JOIN   Refugio.tbMascotas    m  ON m.masc_Id = c.masc_Id
    LEFT JOIN Medico.tbTiposConsulta tc ON tc.tipoCon_Id = c.tipoCon_Id
    WHERE  c.cita_EsEliminado = 0
      AND  CAST(c.cita_FechaConsulta AS DATE) = CAST(GETDATE() AS DATE)
    ORDER BY c.cita_FechaConsulta;
END
GO

-- ==========================================================
-- 6. Solicitudes Pendientes con Antiguedad
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_SolicitudesPendientes]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 15
        sol.sol_Nombres + ' ' + sol.sol_Apellidos AS sol_NombreCompleto,
        sol.sol_Estado,
        sol.sol_Fecha,
        m.masc_Nombre,
        DATEDIFF(DAY, sol.sol_Fecha, GETDATE()) AS DiasAntiguedad
    FROM   Refugio.tbSolicitudes sol
    JOIN   Refugio.tbMascotas    m   ON m.masc_Id = sol.masc_Id
    WHERE  sol.sol_EsEliminado = 0
      AND  sol.sol_Estado      = 'Pendiente'
    ORDER BY sol.sol_Fecha ASC;
END
GO

-- ==========================================================
-- 7. Usuarios por Rol
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_UsuariosPorRol]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ISNULL(r.Rol_Descripcion, 'Sin rol') AS Rol,
        COUNT(*)                              AS Cantidad,
        r.Rol_Id                              AS RolId
    FROM Seguridad.tbUsuarios u
    JOIN Seguridad.tbRoles    r ON r.Rol_Id = u.Rol_Id
    WHERE u.Usu_EsEliminado = 0
    GROUP BY r.Rol_Descripcion, r.Rol_Id
    ORDER BY r.Rol_Id;
END
GO

-- ==========================================================
-- 8. Heatmap de Citas (dia x hora, ultimas 4 semanas)
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_HeatmapCitas]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        DATEPART(WEEKDAY, cita_FechaConsulta) AS DiaSemana,
        DATEPART(HOUR,    cita_FechaConsulta) AS Hora,
        COUNT(*)                              AS Cantidad
    FROM Medico.tbCitaMedica
    WHERE cita_EsEliminado = 0
      AND cita_FechaConsulta >= DATEADD(WEEK, -4, GETDATE())
    GROUP BY DATEPART(WEEKDAY, cita_FechaConsulta), DATEPART(HOUR, cita_FechaConsulta)
    ORDER BY DiaSemana, Hora;
END
GO

-- ==========================================================
-- 9. Alertas de Inventario (stock bajo / agotado)
--    Stock actual en tbExistencias.exi_Cantidad (SUM por itm_Id)
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_InventarioAlertas]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 8
        i.itm_Descripcion                                      AS Descripcion,
        ISNULL(c.cat_Descripcion, '—')                         AS Categoria,
        ISNULL(e.StockActual, 0)                               AS StockActual,
        ISNULL(i.itm_StockMinimo, 0)                           AS StockMinimo,
        CASE
            WHEN ISNULL(e.StockActual, 0) = 0 THEN 'Agotado'
            ELSE 'Bajo'
        END                                                    AS Estado
    FROM Inventario.tbItems i
    LEFT JOIN Inventario.tbCategorias c ON c.cat_Id = i.cat_Id
    LEFT JOIN (
        SELECT itm_Id, SUM(exi_Cantidad) AS StockActual
        FROM Inventario.tbExistencias
        GROUP BY itm_Id
    ) e ON e.itm_Id = i.itm_Id
    WHERE i.itm_EsEliminado = 0
      AND ISNULL(e.StockActual, 0) <= ISNULL(i.itm_StockMinimo, 0)
    ORDER BY ISNULL(e.StockActual, 0) ASC;
END
GO

-- ==========================================================
-- 10. Embudo de Adopcion
-- ==========================================================
CREATE OR ALTER PROCEDURE [General].[PR_General_DashboardAdmin_EmbudoAdopcion]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Etapa, Cantidad, Orden
    FROM (
        SELECT 'Total solicitudes' AS Etapa, COUNT(*) AS Cantidad, 1 AS Orden
        FROM Refugio.tbSolicitudes WHERE sol_EsEliminado = 0
        UNION ALL
        SELECT 'Pendientes', COUNT(*), 2
        FROM Refugio.tbAdopciones WHERE adop_EsEliminado = 0 AND adop_Estado = 'Pendiente'
        UNION ALL
        SELECT 'Aprobadas', COUNT(*), 3
        FROM Refugio.tbAdopciones WHERE adop_EsEliminado = 0 AND adop_Estado = 'Aprobado'
        UNION ALL
        SELECT 'Rechazadas', COUNT(*), 4
        FROM Refugio.tbAdopciones WHERE adop_EsEliminado = 0 AND adop_Estado = 'Rechazado'
    ) t
    ORDER BY Orden;
END
GO
