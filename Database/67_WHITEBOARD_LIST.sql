/*
=============================================
SCRIPT 67: Whiteboard / tablero Kanban del día (Mejoras v3 - Fase 4)
AUTOR: Sistema PetsHome
FECHA: 2026-10-22
DESCRIPCIÓN:
  [Medico].[PR_Medico_Whiteboard_List] = misma forma que PR_Medico_SalaEspera_List
  pero con los 3 estados del tablero Kanban del día: En espera / En atencion / Finalizada.
  (El SalaEspera_List se deja intacto: solo trae espera/atencion para el tablero antiguo.)
  Resultado compatible con PR_Medico_SalaEspera_ListResult. CERO cambios de esquema.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Whiteboard_List]
    @fecha  DATE = NULL,
    @vet_Id INT  = NULL
AS
BEGIN
    SET NOCOUNT ON
    IF @fecha IS NULL SET @fecha = CAST(GETDATE() AS DATE)

    SELECT
        c.cita_Id                                                AS cita_Id,
        c.masc_Id                                                AS masc_Id,
        m.masc_Nombre                                            AS MascotaNombre,
        CASE WHEN m.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(cli.cli_Nombres + ' ' + ISNULL(cli.cli_Apellidos, '')))
             ELSE NULL END                                       AS DuenoNombre,
        CAST(CASE WHEN m.cli_Id IS NULL THEN 1 ELSE 0 END AS BIT) AS EsInterno,
        c.cita_MotivoConsulta                                    AS Motivo,
        tc.tipoCon_Descripcion                                   AS TipoConsulta,
        c.cita_Prioridad                                         AS Prioridad,
        c.cita_HoraLlegada                                       AS HoraLlegada,
        DATEDIFF(MINUTE, ISNULL(c.cita_HoraLlegada, c.cita_FechaConsulta), GETDATE()) AS MinutosEspera,
        c.cita_Estado                                            AS Estado,
        CAST(u.Usu_Nombre AS NVARCHAR(200))                      AS VetAsignado
    FROM [Medico].[tbCitaMedica] c
    INNER JOIN [Refugio].[tbMascotas] m        ON c.masc_Id = m.masc_Id
    LEFT  JOIN [Veterinaria].[tbClientes] cli  ON m.cli_Id = cli.cli_Id
    LEFT  JOIN [Medico].[tbTiposConsulta] tc   ON c.tipoCon_Id = tc.tipoCon_Id
    LEFT  JOIN [Seguridad].[tbUsuarios] u      ON c.cita_Veterinario = u.usu_Id
    WHERE c.cita_EsEliminado = 0
      AND CAST(c.cita_FechaConsulta AS DATE) = @fecha
      AND c.cita_Estado IN ('En espera', 'En atencion', 'Finalizada')
      AND (@vet_Id IS NULL OR c.cita_Veterinario = @vet_Id OR c.cita_Veterinario IS NULL)
    ORDER BY
        CASE c.cita_Prioridad WHEN 'Urgente' THEN 0 WHEN 'Alta' THEN 1 ELSE 2 END,
        c.cita_HoraLlegada ASC
END
GO

PRINT '✅ Script 67: PR_Medico_Whiteboard_List creado'
GO
