USE PETSHOMEDB
GO

-- ============================================================
-- PR_Medico_DashboardVeterinario_AgendaHoy
-- ============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_DashboardVeterinario_AgendaHoy]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        c.cita_Id,
        m.masc_Nombre,
        r.raza_TipoAnimal                           AS masc_Especie,
        r.raza_Descripcion                          AS raz_Descripcion,
        c.cita_FechaConsulta                        AS cita_FechaHora,
        ISNULL(tc.tipoCon_Descripcion, 'General')   AS cita_TipoConsulta,
        CASE
            WHEN c.cita_FechaConsulta < GETDATE()   THEN 'Completada'
            WHEN g.grav_Descripcion LIKE '%urgencia%'
              OR g.grav_Descripcion LIKE '%emergencia%' THEN 'Urgente'
            ELSE 'Pendiente'
        END                                         AS cita_Estado,
        CAST(CASE WHEN g.grav_Descripcion LIKE '%urgencia%'
                    OR g.grav_Descripcion LIKE '%emergencia%'
                  THEN 1 ELSE 0 END AS BIT)         AS cita_EsUrgente
    FROM [Medico].[tbCitaMedica] c
    INNER JOIN [Refugio].[tbMascotas]     m  ON m.masc_Id    = c.masc_Id
    INNER JOIN [Refugio].[tbRazas]        r  ON r.raza_Id    = m.raza_Id
    LEFT  JOIN [Medico].[tbTiposConsulta] tc ON tc.tipoCon_Id = c.tipoCon_Id
    LEFT  JOIN [Medico].[tbGravedades]    g  ON g.grav_Id    = c.grav_Id
    WHERE c.cita_EsEliminado = 0
      AND CAST(c.cita_FechaConsulta AS DATE) = CAST(GETDATE() AS DATE)
    ORDER BY c.cita_FechaConsulta
END
GO

-- ============================================================
-- PR_Medico_DashboardVeterinario_TratamientosActivos
-- ============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_DashboardVeterinario_TratamientosActivos]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        t.trat_Id,
        m.masc_Nombre,
        r.raza_TipoAnimal                                               AS masc_Especie,
        r.raza_Descripcion                                              AS raz_Descripcion,
        t.trat_Medicamento                                              AS trat_Descripcion,
        DATEDIFF(DAY, t.trat_FechaAplicacion, GETDATE())                AS trat_DiaActual,
        CASE WHEN t.trat_ProximaDosis IS NOT NULL
             THEN DATEDIFF(DAY, t.trat_FechaAplicacion, t.trat_ProximaDosis)
             ELSE 14 END                                                AS trat_DuracionTotal,
        ISNULL(t.trat_ProximaDosis, DATEADD(DAY,14,t.trat_FechaAplicacion)) AS trat_FechaFin,
        ISNULL(t.trat_Estado, 'Activo')                                 AS trat_Estado
    FROM [Medico].[tbTratamientos] t
    INNER JOIN [Refugio].[tbMascotas] m ON m.masc_Id = t.masc_Id
    INNER JOIN [Refugio].[tbRazas]    r ON r.raza_Id = m.raza_Id
    WHERE t.trat_EsEliminado = 0
      AND ISNULL(t.trat_Estado,'Activo') NOT IN ('Finalizado','Cancelado')
    ORDER BY t.trat_FechaAplicacion DESC
END
GO

