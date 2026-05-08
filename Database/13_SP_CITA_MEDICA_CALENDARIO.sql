/*
==============================================
SCRIPT: SP para Calendario de Citas Médicas
AUTOR: PetsHome
FECHA: 2026-04-25
==============================================
*/

USE PETSHOMEDB
GO

-- =============================================
-- SP: PR_Medico_CitaMedica_Calendario
-- Descripción: Lista citas médicas en un rango
--              de fechas para el calendario.
--              Normaliza TipoConsulta a los
--              valores que espera el JS/CSS:
--              Emergencia, Vacunacion, Control,
--              Cirugia, Consulta General
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_CitaMedica_Calendario')
    DROP PROCEDURE [Medico].[PR_Medico_CitaMedica_Calendario]
GO

CREATE PROCEDURE [Medico].[PR_Medico_CitaMedica_Calendario]
    @FechaInicio DATE,
    @FechaFin    DATE
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        cita.cita_Id,
        masc.masc_Nombre                                        AS Mascota,
        cita.cita_FechaConsulta,
        -- Normalizar al tipo que espera el calendario (coincide con clases CSS del JS)
        CASE tipoCon.tipoCon_Descripcion
            WHEN 'Emergencia'        THEN 'Emergencia'
            WHEN 'Vacunacion'        THEN 'Vacunacion'
            WHEN 'Seguimiento'       THEN 'Control'
            WHEN 'Chequeo de rutina' THEN 'Control'
            WHEN 'Pre-quirurgico'    THEN 'Cirugia'
            WHEN 'Post-quirurgico'   THEN 'Cirugia'
            ELSE                     'Consulta General'
        END                                                     AS TipoConsulta,
        ISNULL(cita.cita_MotivoConsulta, '')                    AS cita_MotivoConsulta,
        ISNULL(grav.grav_Descripcion, '')                       AS Gravedad,
        -- Duración en minutos según tipo de consulta
        CASE tipoCon.tipoCon_Descripcion
            WHEN 'Emergencia'        THEN 60
            WHEN 'Pre-quirurgico'    THEN 90
            WHEN 'Post-quirurgico'   THEN 60
            WHEN 'Vacunacion'        THEN 20
            ELSE 30
        END                                                     AS Duracion
    FROM [Medico].[tbCitaMedica] AS cita
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON cita.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposConsulta] AS tipoCon
        ON cita.tipoCon_Id = tipoCon.tipoCon_Id
    LEFT JOIN [Medico].[tbGravedades] AS grav
        ON cita.grav_Id = grav.grav_Id
    WHERE cita.cita_EsEliminado = 0
      AND CAST(cita.cita_FechaConsulta AS DATE) BETWEEN @FechaInicio AND @FechaFin
    ORDER BY cita.cita_FechaConsulta ASC
END
GO

PRINT '=========================================='
PRINT 'SP CALENDARIO CREADO / ACTUALIZADO'
PRINT 'PR_Medico_CitaMedica_Calendario'
PRINT '=========================================='
GO
