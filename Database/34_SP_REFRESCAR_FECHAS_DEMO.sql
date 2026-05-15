-- =============================================================================
-- Refrescar fechas del demo: distribuye toda la data en ventanas relativas a HOY
-- Idempotente — se puede ejecutar las veces que quieras
-- =============================================================================

CREATE OR ALTER PROCEDURE [General].[PR_General_RefrescarFechasDemo]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Hoy DATETIME = GETDATE();
    DECLARE @MascotasUpd INT = 0, @AdopcionesUpd INT = 0, @DonacionesUpd INT = 0,
            @SolicitudesUpd INT = 0, @CitasUpd INT = 0, @ProxCitaUpd INT = 0,
            @EventosUpd INT = 0;

    -- ============================================================
    -- 1. MASCOTAS — distribución en últimos 180 días (6 meses)
    -- ============================================================
    ;WITH ord AS (
        SELECT masc_Id,
               ROW_NUMBER() OVER (ORDER BY masc_FechaCrea, masc_Id) AS rn,
               COUNT(*)     OVER () AS total
        FROM Refugio.tbMascotas WHERE masc_EsEliminado = 0
    )
    UPDATE m
       SET masc_FechaCrea = DATEADD(DAY, -CAST((o.total - o.rn) * 180.0 / NULLIF(o.total,0) AS INT), @Hoy)
      FROM Refugio.tbMascotas m
      JOIN ord o ON o.masc_Id = m.masc_Id;
    SET @MascotasUpd = @@ROWCOUNT;

    -- ============================================================
    -- 2. ADOPCIONES — distribución en últimos 180 días
    -- ============================================================
    ;WITH ord AS (
        SELECT adop_Id,
               ROW_NUMBER() OVER (ORDER BY adop_FechaCrea, adop_Id) AS rn,
               COUNT(*)     OVER () AS total
        FROM Refugio.tbAdopciones WHERE adop_EsEliminado = 0
    )
    UPDATE a
       SET adop_FechaCrea = DATEADD(DAY, -CAST((o.total - o.rn) * 180.0 / NULLIF(o.total,0) AS INT), @Hoy)
      FROM Refugio.tbAdopciones a
      JOIN ord o ON o.adop_Id = a.adop_Id;
    SET @AdopcionesUpd = @@ROWCOUNT;

    -- ============================================================
    -- 3. DONACIONES — distribución en últimos 150 días (5 meses)
    -- ============================================================
    ;WITH ord AS (
        SELECT dona_Id,
               ROW_NUMBER() OVER (ORDER BY dona_FechaDonacion, dona_Id) AS rn,
               COUNT(*)     OVER () AS total
        FROM Refugio.tbDonaciones WHERE dona_EsEliminado = 0
    )
    UPDATE d
       SET dona_FechaDonacion = DATEADD(DAY, -CAST((o.total - o.rn) * 150.0 / NULLIF(o.total,0) AS INT), @Hoy),
           dona_FechaCrea     = DATEADD(DAY, -CAST((o.total - o.rn) * 150.0 / NULLIF(o.total,0) AS INT), @Hoy)
      FROM Refugio.tbDonaciones d
      JOIN ord o ON o.dona_Id = d.dona_Id;
    SET @DonacionesUpd = @@ROWCOUNT;

    -- ============================================================
    -- 4. SOLICITUDES — últimos 60 días (algunas viejas para alertar
    --    "días de antigüedad" en la card de pendientes)
    -- ============================================================
    ;WITH ord AS (
        SELECT sol_Id,
               ROW_NUMBER() OVER (ORDER BY sol_Fecha, sol_Id) AS rn,
               COUNT(*)     OVER () AS total
        FROM Refugio.tbSolicitudes WHERE sol_EsEliminado = 0
    )
    UPDATE s
       SET sol_Fecha     = DATEADD(DAY, -CAST((o.total - o.rn) * 60.0 / NULLIF(o.total,0) AS INT), @Hoy),
           sol_FechaCrea = DATEADD(DAY, -CAST((o.total - o.rn) * 60.0 / NULLIF(o.total,0) AS INT), @Hoy)
      FROM Refugio.tbSolicitudes s
      JOIN ord o ON o.sol_Id = s.sol_Id;
    SET @SolicitudesUpd = @@ROWCOUNT;

    -- ============================================================
    -- 5. CITAS MÉDICAS — últimos 28 días + algunas HOY
    --    (alimenta heatmap + card "Citas hoy")
    -- ============================================================
    ;WITH ord AS (
        SELECT cita_Id,
               ROW_NUMBER() OVER (ORDER BY cita_FechaConsulta, cita_Id) AS rn,
               COUNT(*)     OVER () AS total
        FROM Medico.tbCitaMedica WHERE cita_EsEliminado = 0
    )
    UPDATE c
       SET cita_FechaConsulta = DATEADD(MINUTE,
                                        ((o.rn * 37) % 8) * 60 + 8 * 60, -- horas entre 08:00 y 16:00
                                        CAST(DATEADD(DAY, -CAST((o.total - o.rn) * 28.0 / NULLIF(o.total,0) AS INT),
                                                     CAST(@Hoy AS DATE)) AS DATETIME)),
           cita_FechaCrea     = DATEADD(DAY, -CAST((o.total - o.rn) * 28.0 / NULLIF(o.total,0) AS INT), @Hoy)
      FROM Medico.tbCitaMedica c
      JOIN ord o ON o.cita_Id = c.cita_Id;
    SET @CitasUpd = @@ROWCOUNT;

    -- 5b. Forzar que las 3 citas más recientes queden HOY (a distintas horas)
    ;WITH topRec AS (
        SELECT TOP 3 cita_Id, ROW_NUMBER() OVER (ORDER BY cita_FechaConsulta DESC) AS rn
        FROM Medico.tbCitaMedica WHERE cita_EsEliminado = 0
        ORDER BY cita_FechaConsulta DESC
    )
    UPDATE c
       SET cita_FechaConsulta = DATEADD(HOUR, 8 + (t.rn - 1) * 3, CAST(CAST(@Hoy AS DATE) AS DATETIME))
      FROM Medico.tbCitaMedica c
      JOIN topRec t ON t.cita_Id = c.cita_Id;

    -- ============================================================
    -- 6. PRÓXIMA CITA — próximos 14 días (para "Alertas activas")
    -- ============================================================
    ;WITH withProx AS (
        SELECT cita_Id,
               ROW_NUMBER() OVER (ORDER BY cita_Id) AS rn,
               COUNT(*) OVER () AS total
        FROM Medico.tbCitaMedica
        WHERE cita_EsEliminado = 0 AND cita_ProximaCita IS NOT NULL
    )
    UPDATE c
       SET cita_ProximaCita = DATEADD(DAY, CAST(w.rn * 14.0 / NULLIF(w.total,0) AS INT), CAST(@Hoy AS DATE))
      FROM Medico.tbCitaMedica c
      JOIN withProx w ON w.cita_Id = c.cita_Id;
    SET @ProxCitaUpd = @@ROWCOUNT;

    -- ============================================================
    -- 7. EVENTOS — mitad en el mes pasado, mitad en el próximo mes
    -- ============================================================
    ;WITH ord AS (
        SELECT eve_Id,
               ROW_NUMBER() OVER (ORDER BY eve_Fecha, eve_Id) AS rn,
               COUNT(*)     OVER () AS total
        FROM Refugio.tbEventos WHERE eve_EsEliminado = 0
    )
    UPDATE e
       SET eve_Fecha = DATEADD(DAY,
                               CAST((o.rn - o.total / 2.0) * 60.0 / NULLIF(o.total,0) AS INT),
                               CAST(@Hoy AS DATE))
      FROM Refugio.tbEventos e
      JOIN ord o ON o.eve_Id = e.eve_Id;
    SET @EventosUpd = @@ROWCOUNT;

    -- ============================================================
    -- RESUMEN
    -- ============================================================
    SELECT
        @MascotasUpd    AS Mascotas,
        @AdopcionesUpd  AS Adopciones,
        @DonacionesUpd  AS Donaciones,
        @SolicitudesUpd AS Solicitudes,
        @CitasUpd       AS Citas,
        @ProxCitaUpd    AS ProximasCitas,
        @EventosUpd     AS Eventos,
        @Hoy            AS EjecutadoEn;
END
GO
