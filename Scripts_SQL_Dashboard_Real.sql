-- =============================================
-- Scripts SQL para Dashboard de Reportes - Datos Reales
-- Proyecto: PetsHome - Sistema de Gestión de Refugios
-- =============================================

-- Crear esquema de Reportes si no existe
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Reportes')
BEGIN
    EXEC('CREATE SCHEMA [Reportes]')
END
GO

-- =============================================
-- Procedimiento: PR_Reportes_Dashboard
-- Descripción: Obtiene métricas principales del dashboard
-- =============================================
CREATE OR ALTER PROCEDURE [Reportes].[PR_Reportes_Dashboard]
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalMascotas INT = 0;
    DECLARE @MascotasAdoptadas INT = 0;
    DECLARE @MascotasDisponibles INT = 0;
    DECLARE @CitasMedicasPendientes INT = 0;
    DECLARE @VoluntariosActivos INT = 0;
    DECLARE @EventosEsteMes INT = 0;
    DECLARE @PorcentajeAdopciones DECIMAL(5,2) = 0;
    
    -- Total de mascotas (no eliminadas)
    SELECT @TotalMascotas = COUNT(*)
    FROM [Refugio].[tbMascotas]
    WHERE masc_EsEliminado = 0;
    
    -- Mascotas adoptadas
    SELECT @MascotasAdoptadas = COUNT(*)
    FROM [Refugio].[tbMascotas]
    WHERE masc_EsEliminado = 0 AND masc_EsAdoptado = 1;
    
    -- Mascotas disponibles para adopción
    SELECT @MascotasDisponibles = COUNT(*)
    FROM [Refugio].[tbMascotas]
    WHERE masc_EsEliminado = 0 AND masc_EsAdoptado = 0 AND masc_EsReservado = 0;
    
    -- Citas médicas pendientes (próximas citas programadas)
    SELECT @CitasMedicasPendientes = COUNT(*)
    FROM [Refugio].[tbCitaMedica]
    WHERE medic_EsEliminado = 0 
      AND medic_ProximaCita IS NOT NULL 
      AND medic_ProximaCita >= CAST(GETDATE() AS DATE);
    
    -- Voluntarios activos (que han participado en eventos en los últimos 6 meses)
    SELECT @VoluntariosActivos = COUNT(DISTINCT v.vol_Id)
    FROM [Refugio].[tbVoluntarios] v
    INNER JOIN [Refugio].[tbEventos_tbVoluntarios] ev ON v.vol_Id = ev.vol_Id
    INNER JOIN [Refugio].[tbEventos] e ON ev.eve_Id = e.eve_Id
    WHERE e.eve_EsEliminado = 0 
      AND e.eve_Fecha >= DATEADD(MONTH, -6, GETDATE());
    
    -- Eventos en el mes actual
    SELECT @EventosEsteMes = COUNT(*)
    FROM [Refugio].[tbEventos]
    WHERE eve_EsEliminado = 0
      AND YEAR(eve_Fecha) = YEAR(GETDATE())
      AND MONTH(eve_Fecha) = MONTH(GETDATE());
    
    -- Calcular porcentaje de adopciones
    IF @TotalMascotas > 0
    BEGIN
        SET @PorcentajeAdopciones = CAST(@MascotasAdoptadas AS DECIMAL(5,2)) / CAST(@TotalMascotas AS DECIMAL(5,2)) * 100;
    END
    
    -- Devolver resultados
    SELECT 
        @TotalMascotas AS TotalMascotas,
        @MascotasAdoptadas AS MascotasAdoptadas,
        @MascotasDisponibles AS MascotasDisponibles,
        @CitasMedicasPendientes AS CitasMedicasPendientes,
        @VoluntariosActivos AS VoluntariosActivos,
        @EventosEsteMes AS EventosEsteMes,
        @PorcentajeAdopciones AS PorcentajeAdopciones;
END
GO

-- =============================================
-- Procedimiento: PR_Reportes_MascotasPorRaza
-- Descripción: Obtiene estadísticas de mascotas agrupadas por raza
-- =============================================
CREATE OR ALTER PROCEDURE [Reportes].[PR_Reportes_MascotasPorRaza]
    @refg_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.raza_Descripcion,
        COUNT(m.masc_Id) AS TotalMascotas,
        SUM(CASE WHEN m.masc_EsAdoptado = 1 THEN 1 ELSE 0 END) AS MascotasAdoptadas,
        SUM(CASE WHEN m.masc_EsAdoptado = 0 AND m.masc_EsReservado = 0 THEN 1 ELSE 0 END) AS MascotasDisponibles
    FROM [Refugio].[tbRazas] r
    LEFT JOIN [Refugio].[tbMascotas] m ON r.raza_Id = m.raza_Id AND m.masc_EsEliminado = 0
    WHERE (@refg_Id IS NULL OR m.refg_Id = @refg_Id)
    GROUP BY r.raza_Descripcion
    HAVING COUNT(m.masc_Id) > 0
    ORDER BY COUNT(m.masc_Id) DESC;
