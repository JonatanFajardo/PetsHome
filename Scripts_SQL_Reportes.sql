-- =============================================
-- SCRIPT SQL PARA PROCEDIMIENTOS ALMACENADOS DE REPORTES
-- Sistema: PetsHome
-- Fecha: $(Fecha)
-- Descripción: Procedimientos para generar reportes del sistema
-- =============================================

-- Crear esquema de Reportes si no existe
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Reportes')
BEGIN
    EXEC('CREATE SCHEMA [Reportes]')
END
GO

-- =============================================
-- 1. PROCEDIMIENTO PARA DASHBOARD PRINCIPAL
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Reportes_Dashboard')
    DROP PROCEDURE [Reportes].[PR_Reportes_Dashboard]
GO

CREATE PROCEDURE [Reportes].[PR_Reportes_Dashboard]
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalMascotas INT = 0
    DECLARE @MascotasAdoptadas INT = 0
    DECLARE @MascotasDisponibles INT = 0
    DECLARE @CitasMedicasPendientes INT = 0
    DECLARE @VoluntariosActivos INT = 0
    DECLARE @EventosEsteMes INT = 0
    DECLARE @PorcentajeAdopciones DECIMAL(5,2) = 0
    
    -- Total de mascotas
    SELECT @TotalMascotas = COUNT(*) 
    FROM [Refugio].[tbMascotas] 
    WHERE masc_EsEliminado = 0
    
    -- Mascotas adoptadas
    SELECT @MascotasAdoptadas = COUNT(*) 
    FROM [Refugio].[tbMascotas] 
    WHERE masc_EsEliminado = 0 AND masc_EsAdoptado = 1
    
    -- Mascotas disponibles
    SET @MascotasDisponibles = @TotalMascotas - @MascotasAdoptadas
    
    -- Citas médicas pendientes (próximas 30 días)
    SELECT @CitasMedicasPendientes = COUNT(*) 
    FROM [Refugio].[tbCitaMedica] cm
    INNER JOIN [Refugio].[tbMascotas] m ON cm.masc_Id = m.masc_Id
    WHERE m.masc_EsEliminado = 0 
      AND cm.medic_ProximaCita >= GETDATE() 
      AND cm.medic_ProximaCita <= DATEADD(DAY, 30, GETDATE())
    
    -- Voluntarios activos (que han participado en eventos en los últimos 6 meses)
    SELECT @VoluntariosActivos = COUNT(DISTINCT v.vol_Id)
    FROM [Refugio].[tbVoluntarios] v
    INNER JOIN [Refugio].[tbEventosVoluntarios] ev ON v.vol_Id = ev.vol_Id
    INNER JOIN [Refugio].[tbEventos] e ON ev.eve_Id = e.eve_Id
    WHERE e.eve_Fecha >= DATEADD(MONTH, -6, GETDATE())
      AND e.eve_EsEliminado = 0
    
    -- Eventos del mes actual
    SELECT @EventosEsteMes = COUNT(*) 
    FROM [Refugio].[tbEventos] 
    WHERE eve_EsEliminado = 0 
      AND YEAR(eve_Fecha) = YEAR(GETDATE()) 
      AND MONTH(eve_Fecha) = MONTH(GETDATE())
    
    -- Calcular porcentaje de adopciones
    IF @TotalMascotas > 0
        SET @PorcentajeAdopciones = CAST(@MascotasAdoptadas AS DECIMAL(5,2)) / @TotalMascotas * 100
    
    -- Retornar resultados
    SELECT 
        @TotalMascotas AS TotalMascotas,
        @MascotasAdoptadas AS MascotasAdoptadas,
        @MascotasDisponibles AS MascotasDisponibles,
        @CitasMedicasPendientes AS CitasMedicasPendientes,
        @VoluntariosActivos AS VoluntariosActivos,
        @EventosEsteMes AS EventosEsteMes,
        @PorcentajeAdopciones AS PorcentajeAdopciones
END
GO

-- =============================================
-- 2. PROCEDIMIENTO PARA MASCOTAS POR RAZA
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Reportes_MascotasPorRaza')
    DROP PROCEDURE [Reportes].[PR_Reportes_MascotasPorRaza]
GO

