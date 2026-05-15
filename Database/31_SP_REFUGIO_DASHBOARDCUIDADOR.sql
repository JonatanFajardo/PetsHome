USE PETSHOMEDB
GO

-- ============================================================
-- PR_Refugio_DashboardCuidador_MascotasActivas
-- ============================================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_DashboardCuidador_MascotasActivas]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        m.masc_Id,
        m.masc_Nombre,
        r.raza_TipoAnimal       AS masc_Especie,
        r.raza_Descripcion      AS masc_Raza,
        m.masc_Edad,
        m.masc_Sexo,
        m.masc_Color,
        p.proc_Descripcion      AS masc_Procedencia
    FROM [Refugio].[tbMascotas] m
    INNER JOIN [Refugio].[tbRazas]       r ON r.raza_Id = m.raza_Id
    LEFT  JOIN [Refugio].[tbProcedencias] p ON p.proc_Id = m.proc_Id
    WHERE m.masc_EsEliminado = 0
      AND m.masc_EsAdoptado  = 0
    ORDER BY m.masc_FechaCrea DESC
END
GO

-- ============================================================
-- PR_Medico_DashboardCuidador_CitasHoy
-- ============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_DashboardCuidador_CitasHoy]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        c.cita_Id,
        m.masc_Nombre,
        r.raza_TipoAnimal                           AS masc_Especie,
        c.cita_FechaConsulta                        AS cita_FechaHora,
        ISNULL(tc.tipoCon_Descripcion, 'General')   AS cita_TipoConsulta,
        CASE
            WHEN c.cita_FechaConsulta < GETDATE()   THEN 'Completada'
            WHEN g.grav_Descripcion LIKE '%urgencia%'
              OR g.grav_Descripcion LIKE '%emergencia%' THEN 'Urgente'
            ELSE 'Pendiente'
        END                                         AS cita_Estado
    FROM [Medico].[tbCitaMedica] c
    INNER JOIN [Refugio].[tbMascotas]     m  ON m.masc_Id     = c.masc_Id
    INNER JOIN [Refugio].[tbRazas]        r  ON r.raza_Id     = m.raza_Id
    LEFT  JOIN [Medico].[tbTiposConsulta] tc ON tc.tipoCon_Id = c.tipoCon_Id
    LEFT  JOIN [Medico].[tbGravedades]    g  ON g.grav_Id     = c.grav_Id
    WHERE c.cita_EsEliminado = 0
      AND CAST(c.cita_FechaConsulta AS DATE) = CAST(GETDATE() AS DATE)
    ORDER BY c.cita_FechaConsulta
END
GO

-- ============================================================
-- PR_Medico_DashboardCuidador_AlertasActivas
-- Vacunas vencidas + citas urgentes
-- ============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_DashboardCuidador_AlertasActivas]
AS
BEGIN
    SET NOCOUNT ON

    -- Vacunas vencidas
    SELECT
        'red'                   AS alert_Tipo,
        'Vacuna vencida'        AS alert_Descripcion,
        m.masc_Nombre           AS alert_MascotaNombre,
        v.vac_Descripcion       AS alert_Detalle,
        cv.cita_ProximaCita     AS alert_FechaRef
    FROM [Medico].[tbCitaMedica] cv
    INNER JOIN [Refugio].[tbMascotas] m  ON m.masc_Id  = cv.masc_Id
    INNER JOIN [Refugio].[tbVacunas]  v  ON v.vac_Id   = cv.vac_Id
    WHERE cv.cita_EsEliminado = 0
      AND cv.cita_ProximaCita IS NOT NULL
      AND cv.cita_ProximaCita < GETDATE()
      AND m.masc_EsEliminado = 0
      AND m.masc_EsAdoptado  = 0

    UNION ALL

    -- Citas urgentes pendientes
    SELECT
        'orange'                AS alert_Tipo,
        'Cita urgente'          AS alert_Descripcion,
        m.masc_Nombre           AS alert_MascotaNombre,
        ISNULL(tc.tipoCon_Descripcion,'Consulta general') AS alert_Detalle,
        c.cita_FechaConsulta    AS alert_FechaRef
    FROM [Medico].[tbCitaMedica] c
    INNER JOIN [Refugio].[tbMascotas]     m  ON m.masc_Id     = c.masc_Id
    INNER JOIN [Medico].[tbGravedades]    g  ON g.grav_Id     = c.grav_Id
    LEFT  JOIN [Medico].[tbTiposConsulta] tc ON tc.tipoCon_Id = c.tipoCon_Id
    WHERE c.cita_EsEliminado = 0
      AND (g.grav_Descripcion LIKE '%urgencia%' OR g.grav_Descripcion LIKE '%emergencia%')
      AND c.cita_FechaConsulta >= GETDATE()
      AND m.masc_EsEliminado = 0

    ORDER BY alert_Tipo, alert_FechaRef
END
GO

-- ============================================================
-- PR_Refugio_DashboardCuidador_SolicitudesPendientes
-- ============================================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_DashboardCuidador_SolicitudesPendientes]
AS
BEGIN
    SET NOCOUNT ON
    SELECT TOP 8
        s.sol_Id,
        s.sol_Nombres + ' ' + s.sol_Apellidos   AS sol_NombreCompleto,
        s.sol_Correo,
        m.masc_Nombre,
        r.raza_TipoAnimal                       AS masc_Especie,
        s.sol_Fecha,
        ISNULL(s.sol_Estado, 'Pendiente')       AS sol_Estado
    FROM [Refugio].[tbSolicitudes] s
    INNER JOIN [Refugio].[tbMascotas] m ON m.masc_Id = s.masc_Id
    INNER JOIN [Refugio].[tbRazas]    r ON r.raza_Id = m.raza_Id
    WHERE s.sol_EsEliminado = 0
    ORDER BY s.sol_Fecha DESC
END
GO

-- ============================================================
-- Registrar pantalla "Dashboard cuidador" y asignarla al rol
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Dashboard cuidador')
    INSERT INTO [Seguridad].[tbPantallas] (pan_Descripcion, pan_Grupo, pan_EsActivo)
    VALUES ('Dashboard cuidador', 'Home', 1)
GO

DECLARE @pan_Id INT = (SELECT pan_Id FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Dashboard cuidador')

IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesPantallas] WHERE rol_Id = 5 AND pan_Id = @pan_Id)
    INSERT INTO [Seguridad].[tbRolesPantallas] (rol_Id, pan_Id, ropan_EsActivo, ropan_Consultar, ropan_Insertar, ropan_Editar, ropan_Eliminar)
    VALUES (5, @pan_Id, 1, 1, 0, 0, 0)
ELSE
    UPDATE [Seguridad].[tbRolesPantallas]
    SET ropan_EsActivo = 1, ropan_Consultar = 1
    WHERE rol_Id = 5 AND pan_Id = @pan_Id
GO

PRINT 'SPs DashboardCuidador creados correctamente.'
GO
