/*
=============================================
 Control de Vacunación — SPs
 Tablas reales:
   Refugio.tbMascotas    (masc_Id, masc_Nombre, raza_Id, refg_Id, masc_EsEliminado)
   Refugio.tbRazas       (raza_Id, raza_Descripcion, raza_TipoAnimal)
   Refugio.tbRefugios    (refg_Id, refg_Nombre)
   Refugio.tbVacunas     (vac_Id, vac_Descripcion, vacu_Especie, vacu_PeriodoRefuerzo[varchar], vac_EsEliminado)
   Refugio.tbCitaMedica  (medic_Id, masc_Id, vac_Id, medic_FechaConsulta, medic_EsEliminado)

 Notas:
   - vacu_PeriodoRefuerzo es texto ('Anual', 'Cada 3 anos', etc.) → se convierte a días
   - vacu_Especie puede ser 'Perro', 'Gato' o 'Perro/Gato'
=============================================
*/

USE PETSHOMEDB
GO

-- ============================================================
-- PR_Medico_ControlVacunacion_MatrizVacunacion
-- ============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ControlVacunacion_MatrizVacunacion]
AS
BEGIN
    SET NOCOUNT ON

    ;WITH UltimaVacuna AS (
        SELECT
            c.masc_Id,
            c.vac_Id,
            MAX(c.medic_FechaConsulta) AS ultima_fecha
        FROM [Refugio].[tbCitaMedica] c
        WHERE c.vac_Id IS NOT NULL
          AND c.medic_EsEliminado = 0
        GROUP BY c.masc_Id, c.vac_Id
    ),
    VacunasDias AS (
        SELECT
            vac_Id,
            vac_Descripcion,
            vacu_Especie,
            CASE
                WHEN vacu_PeriodoRefuerzo LIKE '%3 a%' THEN 1095
                WHEN vacu_PeriodoRefuerzo LIKE '%2 a%' THEN 730
                WHEN vacu_PeriodoRefuerzo LIKE '%6 m%' THEN 180
                ELSE 365  -- 'Anual' y cualquier otro
            END AS periodo_dias
        FROM [Refugio].[tbVacunas]
        WHERE vac_EsEliminado = 0
    )
    SELECT
        m.masc_Id,
        m.masc_Nombre,
        r.raza_TipoAnimal                        AS masc_Especie,
        r.raza_Descripcion                       AS masc_Raza,
        rf.refg_Nombre                           AS masc_Refugio,
        v.vac_Id,
        v.vac_Descripcion                        AS vac_Nombre,
        uv.ultima_fecha                          AS cvac_FechaAplicacion,
        CASE
            WHEN uv.ultima_fecha IS NULL THEN NULL
            ELSE DATEADD(DAY, v.periodo_dias, uv.ultima_fecha)
        END                                      AS cvac_FechaVencimiento,
        CASE
            WHEN uv.ultima_fecha IS NULL THEN 'red'
            WHEN DATEADD(DAY, v.periodo_dias, uv.ultima_fecha) < GETDATE() THEN 'red'
            WHEN DATEDIFF(DAY, GETDATE(), DATEADD(DAY, v.periodo_dias, uv.ultima_fecha)) <= 30 THEN 'warn'
            ELSE 'ok'
        END                                      AS cvac_Estado
    FROM [Refugio].[tbMascotas] m
    INNER JOIN [Refugio].[tbRazas]    r  ON r.raza_Id  = m.raza_Id
    INNER JOIN [Refugio].[tbRefugios] rf ON rf.refg_Id = m.refg_Id
    INNER JOIN VacunasDias v
        ON v.vacu_Especie LIKE '%' + r.raza_TipoAnimal + '%'
    LEFT  JOIN UltimaVacuna uv
        ON uv.masc_Id = m.masc_Id AND uv.vac_Id = v.vac_Id
    WHERE m.masc_EsEliminado = 0
    ORDER BY m.masc_Nombre, v.vac_Descripcion
END
GO

-- ============================================================
-- PR_Medico_ControlVacunacion_Dashboard
-- ============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ControlVacunacion_Dashboard]
AS
BEGIN
    SET NOCOUNT ON

    ;WITH UltimaVacuna AS (
        SELECT c.masc_Id, c.vac_Id, MAX(c.medic_FechaConsulta) AS ultima_fecha
        FROM [Refugio].[tbCitaMedica] c
        WHERE c.vac_Id IS NOT NULL AND c.medic_EsEliminado = 0
        GROUP BY c.masc_Id, c.vac_Id
    ),
    VacunasDias AS (
        SELECT vac_Id,
            CASE
                WHEN vacu_PeriodoRefuerzo LIKE '%3 a%' THEN 1095
                WHEN vacu_PeriodoRefuerzo LIKE '%2 a%' THEN 730
                WHEN vacu_PeriodoRefuerzo LIKE '%6 m%' THEN 180
                ELSE 365
            END AS periodo_dias,
            vacu_Especie
        FROM [Refugio].[tbVacunas] WHERE vac_EsEliminado = 0
    ),
    EstadoMascotaVacuna AS (
        SELECT m.masc_Id,
            CASE
                WHEN uv.ultima_fecha IS NULL THEN 2
                WHEN DATEADD(DAY, v.periodo_dias, uv.ultima_fecha) < GETDATE() THEN 2
                WHEN DATEDIFF(DAY, GETDATE(), DATEADD(DAY, v.periodo_dias, uv.ultima_fecha)) <= 30 THEN 1
                ELSE 0
            END AS estado
        FROM [Refugio].[tbMascotas] m
        INNER JOIN [Refugio].[tbRazas] r ON r.raza_Id = m.raza_Id
        INNER JOIN VacunasDias v ON v.vacu_Especie LIKE '%' + r.raza_TipoAnimal + '%'
        LEFT  JOIN UltimaVacuna uv ON uv.masc_Id = m.masc_Id AND uv.vac_Id = v.vac_Id
        WHERE m.masc_EsEliminado = 0
    ),
    PeorPorMascota AS (
        SELECT masc_Id, MAX(estado) AS peor FROM EstadoMascotaVacuna GROUP BY masc_Id
    )
    SELECT
        COUNT(*)                                           AS masc_Total,
        SUM(CASE WHEN peor = 0 THEN 1 ELSE 0 END)         AS masc_AlDia,
        SUM(CASE WHEN peor = 1 THEN 1 ELSE 0 END)         AS masc_PorVencer,
        SUM(CASE WHEN peor = 2 THEN 1 ELSE 0 END)         AS masc_Urgentes
    FROM PeorPorMascota
END
GO

PRINT '=========================================='
PRINT 'SPs ControlVacunacion creados OK'
PRINT '    PR_Medico_ControlVacunacion_MatrizVacunacion'
PRINT '    PR_Medico_ControlVacunacion_Dashboard'
PRINT '=========================================='
GO