CREATE PROCEDURE [Reportes].[PR_Reportes_MascotasPorRaza]
    @refg_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.raza_Id,
        ISNULL(r.raza_Descripcion, 'Sin raza') AS raza_Descripcion,
        COUNT(*) AS TotalMascotas,
        SUM(CASE WHEN m.masc_EsAdoptado = 1 THEN 1 ELSE 0 END) AS MascotasAdoptadas,
        SUM(CASE WHEN m.masc_EsAdoptado = 0 THEN 1 ELSE 0 END) AS MascotasDisponibles,
        CASE 
            WHEN COUNT(*) > 0 THEN 
                CAST(SUM(CASE WHEN m.masc_EsAdoptado = 1 THEN 1 ELSE 0 END) AS DECIMAL(5,2)) / COUNT(*) * 100 
            ELSE 0 
        END AS PorcentajeAdopcion
    FROM [Refugio].[tbMascotas] m
    LEFT JOIN [Refugio].[tbRazas] r ON m.raza_Id = r.raza_Id
    WHERE m.masc_EsEliminado = 0
      AND (@refg_Id IS NULL OR m.refg_Id = @refg_Id)
    GROUP BY r.raza_Id, r.raza_Descripcion
    ORDER BY TotalMascotas DESC
END
GO

-- =============================================
-- 3. PROCEDIMIENTO PARA ADOPCIONES POR MES
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Reportes_AdopcionesPorMes')
    DROP PROCEDURE [Reportes].[PR_Reportes_AdopcionesPorMes]
GO

CREATE PROCEDURE [Reportes].[PR_Reportes_AdopcionesPorMes]
    @mesesAtras INT = 6,
    @refg_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @FechaInicio DATE = DATEADD(MONTH, -@mesesAtras, GETDATE())
    
    SELECT 
        YEAR(a.adop_FechaCrea) AS Año,
        MONTH(a.adop_FechaCrea) AS Mes,
        DATENAME(MONTH, a.adop_FechaCrea) AS NombreMes,
        COUNT(*) AS TotalAdopciones,
        CONCAT(DATENAME(MONTH, a.adop_FechaCrea), ' ', YEAR(a.adop_FechaCrea)) AS Periodo
    FROM [Refugio].[tbAdopciones] a
    INNER JOIN [Refugio].[tbSolicitudes] s ON a.sol_Id = s.sol_Id
    INNER JOIN [Refugio].[tbMascotas] m ON s.masc_Id = m.masc_Id
    WHERE a.adop_EsEliminado = 0 
      AND a.adop_EsAprobado = 1
      AND a.adop_FechaCrea >= @FechaInicio
      AND (@refg_Id IS NULL OR m.refg_Id = @refg_Id)
    GROUP BY YEAR(a.adop_FechaCrea), MONTH(a.adop_FechaCrea), DATENAME(MONTH, a.adop_FechaCrea)
    ORDER BY Año, Mes
END
GO

-- =============================================
-- 4. PROCEDIMIENTO PARA CITAS MÉDICAS POR TIPO
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Reportes_CitasMedicasPorTipo')
    DROP PROCEDURE [Reportes].[PR_Reportes_CitasMedicasPorTipo]
GO

CREATE PROCEDURE [Reportes].[PR_Reportes_CitasMedicasPorTipo]
    @fechaInicio DATETIME = NULL,
    @fechaFin DATETIME = NULL,
    @refg_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Si no se especifican fechas, usar los últimos 3 meses
    IF @fechaInicio IS NULL
        SET @fechaInicio = DATEADD(MONTH, -3, GETDATE())
    
    IF @fechaFin IS NULL
        SET @fechaFin = GETDATE()
    
    DECLARE @TotalCitas INT
    
    -- Obtener total de citas para calcular porcentajes
    SELECT @TotalCitas = COUNT(*)
    FROM [Refugio].[tbCitaMedica] cm
    INNER JOIN [Refugio].[tbMascotas] m ON cm.masc_Id = m.masc_Id
    WHERE cm.medic_FechaConsulta >= @fechaInicio 
      AND cm.medic_FechaConsulta <= @fechaFin
      AND m.masc_EsEliminado = 0
      AND (@refg_Id IS NULL OR m.refg_Id = @refg_Id)
    
    SELECT 
        ISNULL(cm.medic_TipoConsulta, 'Sin tipo') AS medic_TipoConsulta,
        COUNT(*) AS TotalCitas,
        CASE 
            WHEN @TotalCitas > 0 THEN 
                CAST(COUNT(*) AS DECIMAL(5,2)) / @TotalCitas * 100 
            ELSE 0 
        END AS PorcentajeCitas
    FROM [Refugio].[tbCitaMedica] cm
    INNER JOIN [Refugio].[tbMascotas] m ON cm.masc_Id = m.masc_Id
    WHERE cm.medic_FechaConsulta >= @fechaInicio 
      AND cm.medic_FechaConsulta <= @fechaFin
      AND m.masc_EsEliminado = 0
      AND (@refg_Id IS NULL OR m.refg_Id = @refg_Id)
    GROUP BY cm.medic_TipoConsulta
    ORDER BY TotalCitas DESC
