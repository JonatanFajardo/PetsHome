/*
=============================================
SCRIPT 69: Calendario con lente por veterinario (Agenda - rol × función → filtros)
AUTOR: Sistema PetsHome
FECHA: 2026-10-22
DESCRIPCIÓN:
  Amplía [Medico].[PR_Medico_CitaMedica_Calendario] con @vet_Id (opcional) y devuelve
  el veterinario asignado. Filtro: si @vet_Id se envía, trae solo las citas de ese vet
  + las SIN asignar (lente del veterinario). Si NULL, trae todas (recepción/admin).
  CERO cambios de esquema (tbCitaMedica.cita_Veterinario ya existe). Idempotente.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_CitaMedica_Calendario]
    @FechaInicio DATE,
    @FechaFin    DATE,
    @vet_Id      INT = NULL
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        cita.cita_Id,
        masc.masc_Nombre                                        AS Mascota,
        cita.cita_FechaConsulta,
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
        CASE tipoCon.tipoCon_Descripcion
            WHEN 'Emergencia'        THEN 60
            WHEN 'Pre-quirurgico'    THEN 90
            WHEN 'Post-quirurgico'   THEN 60
            WHEN 'Vacunacion'        THEN 20
            ELSE 30
        END                                                     AS Duracion,
        cita.cita_Veterinario                                   AS cita_Veterinario,
        CAST(u.Usu_Nombre AS NVARCHAR(200))                     AS Veterinario
    FROM [Medico].[tbCitaMedica] AS cita
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON cita.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposConsulta] AS tipoCon
        ON cita.tipoCon_Id = tipoCon.tipoCon_Id
    LEFT JOIN [Medico].[tbGravedades] AS grav
        ON cita.grav_Id = grav.grav_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS u
        ON cita.cita_Veterinario = u.usu_Id
    WHERE cita.cita_EsEliminado = 0
      AND CAST(cita.cita_FechaConsulta AS DATE) BETWEEN @FechaInicio AND @FechaFin
      AND (@vet_Id IS NULL
           OR cita.cita_Veterinario = @vet_Id
           OR cita.cita_Veterinario IS NULL)
    ORDER BY cita.cita_FechaConsulta ASC
END
GO

PRINT '✅ Script 69: PR_Medico_CitaMedica_Calendario con lente @vet_Id'
GO
