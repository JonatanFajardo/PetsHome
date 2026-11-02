/*
=============================================
SCRIPT: Tendencia de LABORATORIO en la ficha del paciente
AUTOR:  Sistema PetsHome
FECHA:  2026-11-02
DETALLE: SP PR_Medico_Laboratorio_Tendencia (@masc_Id) que devuelve,
         para una mascota, cada parametro medido a lo largo del tiempo
         (parametro, valor, unidad, rango de referencia y fecha de
         resultado) para dibujar la grafica de evolucion entre visitas,
         al estilo de OpenVPM.
         Solo examenes con resultado (lab_FechaResultado no nulo) y no
         eliminados. Ordenado por parametro y fecha.
         Aditivo: no reemplaza nada del script 71 (modulo Laboratorio).
         Incluye una siembra MINIMA e IDEMPOTENTE para que al menos una
         mascota tenga un parametro con 2+ fechas y la tendencia se vea.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE PETSHOMEDB
GO

-- =====================================================
-- SP: PR_Medico_Laboratorio_Tendencia
--   Filas de los parametros de laboratorio de una mascota
--   a lo largo del tiempo, para graficar su evolucion.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Laboratorio_Tendencia]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        d.labd_Parametro      AS Parametro,
        d.labd_Valor          AS Valor,
        d.labd_Unidad         AS Unidad,
        d.labd_RangoMin       AS RangoMin,
        d.labd_RangoMax       AS RangoMax,
        lab.lab_FechaResultado AS FechaResultado
    FROM [Medico].[tbLaboratorio] AS lab
    INNER JOIN [Medico].[tbLaboratorioDetalle] AS d
        ON d.lab_Id = lab.lab_Id
    WHERE lab.lab_EsEliminado = 0
      AND d.labd_EsEliminado  = 0
      AND lab.masc_Id         = @masc_Id
      AND lab.lab_FechaResultado IS NOT NULL
    ORDER BY d.labd_Parametro ASC, lab.lab_FechaResultado ASC
END
GO

-- =====================================================
-- SIEMBRA MINIMA DE DEMOSTRACION (idempotente)
--   Da a la mascota 23 un segundo punto de Glucosa y
--   Hematocrito en otra fecha, para que su tendencia tenga
--   2+ puntos y la grafica se vea. Marcada por observaciones
--   para no duplicarse en re-ejecuciones.
-- =====================================================
IF EXISTS (SELECT 1 FROM [Refugio].[tbMascotas] WHERE masc_Id = 23 AND masc_EsEliminado = 0)
   AND NOT EXISTS (SELECT 1 FROM [Medico].[tbLaboratorio]
                   WHERE masc_Id = 23 AND lab_Observaciones = 'SEED_TENDENCIA')
BEGIN
    DECLARE @usu INT = (SELECT TOP 1 lab_UsuarioCrea FROM [Medico].[tbLaboratorio]
                        WHERE masc_Id = 23 ORDER BY lab_Id)
    IF @usu IS NULL SET @usu = (SELECT TOP 1 usu_Id FROM [Seguridad].[tbUsuarios] ORDER BY usu_Id)

    INSERT INTO [Medico].[tbLaboratorio]
        (masc_Id, lab_Examen, lab_Tipo, lab_Estado,
         lab_FechaSolicitud, lab_FechaResultado, lab_Observaciones,
         lab_UsuarioCrea, lab_FechaCrea)
    VALUES
        (23, 'Perfil de control', 'Sangre', 'Revisado',
         '2026-10-28', '2026-10-28', 'SEED_TENDENCIA', @usu, GETDATE())

    DECLARE @labNuevo INT = CAST(SCOPE_IDENTITY() AS INT)

    INSERT INTO [Medico].[tbLaboratorioDetalle]
        (lab_Id, labd_Parametro, labd_Valor, labd_Unidad,
         labd_RangoMin, labd_RangoMax, labd_FueraRango, labd_UsuarioCrea, labd_FechaCrea)
    VALUES
        (@labNuevo, 'Glucosa',     '110', 'mg/dL', 70.0, 120.0, 0, @usu, GETDATE()),
        (@labNuevo, 'Hematocrito', '48',  '%',     37.0, 55.0,  0, @usu, GETDATE())
END
GO

PRINT 'Tendencia de LABORATORIO: SP PR_Medico_Laboratorio_Tendencia creado.'
GO