-- ============================================================
-- PR_Medico_DashboardVeterinario_AlertasVeterinario
-- ============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_DashboardVeterinario_AlertasVeterinario]
AS
BEGIN
    SET NOCOUNT ON

    -- Vacunas vencidas
    SELECT
        'red'                                       AS alert_Tipo,
        'Vacuna vencida'                            AS alert_Descripcion,
        m.masc_Nombre                               AS alert_MascotaNombre,
        v.vac_Descripcion + ' — vacuna vencida'     AS alert_Detalle,
        MAX(c.cita_FechaConsulta)                   AS alert_FechaRef
    FROM [Medico].[tbCitaMedica] c
    INNER JOIN [Refugio].[tbMascotas] m ON m.masc_Id = c.masc_Id
    INNER JOIN [Refugio].[tbVacunas]  v ON v.vac_Id  = c.vac_Id
    WHERE c.vac_Id IS NOT NULL AND c.cita_EsEliminado = 0 AND m.masc_EsEliminado = 0
    GROUP BY m.masc_Id, m.masc_Nombre, v.vac_Id, v.vac_Descripcion, v.vacu_PeriodoRefuerzo
    HAVING DATEADD(DAY,
        CASE WHEN v.vacu_PeriodoRefuerzo LIKE '%3 a%' THEN 1095
             WHEN v.vacu_PeriodoRefuerzo LIKE '%2 a%' THEN 730
             WHEN v.vacu_PeriodoRefuerzo LIKE '%6 m%' THEN 180
             ELSE 365 END,
        MAX(c.cita_FechaConsulta)) < GETDATE()

    UNION ALL

    -- Vacunas próximas (≤ 30 días)
    SELECT
        'blue'                                      AS alert_Tipo,
        'Vacuna próxima'                            AS alert_Descripcion,
        m.masc_Nombre                               AS alert_MascotaNombre,
        v.vac_Descripcion + ' — vence en ' +
            CAST(DATEDIFF(DAY, GETDATE(), DATEADD(DAY,
                CASE WHEN v.vacu_PeriodoRefuerzo LIKE '%3 a%' THEN 1095
                     WHEN v.vacu_PeriodoRefuerzo LIKE '%2 a%' THEN 730
                     WHEN v.vacu_PeriodoRefuerzo LIKE '%6 m%' THEN 180
                     ELSE 365 END,
                MAX(c.cita_FechaConsulta))) AS VARCHAR) + ' días' AS alert_Detalle,
        MAX(c.cita_FechaConsulta)                   AS alert_FechaRef
    FROM [Medico].[tbCitaMedica] c
    INNER JOIN [Refugio].[tbMascotas] m ON m.masc_Id = c.masc_Id
    INNER JOIN [Refugio].[tbVacunas]  v ON v.vac_Id  = c.vac_Id
    WHERE c.vac_Id IS NOT NULL AND c.cita_EsEliminado = 0 AND m.masc_EsEliminado = 0
    GROUP BY m.masc_Id, m.masc_Nombre, v.vac_Id, v.vac_Descripcion, v.vacu_PeriodoRefuerzo
    HAVING DATEDIFF(DAY, GETDATE(), DATEADD(DAY,
        CASE WHEN v.vacu_PeriodoRefuerzo LIKE '%3 a%' THEN 1095
             WHEN v.vacu_PeriodoRefuerzo LIKE '%2 a%' THEN 730
             WHEN v.vacu_PeriodoRefuerzo LIKE '%6 m%' THEN 180
             ELSE 365 END,
        MAX(c.cita_FechaConsulta))) BETWEEN 0 AND 30

    UNION ALL

    -- Recetas activas sin revisión > 30 días
    SELECT
        'orange'                                    AS alert_Tipo,
        'Receta sin revisión'                       AS alert_Descripcion,
        m.masc_Nombre                               AS alert_MascotaNombre,
        r.receta_Medicamento + ' — sin control hace ' +
            CAST(DATEDIFF(DAY, r.receta_FechaInicio, GETDATE()) AS VARCHAR) + ' días' AS alert_Detalle,
        r.receta_FechaInicio                        AS alert_FechaRef
    FROM [Medico].[tbRecetas] r
    INNER JOIN [Refugio].[tbMascotas] m ON m.masc_Id = r.masc_Id
    WHERE r.receta_EsEliminado = 0
      AND ISNULL(r.receta_Estado,'Activa') NOT IN ('Finalizada','Cancelada')
      AND DATEDIFF(DAY, r.receta_FechaInicio, GETDATE()) > 30

    ORDER BY alert_Tipo, alert_FechaRef
END
GO

-- ============================================================
-- PR_Medico_DashboardVeterinario_ResumenMes
-- ============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_DashboardVeterinario_ResumenMes]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        COUNT(*)                                                            AS citas_TotalMes,
        SUM(CASE WHEN cita_FechaConsulta < GETDATE() THEN 1 ELSE 0 END)    AS citas_Completadas,
        SUM(CASE WHEN cita_FechaConsulta >= GETDATE() THEN 1 ELSE 0 END)   AS citas_Pendientes
    FROM [Medico].[tbCitaMedica]
    WHERE cita_EsEliminado = 0
      AND YEAR(cita_FechaConsulta)  = YEAR(GETDATE())
      AND MONTH(cita_FechaConsulta) = MONTH(GETDATE())
END
GO

PRINT '=========================================='
PRINT 'SPs DashboardVeterinario creados OK'
PRINT '    PR_Medico_DashboardVeterinario_AgendaHoy'
PRINT '    PR_Medico_DashboardVeterinario_TratamientosActivos'
PRINT '    PR_Medico_DashboardVeterinario_AlertasVeterinario'
PRINT '    PR_Medico_DashboardVeterinario_ResumenMes'
PRINT '=========================================='
GO