END
GO

-- =============================================
-- 5. PROCEDIMIENTO PARA REPORTE DE VOLUNTARIOS
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Reportes_Voluntarios')
    DROP PROCEDURE [Reportes].[PR_Reportes_Voluntarios]
GO

CREATE PROCEDURE [Reportes].[PR_Reportes_Voluntarios]
    @soloActivos BIT = 0,
    @refg_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.vol_Id,
        v.per_Id,
        CONCAT(p.per_PrimerNombre, ' ', ISNULL(p.per_SegundoNombre + ' ', ''), 
               p.per_ApellidoPaterno, ' ', ISNULL(p.per_ApellidoMaterno, '')) AS NombreCompleto,
        p.per_PrimerNombre,
        p.per_ApellidoPaterno,
        p.per_Telefono,
        p.per_Correo,
        COUNT(DISTINCT ev.eve_Id) AS EventosParticipados,
        ISNULL(v.vol_HorasTrabajadas, 0) AS vol_HorasTrabajadas,
        MAX(e.eve_Fecha) AS UltimaParticipacion,
        CASE 
            WHEN COUNT(DISTINCT ev.eve_Id) > 0 AND MAX(e.eve_Fecha) >= DATEADD(MONTH, -6, GETDATE()) 
            THEN 'Activo' 
            ELSE 'Inactivo' 
        END AS Estado,
        v.vol_Recurrente
    FROM [Refugio].[tbVoluntarios] v
    INNER JOIN [General].[tbPersonas] p ON v.per_Id = p.per_Id
    LEFT JOIN [Refugio].[tbEventosVoluntarios] ev ON v.vol_Id = ev.vol_Id
    LEFT JOIN [Refugio].[tbEventos] e ON ev.eve_Id = e.eve_Id AND e.eve_EsEliminado = 0
    WHERE p.per_EsEliminado = 0
      AND (@refg_Id IS NULL OR EXISTS (
          SELECT 1 FROM [Refugio].[tbEventos] et 
          WHERE et.eve_Id = e.eve_Id AND et.refg_Id = @refg_Id
      ))
    GROUP BY 
        v.vol_Id, v.per_Id, p.per_PrimerNombre, p.per_SegundoNombre, 
        p.per_ApellidoPaterno, p.per_ApellidoMaterno, p.per_Telefono, 
        p.per_Correo, v.vol_HorasTrabajadas, v.vol_Recurrente
    HAVING 
        (@soloActivos = 0 OR 
         (COUNT(DISTINCT ev.eve_Id) > 0 AND MAX(e.eve_Fecha) >= DATEADD(MONTH, -6, GETDATE())))
    ORDER BY EventosParticipados DESC, UltimaParticipacion DESC
END
GO

-- =============================================
-- 6. PROCEDIMIENTO PARA REPORTE DE INVENTARIO
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Reportes_Inventario')
    DROP PROCEDURE [Reportes].[PR_Reportes_Inventario]
GO

