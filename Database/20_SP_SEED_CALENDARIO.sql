/*
==============================================
SCRIPT: SP Generador Automático de Citas para Calendario
AUTOR: PetsHome
FECHA: 2026-04-25
Descripción:
  Genera citas médicas para el mes anterior, el actual
  y el siguiente SOLO si el mes actual no tiene citas.
  Primero borra todas las citas (y sus dependencias),
  luego inserta datos variados y realistas.

Ejecutar ANTES de este script:
  - 12_SP_CITA_MEDICA.sql     (SPs de tbCitaMedica)
  - 13_SP_CITA_MEDICA_CALENDARIO.sql (SP del calendario)
  - 15_LIMPIAR_Y_POBLAR_DATOS_PORTAFOLIO.sql (datos base)
==============================================
*/

USE PETSHOMEDB
GO

-- =============================================
-- SP: PR_Medico_CitaMedica_SeedCalendario
-- =============================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'PR_Medico_CitaMedica_SeedCalendario')
    DROP PROCEDURE [Medico].[PR_Medico_CitaMedica_SeedCalendario]
GO

CREATE PROCEDURE [Medico].[PR_Medico_CitaMedica_SeedCalendario]
AS
BEGIN
    SET NOCOUNT ON;

    -- ─────────────────────────────────────────────────
    -- Si el mes actual ya tiene citas activas → salir
    -- ─────────────────────────────────────────────────
    IF EXISTS (
        SELECT 1 FROM [Medico].[tbCitaMedica]
        WHERE cita_EsEliminado = 0
          AND YEAR (cita_FechaConsulta) = YEAR (GETDATE())
          AND MONTH(cita_FechaConsulta) = MONTH(GETDATE())
    )
        RETURN;

    -- ─────────────────────────────────────────────────
    -- 1. Limpiar dependencias y luego tbCitaMedica
    -- ─────────────────────────────────────────────────
    DELETE FROM [Medico].[tbCitaMedica_tbVacunas];
    DELETE FROM [Medico].[tbTratamientos];
    DELETE FROM [Medico].[tbRecetas];
    DELETE FROM [Medico].[tbCitaMedica];

    -- ─────────────────────────────────────────────────
    -- 2. Catálogos de referencia en variables de tabla
    -- ─────────────────────────────────────────────────

    -- Mascotas activas
    DECLARE @Masc TABLE (rn INT IDENTITY(1,1), masc_Id INT);
    INSERT INTO @Masc SELECT masc_Id
    FROM [Refugio].[tbMascotas]
    WHERE masc_EsEliminado = 0
    ORDER BY masc_Id;

    DECLARE @NMasc INT = (SELECT COUNT(*) FROM @Masc);
    IF @NMasc = 0 RETURN;   -- sin mascotas, nada que hacer

    -- Vacunas activas
    DECLARE @Vac TABLE (rn INT IDENTITY(1,1), vac_Id INT);
    INSERT INTO @Vac SELECT vac_Id
    FROM [Refugio].[tbVacunas]
    WHERE vac_EsActivo = 1 AND vac_EsEliminado = 0;
    DECLARE @NVac INT = (SELECT COUNT(*) FROM @Vac);

    -- Horas de consulta (sin horario de almuerzo 12-13)
    DECLARE @Hora TABLE (rn INT IDENTITY(1,1), h INT, m INT);
    INSERT INTO @Hora (h, m) VALUES
    (8,0),(8,30),(9,0),(9,30),(10,0),(10,30),
    (11,0),(11,30),(14,0),(14,30),(15,0),(15,30),(16,0);
    DECLARE @NHora INT = (SELECT COUNT(*) FROM @Hora);

    -- Motivos por tipo de consulta (tipoCon_Id 1-7)
    DECLARE @Mot TABLE (
        rn         INT IDENTITY(1,1),
        tipoCon_Id INT,
        motivo     NVARCHAR(200),
        diag       NVARCHAR(200),
        proc       NVARCHAR(200)
    );
    INSERT INTO @Mot (tipoCon_Id, motivo, diag, proc) VALUES
    -- 1 = General
    (1, N'Revisión general de salud',
        N'Estado general bueno, sin hallazgos relevantes',
        N'Examen físico completo, auscultación'),
    (1, N'Pérdida leve de apetito',
        N'Estrés ambiental, sin patología orgánica detectada',
        N'Evaluación conductual y física'),
    (1, N'Irritación cutánea en abdomen',
        N'Dermatitis alérgica leve de contacto',
        N'Examen dermatológico, crema tópica antiinflamatoria'),
    (1, N'Secreción ocular bilateral',
        N'Conjuntivitis leve de origen infeccioso',
        N'Examen oftalmológico, colirio antibiótico'),
    -- 2 = Emergencia
    (2, N'Vómitos y diarrea persistentes',
        N'Gastroenteritis aguda por ingesta inapropiada',
        N'Hidratación IV, antieméticos, dieta blanda 48h'),
    (2, N'Trauma por caída desde altura',
        N'Contusión leve, sin fractura detectada en radiografía',
        N'Radiografía, observación 24h, analgésico'),
    (2, N'Herida por pelea con otro animal',
        N'Laceraciones múltiples tratadas con éxito',
        N'Limpieza de heridas, sutura, antibiótico preventivo'),
    (2, N'Convulsión - episodio agudo',
        N'Epilepsia idiopática, primer episodio documentado',
        N'Estabilización, Diazepam IV, evaluación neurológica'),
    -- 3 = Seguimiento
    (3, N'Control de peso mensual',
        N'Reducción de peso progresiva, dieta efectiva',
        N'Pesaje, evaluación de dieta, ajuste de raciones'),
    (3, N'Seguimiento de herida post-sutura',
        N'Cicatrización en curso, sin signos de infección',
        N'Revisión de herida, cambio de apósito'),
    (3, N'Control post-vacunación',
        N'Sin reacciones adversas tardías, buen estado general',
        N'Evaluación física, toma de temperatura'),
    -- 4 = Pre-quirúrgico
    (4, N'Evaluación pre-quirúrgica para esterilización',
        N'Apto para cirugía, parámetros dentro de lo normal',
        N'Hemograma, coagulación, bioquímica, examen físico'),
    (4, N'Evaluación pre-quirúrgica - masa cutánea',
        N'Masa benigna confirmada por PAAF, indicada extirpación',
        N'PAAF, análisis de sangre, coagulación'),
    -- 5 = Post-quirúrgico
    (5, N'Control post-operatorio día 7',
        N'Recuperación satisfactoria, sin complicaciones',
        N'Revisión de sutura, evaluación de dolor, analgésico'),
    (5, N'Retiro de puntos post-cirugía',
        N'Herida quirúrgica sana, cicatrización completa',
        N'Retiro de puntos, limpieza final de zona operada'),
    -- 6 = Vacunación
    (6, N'Vacunación anual programada',
        N'Apto para vacunación, buen estado general',
        N'Aplicación de vacuna, registro en cartilla'),
    (6, N'Refuerzo de vacuna semestral',
        N'Sin reacciones previas registradas, apto para refuerzo',
        N'Aplicación de refuerzo, observación 15 min'),
    -- 7 = Chequeo de rutina
    (7, N'Chequeo de rutina mensual',
        N'Estado general bueno, peso estable en rango ideal',
        N'Examen físico completo, pesaje, revisión de encías'),
    (7, N'Chequeo de rutina semestral',
        N'Excelente estado general, sin anomalías detectadas',
        N'Examen completo, revisión dental y auditiva');

    -- Cuenta de motivos por tipo (para módulo seguro)
    DECLARE @NM1 INT = (SELECT COUNT(*) FROM @Mot WHERE tipoCon_Id = 1); -- 4
    DECLARE @NM2 INT = (SELECT COUNT(*) FROM @Mot WHERE tipoCon_Id = 2); -- 4
    DECLARE @NM3 INT = (SELECT COUNT(*) FROM @Mot WHERE tipoCon_Id = 3); -- 3
    DECLARE @NM4 INT = (SELECT COUNT(*) FROM @Mot WHERE tipoCon_Id = 4); -- 2
    DECLARE @NM5 INT = (SELECT COUNT(*) FROM @Mot WHERE tipoCon_Id = 5); -- 2
    DECLARE @NM6 INT = (SELECT COUNT(*) FROM @Mot WHERE tipoCon_Id = 6); -- 2
    DECLARE @NM7 INT = (SELECT COUNT(*) FROM @Mot WHERE tipoCon_Id = 7); -- 2

    -- ─────────────────────────────────────────────────
    -- 3. Rango de fechas: inicio mes anterior → fin mes siguiente
    -- ─────────────────────────────────────────────────
    DECLARE @Hoy      DATE = CAST(GETDATE() AS DATE);
    DECLARE @Inicio   DATE = DATEADD(MONTH, -1,
                              DATEFROMPARTS(YEAR(@Hoy), MONTH(@Hoy), 1));
    DECLARE @Fin      DATE = EOMONTH(DATEADD(MONTH, 1, @Hoy));
    DECLARE @TotDias  INT  = DATEDIFF(DAY, @Inicio, @Fin) + 1;

    -- ─────────────────────────────────────────────────
    -- 4. Generación de citas (WHILE por día y slot)
    -- ─────────────────────────────────────────────────
    DECLARE @i       INT = 0;     -- offset de día
    DECLARE @s       INT;         -- slot dentro del día
    DECLARE @slots   INT;         -- cuántas citas genera este día
    DECLARE @fecha   DATE;
    DECLARE @dow     INT;         -- día de semana (1=Dom … 7=Sáb, DATEFIRST=7)
    DECLARE @seed    INT;         -- número determinístico por iteración

    DECLARE @tipoCon  INT;
    DECLARE @mascId   INT;
    DECLARE @horaIdx  INT;
    DECLARE @horaH    INT;
    DECLARE @horaM    INT;
    DECLARE @motivo   NVARCHAR(200);
    DECLARE @diag     NVARCHAR(200);
    DECLARE @proc     NVARCHAR(200);
    DECLARE @grav     INT;
    DECLARE @vacId    INT;
    DECLARE @comId    INT;
    DECLARE @nmTipo   INT;
    DECLARE @motivoSq INT;

    WHILE @i < @TotDias
    BEGIN
        SET @fecha = DATEADD(DAY, @i, @Inicio);
        SET @dow   = DATEPART(WEEKDAY, @fecha); -- 1=Dom, 7=Sáb (DATEFIRST=7 default)

        -- Número de citas según día de la semana
        SET @slots = CASE
            WHEN @dow = 1           THEN 0              -- Domingo: sin citas
            WHEN @dow = 7           THEN (@i % 2)       -- Sábado:  0 ó 1
            ELSE 2 + (@i % 3)                           -- L-V:     2, 3 ó 4
        END;

        SET @s = 0;
        WHILE @s < @slots
        BEGIN
            -- Seed determinístico con buena dispersión
            SET @seed = ABS(@i * 1009 + @s * 503 + 7);

            -- ── Mascota ──────────────────────────────
            SET @mascId = (SELECT masc_Id FROM @Masc
                           WHERE rn = (@seed % @NMasc) + 1);

            -- ── Tipo de consulta ─────────────────────
            --    Distribución: ~35% General, 17% Vacunacion,
            --    17% Chequeo, 9% Emergencia, 9% Seguimiento,
            --    7% Pre/Post-quirúrgico
            SET @tipoCon = CASE (@seed % 12)
                WHEN 0  THEN 2   -- Emergencia
                WHEN 1  THEN 6   -- Vacunacion
                WHEN 2  THEN 7   -- Chequeo rutina
                WHEN 3  THEN 3   -- Seguimiento
                WHEN 4  THEN 6   -- Vacunacion
                WHEN 5  THEN 7   -- Chequeo rutina
                WHEN 6  THEN 4   -- Pre-quirurgico
                WHEN 7  THEN 5   -- Post-quirurgico
                ELSE    1        -- General (casos 8-11 = 33%)
            END;
            -- Sábados: solo emergencias o generales
            IF @dow = 7 SET @tipoCon = CASE (@seed % 2) WHEN 0 THEN 2 ELSE 1 END;

            -- ── Hora ─────────────────────────────────
            --    Cada slot del mismo día toma un índice distinto
            SET @horaIdx = ((@seed + @s * 3) % @NHora) + 1;
            SELECT @horaH = h, @horaM = m
            FROM @Hora WHERE rn = @horaIdx;

            -- ── Gravedad ─────────────────────────────
            SET @grav = CASE
                WHEN @tipoCon = 2 THEN        -- Emergencias: pueden ser 1-3
                    CASE (@seed % 3) WHEN 0 THEN 3 WHEN 1 THEN 2 ELSE 1 END
                ELSE 1
            END;

            -- ── Vacuna (solo tipo 6) ──────────────────
            SET @vacId = NULL;
            IF @tipoCon = 6 AND @NVac > 0
                SET @vacId = (SELECT vac_Id FROM @Vac
                              WHERE rn = (@seed % @NVac) + 1);

            -- ── Comportamiento (1-10) ─────────────────
            SET @comId = (@seed % 10) + 1;

            -- ── Motivo / diagnóstico / procedimiento ──
            SET @nmTipo = CASE @tipoCon
                WHEN 1 THEN @NM1  WHEN 2 THEN @NM2
                WHEN 3 THEN @NM3  WHEN 4 THEN @NM4
                WHEN 5 THEN @NM5  WHEN 6 THEN @NM6
                ELSE        @NM7
            END;
            SET @motivoSq = (@seed % @nmTipo) + 1;

            SELECT @motivo = motivo, @diag = diag, @proc = proc
            FROM (
                SELECT motivo, diag, proc,
                       ROW_NUMBER() OVER (ORDER BY rn) AS seq
                FROM @Mot
                WHERE tipoCon_Id = @tipoCon
            ) x
            WHERE seq = @motivoSq;

            -- ── Insertar cita ─────────────────────────
            INSERT INTO [Medico].[tbCitaMedica]
                (masc_Id, cita_FechaConsulta, tipoCon_Id,
                 cita_MotivoConsulta, cita_Diagnostico,
                 grav_Id, cita_Peso, cita_Temperatura,
                 cita_FrecuenciaCardiaca, cita_FrecuenciaRespiratoria,
                 com_Id, vac_Id,
                 cita_ProcedimientosRealizados, cita_ResultadosExamenes,
                 cita_ProximaCita, cita_MotivoProximaCita,
                 cita_EsEliminado, cita_UsuarioCrea, cita_FechaCrea)
            VALUES (
                @mascId,
                DATEADD(MINUTE, @horaM, DATEADD(HOUR, @horaH, CAST(@fecha AS DATETIME))),
                @tipoCon,
                @motivo, @diag,
                @grav, NULL, NULL, NULL, NULL,
                @comId, @vacId,
                @proc, NULL,
                NULL, NULL,
                0, 1, @fecha
            );

            SET @s = @s + 1;
        END; -- slots

        SET @i = @i + 1;
    END; -- días

END
GO

PRINT '=========================================='
PRINT 'SP CREADO: PR_Medico_CitaMedica_SeedCalendario'
PRINT '  - Genera 3 meses de citas si el mes actual'
PRINT '    no tiene datos'
PRINT '  - Llámalo desde el controller al entrar'
PRINT '    a /CitaMedica/Calendario'
PRINT '=========================================='
GO