END
GO

-- =============================================
-- Procedimiento: PR_Reportes_AdopcionesPorMes
-- Descripción: Obtiene estadísticas de adopciones por mes
-- =============================================
CREATE OR ALTER PROCEDURE [Reportes].[PR_Reportes_AdopcionesPorMes]
    @mesesAtras INT = 6,
    @refg_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Crear tabla temporal con los meses
    DECLARE @fechaInicio DATE = DATEADD(MONTH, -@mesesAtras, GETDATE());
    
    WITH MesesCTE AS (
        SELECT 
            YEAR(@fechaInicio) AS Año,
            MONTH(@fechaInicio) AS NumeroMes,
            DATENAME(MONTH, @fechaInicio) AS NombreMes,
            @fechaInicio AS FechaMes
        UNION ALL
        SELECT 
            YEAR(DATEADD(MONTH, 1, FechaMes)) AS Año,
            MONTH(DATEADD(MONTH, 1, FechaMes)) AS NumeroMes,
            DATENAME(MONTH, DATEADD(MONTH, 1, FechaMes)) AS NombreMes,
            DATEADD(MONTH, 1, FechaMes) AS FechaMes
        FROM MesesCTE
        WHERE DATEADD(MONTH, 1, FechaMes) <= GETDATE()
    )
    SELECT 
        m.Año,
        m.NombreMes,
        ISNULL(COUNT(a.adop_Id), 0) AS TotalAdopciones
    FROM MesesCTE m
    LEFT JOIN [Refugio].[tbAdopciones] a ON 
        YEAR(a.adop_FechaCrea) = m.Año AND 
        MONTH(a.adop_FechaCrea) = m.NumeroMes AND
        a.adop_EsEliminado = 0 AND
        a.adop_EsAprobado = 1 AND
        (@refg_Id IS NULL OR EXISTS (
            SELECT 1 FROM [Refugio].[tbSolicitudes] s 
            INNER JOIN [Refugio].[tbMascotas] ma ON s.masc_Id = ma.masc_Id 
            WHERE s.sol_Id = a.sol_Id AND ma.refg_Id = @refg_Id
        ))
    GROUP BY m.Año, m.NumeroMes, m.NombreMes, m.FechaMes
    ORDER BY m.FechaMes;
END
GO

-- =============================================
-- Procedimiento: PR_Reportes_CitasMedicasPorTipo
-- Descripción: Obtiene estadísticas de citas médicas por tipo
-- =============================================
CREATE OR ALTER PROCEDURE [Reportes].[PR_Reportes_CitasMedicasPorTipo]
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL,
    @refg_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Establecer fechas por defecto si no se proporcionan
    IF @fechaInicio IS NULL SET @fechaInicio = DATEADD(MONTH, -3, GETDATE());
    IF @fechaFin IS NULL SET @fechaFin = GETDATE();
    
    DECLARE @TotalCitas INT;
    
    -- Obtener total de citas para calcular porcentajes
    SELECT @TotalCitas = COUNT(*)
    FROM [Refugio].[tbCitaMedica] c
    INNER JOIN [Refugio].[tbMascotas] m ON c.masc_Id = m.masc_Id
    WHERE c.medic_EsEliminado = 0
      AND c.medic_FechaConsulta BETWEEN @fechaInicio AND @fechaFin
      AND (@refg_Id IS NULL OR m.refg_Id = @refg_Id);
    
    -- Obtener estadísticas por tipo
    SELECT 
        c.medic_TipoConsulta,
        COUNT(*) AS TotalCitas,
        CASE 
            WHEN @TotalCitas > 0 THEN CAST(COUNT(*) AS DECIMAL(5,2)) / CAST(@TotalCitas AS DECIMAL(5,2)) * 100
            ELSE 0 
        END AS PorcentajeCitas
    FROM [Refugio].[tbCitaMedica] c
    INNER JOIN [Refugio].[tbMascotas] m ON c.masc_Id = m.masc_Id
    WHERE c.medic_EsEliminado = 0
      AND c.medic_FechaConsulta BETWEEN @fechaInicio AND @fechaFin
      AND (@refg_Id IS NULL OR m.refg_Id = @refg_Id)
      AND c.medic_TipoConsulta IS NOT NULL
    GROUP BY c.medic_TipoConsulta
    ORDER BY COUNT(*) DESC;