CREATE PROCEDURE [Reportes].[PR_Reportes_Inventario]
    @refg_Id INT = NULL,
    @soloCriticos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        i.itm_Id,
        i.itm_Codigo,
        i.itm_Descripcion,
        c.cat_Descripcion,
        ISNULL(
            (SELECT SUM(id.indt_Cantidad) 
             FROM [Inventario].[tbInventariosDetalles] id 
             INNER JOIN [Inventario].[tbInventarios] inv ON id.inv_Id = inv.inv_Id
             WHERE id.itm_Id = i.itm_Id 
               AND inv.inv_EsEliminado = 0
               AND (@refg_Id IS NULL OR inv.refg_Id = @refg_Id)
            ), 0
        ) AS StockActual,
        ISNULL(i.itm_StockMinimo, 10) AS StockMinimo,
        CASE 
            WHEN ISNULL(
                (SELECT SUM(id.indt_Cantidad) 
                 FROM [Inventario].[tbInventariosDetalles] id 
                 INNER JOIN [Inventario].[tbInventarios] inv ON id.inv_Id = inv.inv_Id
                 WHERE id.itm_Id = i.itm_Id 
                   AND inv.inv_EsEliminado = 0
                   AND (@refg_Id IS NULL OR inv.refg_Id = @refg_Id)
                ), 0
            ) <= ISNULL(i.itm_StockMinimo, 10) THEN 'Crítico'
            WHEN ISNULL(
                (SELECT SUM(id.indt_Cantidad) 
                 FROM [Inventario].[tbInventariosDetalles] id 
                 INNER JOIN [Inventario].[tbInventarios] inv ON id.inv_Id = inv.inv_Id
                 WHERE id.itm_Id = i.itm_Id 
                   AND inv.inv_EsEliminado = 0
                   AND (@refg_Id IS NULL OR inv.refg_Id = @refg_Id)
                ), 0
            ) <= ISNULL(i.itm_StockMinimo, 10) * 1.5 THEN 'Bajo'
            ELSE 'Normal'
        END AS EstadoStock,
        ISNULL(i.itm_Precio, 0) AS itm_Precio,
        ISNULL(
            (SELECT SUM(id.indt_Cantidad) 
             FROM [Inventario].[tbInventariosDetalles] id 
             INNER JOIN [Inventario].[tbInventarios] inv ON id.inv_Id = inv.inv_Id
             WHERE id.itm_Id = i.itm_Id 
               AND inv.inv_EsEliminado = 0
               AND (@refg_Id IS NULL OR inv.refg_Id = @refg_Id)
            ), 0
        ) * ISNULL(i.itm_Precio, 0) AS ValorTotal,
        CASE 
            WHEN @refg_Id IS NOT NULL THEN 
                (SELECT TOP 1 r.refg_Nombre FROM [Refugio].[tbRefugios] r WHERE r.refg_Id = @refg_Id)
            ELSE 'Todos los refugios'
        END AS refg_Nombre
    FROM [Inventario].[tbItems] i
    LEFT JOIN [Inventario].[tbCategorias] c ON i.cat_Id = c.cat_Id
    WHERE i.itm_EsEliminado = 0
      AND (@soloCriticos = 0 OR 
           ISNULL(
               (SELECT SUM(id.indt_Cantidad) 
                FROM [Inventario].[tbInventariosDetalles] id 
                INNER JOIN [Inventario].[tbInventarios] inv ON id.inv_Id = inv.inv_Id
                WHERE id.itm_Id = i.itm_Id 
                  AND inv.inv_EsEliminado = 0
                  AND (@refg_Id IS NULL OR inv.refg_Id = @refg_Id)
               ), 0
           ) <= ISNULL(i.itm_StockMinimo, 10)
          )
    ORDER BY 
        CASE 
            WHEN ISNULL(
                (SELECT SUM(id.indt_Cantidad) 
                 FROM [Inventario].[tbInventariosDetalles] id 
                 INNER JOIN [Inventario].[tbInventarios] inv ON id.inv_Id = inv.inv_Id
                 WHERE id.itm_Id = i.itm_Id 
                   AND inv.inv_EsEliminado = 0
                   AND (@refg_Id IS NULL OR inv.refg_Id = @refg_Id)
                ), 0
            ) <= ISNULL(i.itm_StockMinimo, 10) THEN 1
            WHEN ISNULL(
                (SELECT SUM(id.indt_Cantidad) 
                 FROM [Inventario].[tbInventariosDetalles] id 
                 INNER JOIN [Inventario].[tbInventarios] inv ON id.inv_Id = inv.inv_Id
                 WHERE id.itm_Id = i.itm_Id 
                   AND inv.inv_EsEliminado = 0
                   AND (@refg_Id IS NULL OR inv.refg_Id = @refg_Id)
                ), 0
            ) <= ISNULL(i.itm_StockMinimo, 10) * 1.5 THEN 2
            ELSE 3
        END,
        i.itm_Descripcion
END
GO

-- =============================================
-- 7. PROCEDIMIENTO PARA REPORTE DE EVENTOS
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Reportes_Eventos')
    DROP PROCEDURE [Reportes].[PR_Reportes_Eventos]
