/*
=============================================
SCRIPT 91: Los 7 estados de cita (paridad con OpenVPM)  -- SOLO BD
AUTOR:  Sistema PetsHome
FECHA:  2026-10-29
RAMA:   feature/reportes

OBJETIVO
  Que Medico.tbCitaMedica.cita_Estado maneje los MISMOS 7 estados de OpenVPM,
  ALMACENADOS SIN TILDE (la UI los muestra con tilde):

      Agendada · Confirmada · En espera · En atencion · Finalizada · No asistio · Cancelada

MAQUINA DE ESTADOS (transiciones validas)
      Agendada     -> Confirmada | Cancelada
      Confirmada   -> En espera  | No asistio | Cancelada
      En espera    -> En atencion| No asistio | Cancelada
      En atencion  -> Finalizada
      Finalizada / No asistio / Cancelada = FINALES (no salen)
      (misma -> misma = no-op idempotente permitido; protege doble-click)

ALCANCE DE ESTE SCRIPT (verificado en vivo con sqlcmd)
  Solo 2 SPs cambian. El resto YA es correcto y se deja intacto (minima interferencia):

    [CAMBIA] [Medico].[PR_Medico_Visita_CambiarEstado]
             -> acepta los 7 estados y VALIDA la transicion (misma firma).
    [CAMBIA] [Medico].[PR_Medico_Reportes_Citas]
             -> NoShow = COUNT(cita_Estado = 'No asistio')  (antes estaba fijo en 0).
                Atendidas = 'Finalizada' y Canceladas = 'Cancelada' ya eran correctos.

    [OK, sin tocar] [Medico].[PR_Medico_Whiteboard_List]     (script 67)
                    filtra IN ('En espera','En atencion','Finalizada') -> Agendada/
                    Confirmada/No asistio/Cancelada NO aparecen en el tablero.
    [OK, sin tocar] [Medico].[PR_Medico_SalaEspera_List]     (script 59, ultima version)
                    filtra IN ('En espera','En atencion') -> activas; no cuenta
                    Cancelada/No asistio como activas.
    [OK, sin tocar] Dashboards 26/31/32 -> NO filtran por cita_Estado (calculan una
                    etiqueta a partir de la fecha); nuevos estados no los rompen.
    [OK, sin tocar] Lista de espera (script 77) -> es OTRA tabla (Medico.tbListaEspera,
                    estados Esperando/Agendada/Cancelada); no usa cita_Estado.

REGLAS RESPETADAS
  - SIN CHECK constraint sobre cita_Estado: la validacion vive SOLO en el SP, para
    no invalidar datos viejos. La columna sigue VARCHAR(20) NOT NULL (cabe el estado
    mas largo, 'En atencion' = 11). Los 3 estados actuales (En espera / En atencion /
    Finalizada) siguen siendo validos.
  - Idempotente (CREATE OR ALTER). CERO cambios de esquema.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

-- =====================================================
-- 1) CambiarEstado de la visita: 7 estados + validacion de transicion.
--    Misma firma que la version previa (script 55).
--    CodeStatus:  1 OK · -1 no existe · -2 estado invalido ·
--                -3 transicion no permitida · -5 error no controlado.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Visita_CambiarEstado]
    @cita_Id     INT,
    @cita_Estado VARCHAR(20),
    @usuario     INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        -- Normalizar espacios del estado destino
        SET @cita_Estado = LTRIM(RTRIM(@cita_Estado))

        -- 1) La cita debe existir y estar activa
        DECLARE @actual VARCHAR(20) =
            (SELECT cita_Estado FROM Medico.tbCitaMedica
             WHERE cita_Id = @cita_Id AND cita_EsEliminado = 0)

        IF @actual IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, 'La cita no fue encontrada.' AS MessageStatus
            RETURN
        END

        -- 2) El estado destino debe ser uno de los 7 estados validos
        IF @cita_Estado NOT IN
            ('Agendada','Confirmada','En espera','En atencion','Finalizada','No asistio','Cancelada')
        BEGIN
            SELECT -2 AS CodeStatus, 'Estado no valido.' AS MessageStatus
            RETURN
        END

        -- 3) Validar la transicion contra la maquina de estados
        DECLARE @ok BIT =
            CASE
                WHEN @actual = @cita_Estado THEN 1  -- no-op idempotente
                WHEN @actual = 'Agendada'    AND @cita_Estado IN ('Confirmada','Cancelada')                THEN 1
                WHEN @actual = 'Confirmada'  AND @cita_Estado IN ('En espera','No asistio','Cancelada')    THEN 1
                WHEN @actual = 'En espera'   AND @cita_Estado IN ('En atencion','No asistio','Cancelada')  THEN 1
                WHEN @actual = 'En atencion' AND @cita_Estado = 'Finalizada'                               THEN 1
                ELSE 0  -- Finalizada/No asistio/Cancelada son finales; cualquier otra combinacion no vale
            END

        IF @ok = 0
        BEGIN
            SELECT -3 AS CodeStatus,
                   'Transicion no permitida: de ''' + @actual + ''' a ''' + @cita_Estado + '''.' AS MessageStatus
            RETURN
        END

        -- 4) Aplicar el cambio de estado
        UPDATE Medico.tbCitaMedica
        SET cita_Estado          = @cita_Estado,
            cita_UsuarioModifica = @usuario,
            cita_FechaModifica   = GETDATE()
        WHERE cita_Id = @cita_Id

        SELECT 1 AS CodeStatus, 'Estado actualizado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- 2) Reporte de citas: KPIs en una sola fila.
--    FIX: NoShow ahora cuenta 'No asistio' (antes estaba fijo en 0).
--    Mismas columnas/orden -> no rompe PR_Medico_Reportes_CitasResult.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Reportes_Citas]
    @desde DATE,
    @hasta DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(*)                                                       AS Total,
        SUM(CASE WHEN c.cita_Estado = 'Finalizada' THEN 1 ELSE 0 END)  AS Atendidas,
        SUM(CASE WHEN c.cita_Estado = 'No asistio' THEN 1 ELSE 0 END)  AS NoShow,
        SUM(CASE WHEN c.cita_Estado = 'Cancelada'  THEN 1 ELSE 0 END)  AS Canceladas
    FROM [Medico].[tbCitaMedica] c
    WHERE c.cita_EsEliminado = 0
      AND CAST(c.cita_FechaConsulta AS DATE) BETWEEN @desde AND @hasta;
END
GO

PRINT 'Script 91 completado: 7 estados de cita (CambiarEstado con validacion de transiciones + Reportes_Citas cuenta No asistio).';
GO