END
GO

-- =============================================
-- Procedimiento: PR_Reportes_Voluntarios
-- Descripción: Obtiene reporte de voluntarios con su participación
-- =============================================
CREATE OR ALTER PROCEDURE [Reportes].[PR_Reportes_Voluntarios]
    @soloActivos BIT = 0,
    @refg_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.vol_Id,
        CONCAT(p.per_PrimerNombre, ' ', ISNULL(p.per_SegundoNombre, ''), ' ', p.per_PrimerApellido, ' ', ISNULL(p.per_SegundoApellido, '')) AS NombreCompleto,
        p.per_Telefono,
        p.per_Correo,
        COUNT(DISTINCT ev.eve_Id) AS EventosParticipados,
        MAX(e.eve_Fecha) AS UltimaParticipacion,
        CASE 
            WHEN MAX(e.eve_Fecha) >= DATEADD(MONTH, -3, GETDATE()) THEN 'Activo'
            ELSE 'Inactivo'
        END AS Estado
    FROM [Refugio].[tbVoluntarios] v
    INNER JOIN [General].[tbPersonas] p ON v.per_Id = p.per_Id
    LEFT JOIN [Refugio].[tbEventos_tbVoluntarios] ev ON v.vol_Id = ev.vol_Id
    LEFT JOIN [Refugio].[tbEventos] e ON ev.eve_Id = e.eve_Id AND e.eve_EsEliminado = 0
    WHERE (@refg_Id IS NULL OR e.refg_Id = @refg_Id OR e.refg_Id IS NULL)
    GROUP BY v.vol_Id, p.per_PrimerNombre, p.per_SegundoNombre, p.per_PrimerApellido, p.per_SegundoApellido, p.per_Telefono, p.per_Correo
    HAVING (@soloActivos = 0 OR MAX(e.eve_Fecha) >= DATEADD(MONTH, -3, GETDATE()))
    ORDER BY COUNT(DISTINCT ev.eve_Id) DESC;
END
GO

-- =============================================
-- Procedimiento: PR_Reportes_Inventario
-- Descripción: Obtiene reporte de inventario con stock crítico
-- =============================================
CREATE OR ALTER PROCEDURE [Reportes].[PR_Reportes_Inventario]
    @refg_Id INT = NULL,
    @soloCriticos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        i.itm_Id,
        i.itm_Descripcion,
        c.cat_Descripcion,
        ISNULL(inv.inv_Stock, 0) AS StockActual,
        i.itm_StockMinimo AS StockMinimo,
        CASE 
            WHEN ISNULL(inv.inv_Stock, 0) = 0 THEN 'Sin Stock'
            WHEN ISNULL(inv.inv_Stock, 0) <= (i.itm_StockMinimo * 0.5) THEN 'Crítico'
            WHEN ISNULL(inv.inv_Stock, 0) <= i.itm_StockMinimo THEN 'Bajo'
            ELSE 'Normal'
        END AS EstadoStock,
        i.itm_Precio AS CostoUnitario,
        (ISNULL(inv.inv_Stock, 0) * i.itm_Precio) AS ValorTotal
    FROM [Inventario].[tbItems] i
    INNER JOIN [Inventario].[tbCategorias] c ON i.cat_Id = c.cat_Id
    LEFT JOIN [Inventario].[tbInventarios] inv ON i.itm_Id = inv.itm_Id AND (@refg_Id IS NULL OR inv.refg_Id = @refg_Id)
    WHERE i.itm_EsEliminado = 0
      AND c.cat_EsEliminado = 0
      AND (@soloCriticos = 0 OR 
           (ISNULL(inv.inv_Stock, 0) <= i.itm_StockMinimo))
    ORDER BY 
        CASE 
            WHEN ISNULL(inv.inv_Stock, 0) = 0 THEN 1
            WHEN ISNULL(inv.inv_Stock, 0) <= (i.itm_StockMinimo * 0.5) THEN 2
            WHEN ISNULL(inv.inv_Stock, 0) <= i.itm_StockMinimo THEN 3
            ELSE 4
        END,
        i.itm_Descripcion;
END
GO