GO

CREATE PROCEDURE [Reportes].[PR_Reportes_Eventos]
    @fechaInicio DATETIME = NULL,
    @fechaFin DATETIME = NULL,
    @refg_Id INT = NULL,
    @soloFuturos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Si no se especifican fechas, usar los últimos 6 meses hacia adelante
    IF @fechaInicio IS NULL
        SET @fechaInicio = DATEADD(MONTH, -6, GETDATE())
    
    IF @fechaFin IS NULL
        SET @fechaFin = DATEADD(MONTH, 6, GETDATE())
    
    SELECT 
        e.eve_Id,
        e.eve_Descripcion,
        e.eve_Fecha,
        e.eve_HoraInicio,
        e.eve_HoraFinal,
        r.refg_Nombre,
        COUNT(DISTINCT ev.vol_Id) AS VoluntariosParticipantes,
        CASE 
            WHEN e.eve_Fecha > GETDATE() THEN 'Próximo'
            WHEN e.eve_Fecha = CAST(GETDATE() AS DATE) THEN 'En curso'
            ELSE 'Realizado'
        END AS Estado,
        e.eve_FechaCrea
    FROM [Refugio].[tbEventos] e
    INNER JOIN [Refugio].[tbRefugios] r ON e.refg_Id = r.refg_Id
    LEFT JOIN [Refugio].[tbEventosVoluntarios] ev ON e.eve_Id = ev.eve_Id
    WHERE e.eve_EsEliminado = 0
      AND e.eve_Fecha >= @fechaInicio 
      AND e.eve_Fecha <= @fechaFin
      AND (@refg_Id IS NULL OR e.refg_Id = @refg_Id)
      AND (@soloFuturos = 0 OR e.eve_Fecha >= GETDATE())
    GROUP BY 
        e.eve_Id, e.eve_Descripcion, e.eve_Fecha, e.eve_HoraInicio, 
        e.eve_HoraFinal, r.refg_Nombre, e.eve_FechaCrea
    ORDER BY e.eve_Fecha DESC
END
GO

-- =============================================
-- 8. PROCEDIMIENTO PARA REPORTE DE SALUD DE MASCOTAS
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Reportes_SaludMascotas')
    DROP PROCEDURE [Reportes].[PR_Reportes_SaludMascotas]
GO

