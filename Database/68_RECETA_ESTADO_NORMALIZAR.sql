-- =============================================================================
-- 68_RECETA_ESTADO_NORMALIZAR.sql
-- Deuda tecnica (Mejoras v3): normalizar Medico.tbRecetas.receta_Estado.
--
-- PROBLEMA: la DB viva tenia 'Activa'/'Completada' (femenino) pero el SP Insert
-- y ConsultaService escribian 'Activo' (masculino). Ademas un lector
-- (PR_Medico_PerfilMedico_MedicamentosActivos) filtraba por 'Activo', por lo que
-- NO devolvia las recetas realmente activas ('Activa'). Por eso se difirio el CHECK.
--
-- SET CANONICO (FEMENINO): 'Activa' | 'Completada' | 'Cancelada' | 'Vencida'
--   - "receta" es sustantivo femenino y es lo que ya domina en los datos vivos.
--   - Las vistas Create/Edit ya usan el set femenino.
--   - 'Vencida' cierra el ciclo de vida (receta cuya fecha fin ya paso).
--
-- Requiere SET QUOTED_IDENTIFIER/ANSI_NULLS ON porque tbRecetas tiene indices
-- filtrados (UQ_Recetas_OperacionId, IX_Recetas_Estado) y el UPDATE los toca.
-- =============================================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

USE PETSHOMEDB;
GO

-- -----------------------------------------------------------------------------
-- 1) Migrar datos existentes al set canonico femenino
-- -----------------------------------------------------------------------------
UPDATE Medico.tbRecetas SET receta_Estado = 'Activa'
WHERE receta_Estado IN ('Activo', 'Activa');

UPDATE Medico.tbRecetas SET receta_Estado = 'Completada'
WHERE receta_Estado IN ('Completado', 'Completo', 'Finalizada', 'Finalizado');

UPDATE Medico.tbRecetas SET receta_Estado = 'Cancelada'
WHERE receta_Estado IN ('Cancelado', 'Suspendido', 'Suspendida');

UPDATE Medico.tbRecetas SET receta_Estado = 'Vencida'
WHERE receta_Estado IN ('Vencido');

-- Cualquier valor nulo/vacio/desconocido residual -> 'Activa' (valor por defecto)
UPDATE Medico.tbRecetas SET receta_Estado = 'Activa'
WHERE receta_Estado IS NULL
   OR LTRIM(RTRIM(receta_Estado)) = ''
   OR receta_Estado NOT IN ('Activa', 'Completada', 'Cancelada', 'Vencida');
GO

-- -----------------------------------------------------------------------------
-- 2) Rehacer el DEFAULT de la columna: 'Activo' (masculino) -> 'Activa'
--    El nombre del default es autogenerado; se resuelve dinamicamente.
-- -----------------------------------------------------------------------------
DECLARE @dfName SYSNAME;
SELECT @dfName = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE c.object_id = OBJECT_ID('Medico.tbRecetas') AND c.name = 'receta_Estado';

IF @dfName IS NOT NULL
    EXEC('ALTER TABLE Medico.tbRecetas DROP CONSTRAINT [' + @dfName + ']');

ALTER TABLE Medico.tbRecetas
    ADD CONSTRAINT DF_tbRecetas_Estado DEFAULT ('Activa') FOR receta_Estado;
GO

-- -----------------------------------------------------------------------------
-- 3) CHECK constraint con el set canonico
-- -----------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_tbRecetas_Estado')
    ALTER TABLE Medico.tbRecetas DROP CONSTRAINT CK_tbRecetas_Estado;
GO

ALTER TABLE Medico.tbRecetas WITH CHECK
    ADD CONSTRAINT CK_tbRecetas_Estado
    CHECK (receta_Estado IN ('Activa', 'Completada', 'Cancelada', 'Vencida'));
GO

-- -----------------------------------------------------------------------------
-- 4) Corregir SPs escritores/lectores que usaban el valor viejo
-- -----------------------------------------------------------------------------

-- 4a) Insert: default 'Activo' -> 'Activa'
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Recetas_Insert]
    @cita_Id INT,
    @masc_Id INT,
    @receta_Medicamento NVARCHAR(200),
    @tipoMed_Id INT = NULL,
    @viaAdmin_Id INT = NULL,
    @receta_Dosis NVARCHAR(100) = NULL,
    @receta_Frecuencia NVARCHAR(100) = NULL,
    @receta_Duracion NVARCHAR(50) = NULL,
    @receta_Instrucciones NVARCHAR(500) = NULL,
    @receta_FechaInicio DATE = NULL,
    @receta_FechaFin DATE = NULL,
    @receta_Estado NVARCHAR(20) = 'Activa',
    @itm_Id INT = NULL,
    @receta_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [Medico].[tbRecetas]
    (
        cita_Id, masc_Id, receta_Medicamento, tipoMed_Id, viaAdmin_Id,
        receta_Dosis, receta_Frecuencia, receta_Duracion, receta_Instrucciones,
        receta_FechaInicio, receta_FechaFin, receta_Estado, itm_Id,
        receta_UsuarioCrea, receta_FechaCrea
    )
    VALUES
    (
        @cita_Id, @masc_Id, @receta_Medicamento, @tipoMed_Id, @viaAdmin_Id,
        @receta_Dosis, @receta_Frecuencia, @receta_Duracion, @receta_Instrucciones,
        @receta_FechaInicio, @receta_FechaFin, @receta_Estado, @itm_Id,
        @receta_UsuarioCrea, GETDATE()
    )

    SELECT SCOPE_IDENTITY() AS receta_Id
END
GO