-- =============================================
-- Procedimiento: PR_Reportes_Eventos
-- Descripción: Obtiene reporte de eventos del refugio
-- =============================================
CREATE OR ALTER PROCEDURE [Reportes].[PR_Reportes_Eventos]
    @fechaInicio DATE = NULL,
    @fechaFin DATE = NULL,
    @refg_Id INT = NULL,
    @soloFuturos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Establecer fechas por defecto
    IF @fechaInicio IS NULL SET @fechaInicio = DATEADD(MONTH, -3, GETDATE());
    IF @fechaFin IS NULL SET @fechaFin = DATEADD(MONTH, 3, GETDATE());
    
    SELECT 
        e.eve_Id,
        e.eve_Descripcion,
        e.eve_Fecha,
        COUNT(DISTINCT ev.vol_Id) AS VoluntariosParticipantes,
        CASE 
            WHEN e.eve_Fecha > GETDATE() THEN 'Próximo'
            WHEN e.eve_Fecha = CAST(GETDATE() AS DATE) THEN 'Hoy'
            ELSE 'Realizado'
        END AS Estado,
        e.eve_Descripcion AS Descripcion
    FROM [Refugio].[tbEventos] e
    LEFT JOIN [Refugio].[tbEventos_tbVoluntarios] ev ON e.eve_Id = ev.eve_Id
    WHERE e.eve_EsEliminado = 0
      AND e.eve_Fecha BETWEEN @fechaInicio AND @fechaFin
      AND (@refg_Id IS NULL OR e.refg_Id = @refg_Id)
      AND (@soloFuturos = 0 OR e.eve_Fecha >= CAST(GETDATE() AS DATE))
    GROUP BY e.eve_Id, e.eve_Descripcion, e.eve_Fecha
    ORDER BY e.eve_Fecha DESC;
END
GO

-- =============================================
-- Procedimiento: PR_Reportes_SaludMascotas
-- Descripción: Obtiene reporte de salud de mascotas
-- =============================================
CREATE OR ALTER PROCEDURE [Reportes].[PR_Reportes_SaludMascotas]
    @refg_Id INT = NULL,
    @soloProblematicas BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.masc_Id,
        m.masc_Nombre,
        r.raza_Descripcion,
        MAX(c.medic_FechaConsulta) AS UltimaCitaMedica,
        CASE 
            WHEN MAX(c.medic_FechaConsulta) IS NULL THEN 'Sin Historial'
            WHEN MAX(c.medic_FechaConsulta) < DATEADD(MONTH, -6, GETDATE()) THEN 'Requiere Revisión'
            WHEN COUNT(CASE WHEN c.medic_ProximaCita IS NOT NULL AND c.medic_ProximaCita < GETDATE() THEN 1 END) > 0 THEN 'Cita Pendiente'
            ELSE 'Al Día'
        END AS EstadoSalud,
        COUNT(c.medic_Id) AS TotalCitas,
        CASE 
            WHEN COUNT(hv.vac_Id) > 0 THEN 1
            ELSE 0
        END AS VacunasAlDia,
        ref.refg_Nombre
    FROM [Refugio].[tbMascotas] m
    INNER JOIN [Refugio].[tbRefugios] ref ON m.refg_Id = ref.refg_Id
    LEFT JOIN [Refugio].[tbRazas] r ON m.raza_Id = r.raza_Id
    LEFT JOIN [Refugio].[tbCitaMedica] c ON m.masc_Id = c.masc_Id AND c.medic_EsEliminado = 0
    LEFT JOIN [Refugio].[tbHistorialMedico_tbVacunas] hv ON c.medic_Id = hv.medic_Id
    WHERE m.masc_EsEliminado = 0
      AND (@refg_Id IS NULL OR m.refg_Id = @refg_Id)
    GROUP BY m.masc_Id, m.masc_Nombre, r.raza_Descripcion, ref.refg_Nombre
    HAVING (@soloProblematicas = 0 OR 
            MAX(c.medic_FechaConsulta) IS NULL OR 
            MAX(c.medic_FechaConsulta) < DATEADD(MONTH, -6, GETDATE()) OR
            COUNT(CASE WHEN c.medic_ProximaCita IS NOT NULL AND c.medic_ProximaCita < GETDATE() THEN 1 END) > 0)
    ORDER BY 
        CASE 
            WHEN MAX(c.medic_FechaConsulta) IS NULL THEN 1
            WHEN COUNT(CASE WHEN c.medic_ProximaCita IS NOT NULL AND c.medic_ProximaCita < GETDATE() THEN 1 END) > 0 THEN 2
            WHEN MAX(c.medic_FechaConsulta) < DATEADD(MONTH, -6, GETDATE()) THEN 3
            ELSE 4
        END,
        m.masc_Nombre;
END
GO

PRINT 'Scripts SQL para Dashboard de Reportes con datos reales ejecutados correctamente.';
GO