CREATE PROCEDURE [Reportes].[PR_Reportes_SaludMascotas]
    @refg_Id INT = NULL,
    @soloProblematicas BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.masc_Id,
        m.masc_Nombre,
        ISNULL(r.raza_Descripcion, 'Sin raza') AS raza_Descripcion,
        ref.refg_Nombre,
        (SELECT TOP 1 cm.medic_FechaConsulta 
         FROM [Refugio].[tbCitaMedica] cm 
         WHERE cm.masc_Id = m.masc_Id 
         ORDER BY cm.medic_FechaConsulta DESC) AS UltimaCitaMedica,
        ISNULL(
            (SELECT TOP 1 cm.medic_Diagnostico 
             FROM [Refugio].[tbCitaMedica] cm 
             WHERE cm.masc_Id = m.masc_Id 
             ORDER BY cm.medic_FechaConsulta DESC), 
            'Sin información'
        ) AS EstadoSalud,
        (SELECT COUNT(*) 
         FROM [Refugio].[tbCitaMedica] cm 
         WHERE cm.masc_Id = m.masc_Id) AS TotalCitas,
        CASE 
            WHEN (SELECT TOP 1 cm.medic_FechaConsulta 
                  FROM [Refugio].[tbCitaMedica] cm 
                  WHERE cm.masc_Id = m.masc_Id 
                  ORDER BY cm.medic_FechaConsulta DESC) IS NOT NULL
            THEN DATEDIFF(DAY, 
                 (SELECT TOP 1 cm.medic_FechaConsulta 
                  FROM [Refugio].[tbCitaMedica] cm 
                  WHERE cm.masc_Id = m.masc_Id 
                  ORDER BY cm.medic_FechaConsulta DESC), 
                 GETDATE())
            ELSE NULL
        END AS DiasSinCita,
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM [Refugio].[tbCitaMedica] cm 
                WHERE cm.masc_Id = m.masc_Id 
                  AND cm.vac_Id IS NOT NULL 
                  AND cm.medic_FechaConsulta >= DATEADD(YEAR, -1, GETDATE())
            ) THEN 1 
            ELSE 0 
        END AS VacunasAlDia,
        CASE 
            WHEN (SELECT TOP 1 cm.medic_FechaConsulta 
                  FROM [Refugio].[tbCitaMedica] cm 
                  WHERE cm.masc_Id = m.masc_Id 
                  ORDER BY cm.medic_FechaConsulta DESC) IS NULL 
                 OR DATEDIFF(DAY, 
                    (SELECT TOP 1 cm.medic_FechaConsulta 
                     FROM [Refugio].[tbCitaMedica] cm 
                     WHERE cm.masc_Id = m.masc_Id 
                     ORDER BY cm.medic_FechaConsulta DESC), 
                    GETDATE()) > 180 
            THEN 'Alta'
            WHEN DATEDIFF(DAY, 
                 (SELECT TOP 1 cm.medic_FechaConsulta 
                  FROM [Refugio].[tbCitaMedica] cm 
                  WHERE cm.masc_Id = m.masc_Id 
                  ORDER BY cm.medic_FechaConsulta DESC), 
                 GETDATE()) > 90 
            THEN 'Media'
            ELSE 'Baja'
        END AS PrioridadAtencion,
        m.masc_Peso,
        m.masc_Edad
    FROM [Refugio].[tbMascotas] m
    LEFT JOIN [Refugio].[tbRazas] r ON m.raza_Id = r.raza_Id
    INNER JOIN [Refugio].[tbRefugios] ref ON m.refg_Id = ref.refg_Id
    WHERE m.masc_EsEliminado = 0
      AND (@refg_Id IS NULL OR m.refg_Id = @refg_Id)
      AND (@soloProblematicas = 0 OR 
           (SELECT TOP 1 cm.medic_FechaConsulta 
            FROM [Refugio].[tbCitaMedica] cm 
            WHERE cm.masc_Id = m.masc_Id 
            ORDER BY cm.medic_FechaConsulta DESC) IS NULL 
           OR DATEDIFF(DAY, 
              (SELECT TOP 1 cm.medic_FechaConsulta 
               FROM [Refugio].[tbCitaMedica] cm 
               WHERE cm.masc_Id = m.masc_Id 
               ORDER BY cm.medic_FechaConsulta DESC), 
              GETDATE()) > 90
          )
    ORDER BY 
        CASE 
            WHEN (SELECT TOP 1 cm.medic_FechaConsulta 
                  FROM [Refugio].[tbCitaMedica] cm 
                  WHERE cm.masc_Id = m.masc_Id 
                  ORDER BY cm.medic_FechaConsulta DESC) IS NULL THEN 1
            WHEN DATEDIFF(DAY, 
                 (SELECT TOP 1 cm.medic_FechaConsulta 
                  FROM [Refugio].[tbCitaMedica] cm 
                  WHERE cm.masc_Id = m.masc_Id 
                  ORDER BY cm.medic_FechaConsulta DESC), 
                 GETDATE()) > 180 THEN 2
            WHEN DATEDIFF(DAY, 
                 (SELECT TOP 1 cm.medic_FechaConsulta 
                  FROM [Refugio].[tbCitaMedica] cm 
                  WHERE cm.masc_Id = m.masc_Id 
                  ORDER BY cm.medic_FechaConsulta DESC), 
                 GETDATE()) > 90 THEN 3
            ELSE 4
        END,
        (SELECT TOP 1 cm.medic_FechaConsulta 
         FROM [Refugio].[tbCitaMedica] cm 
         WHERE cm.masc_Id = m.masc_Id 
         ORDER BY cm.medic_FechaConsulta DESC) DESC
END
GO

-- =============================================
-- SCRIPT COMPLETADO
-- =============================================
PRINT 'Procedimientos almacenados de reportes creados exitosamente.'
PRINT 'Total de procedimientos: 8'
PRINT '- PR_Reportes_Dashboard'
PRINT '- PR_Reportes_MascotasPorRaza'
PRINT '- PR_Reportes_AdopcionesPorMes'  
PRINT '- PR_Reportes_CitasMedicasPorTipo'
PRINT '- PR_Reportes_Voluntarios'
PRINT '- PR_Reportes_Inventario'
PRINT '- PR_Reportes_Eventos'
PRINT '- PR_Reportes_SaludMascotas'
GO