-- 4b) MedicamentosActivos: filtraba por 'Activo' (devolvia 0 filas). -> 'Activa'
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_PerfilMedico_MedicamentosActivos]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        receta.receta_Id                                AS trat_Id,
        receta.receta_Medicamento                       AS Medicamento,
        ISNULL(receta.receta_Dosis, '')
            + CASE
                WHEN receta.receta_Frecuencia IS NOT NULL
                THEN ' - ' + receta.receta_Frecuencia
                ELSE ''
              END                                       AS Dosis,
        DATEDIFF(day, GETDATE(), receta.receta_FechaFin) AS DiasRestantes,
        CASE
            WHEN receta.receta_FechaFin IS NULL OR receta.receta_FechaInicio IS NULL THEN 0
            WHEN DATEDIFF(day, receta.receta_FechaInicio, GETDATE()) <= 0            THEN 0
            ELSE CAST(
                CASE
                    WHEN (DATEDIFF(day, receta.receta_FechaInicio, GETDATE()) * 100.0)
                         / NULLIF(DATEDIFF(day, receta.receta_FechaInicio, receta.receta_FechaFin), 0) > 100
                    THEN 100
                    ELSE (DATEDIFF(day, receta.receta_FechaInicio, GETDATE()) * 100.0)
                         / NULLIF(DATEDIFF(day, receta.receta_FechaInicio, receta.receta_FechaFin), 0)
                END AS INT)
        END                                             AS PorcentajeCompletado
    FROM  [Medico].[tbRecetas] AS receta
    WHERE  receta.masc_Id          = @masc_Id
      AND  receta.receta_EsEliminado = 0
      AND  receta.receta_Estado      = 'Activa'
    ORDER BY receta.receta_FechaInicio DESC
END
GO

-- 4c) RecetasSinRevision: filtraba IN ('Activo','Activa'). -> solo 'Activa'
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_AlertaMedica_RecetasSinRevision]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        rec.receta_Id,
        rec.masc_Id,
        masc.masc_Nombre AS MascotaNombre,
        ISNULL(raza.raza_Descripcion, 'Sin raza') AS Raza,
        ISNULL(CAST(masc.masc_Edad AS NVARCHAR(10)), '?') AS Edad,
        rec.receta_Medicamento,
        rec.receta_Duracion,
        rec.receta_FechaInicio,
        rec.receta_FechaFin,
        rec.receta_Estado,
        DATEDIFF(DAY, rec.receta_FechaInicio, ISNULL(rec.receta_FechaFin, GETDATE())) AS DuracionDias
    FROM [Medico].[tbRecetas] AS rec
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON rec.masc_Id = masc.masc_Id
    LEFT JOIN [Refugio].[tbRazas] AS raza
        ON masc.raza_Id = raza.raza_Id
    WHERE rec.receta_EsEliminado = 0
      AND rec.receta_Estado = 'Activa'
      AND (rec.receta_FechaFin IS NULL OR rec.receta_FechaFin >= GETDATE())
      AND masc.masc_EsEliminado = 0
    ORDER BY rec.receta_FechaInicio DESC
END
GO

-- 4d) DashboardVeterinario alertas: NOT IN ('Finalizada','Cancelada')
--     -> set canonico de estados "cerrados".
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_DashboardVeterinario_AlertasVeterinario]
AS
BEGIN
    SET NOCOUNT ON

    -- Vacunas vencidas
    SELECT
        'red'                                       AS alert_Tipo,
        'Vacuna vencida'                            AS alert_Descripcion,
        m.masc_Nombre                               AS alert_MascotaNombre,
        v.vac_Descripcion + ' - vacuna vencida'     AS alert_Detalle,
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

    -- Vacunas proximas (<= 30 dias)
    SELECT
        'blue'                                      AS alert_Tipo,
        'Vacuna proxima'                            AS alert_Descripcion,
        m.masc_Nombre                               AS alert_MascotaNombre,
        v.vac_Descripcion + ' - vence en ' +
            CAST(DATEDIFF(DAY, GETDATE(), DATEADD(DAY,
                CASE WHEN v.vacu_PeriodoRefuerzo LIKE '%3 a%' THEN 1095
                     WHEN v.vacu_PeriodoRefuerzo LIKE '%2 a%' THEN 730
                     WHEN v.vacu_PeriodoRefuerzo LIKE '%6 m%' THEN 180
                     ELSE 365 END,
                MAX(c.cita_FechaConsulta))) AS VARCHAR) + ' dias' AS alert_Detalle,
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

    -- Recetas activas sin revision > 30 dias
    SELECT
        'orange'                                    AS alert_Tipo,
        'Receta sin revision'                       AS alert_Descripcion,
        m.masc_Nombre                               AS alert_MascotaNombre,
        r.receta_Medicamento + ' - sin control hace ' +
            CAST(DATEDIFF(DAY, r.receta_FechaInicio, GETDATE()) AS VARCHAR) + ' dias' AS alert_Detalle,
        r.receta_FechaInicio                        AS alert_FechaRef
    FROM [Medico].[tbRecetas] r
    INNER JOIN [Refugio].[tbMascotas] m ON m.masc_Id = r.masc_Id
    WHERE r.receta_EsEliminado = 0
      AND ISNULL(r.receta_Estado,'Activa') NOT IN ('Completada','Cancelada','Vencida')
      AND DATEDIFF(DAY, r.receta_FechaInicio, GETDATE()) > 30

    ORDER BY alert_Tipo, alert_FechaRef
END
GO

PRINT '68_RECETA_ESTADO_NORMALIZAR aplicado: datos migrados, DEFAULT=Activa, CHECK CK_tbRecetas_Estado y SPs normalizados.';
GO
