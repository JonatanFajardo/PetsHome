-- ============================================================
-- Script 87: Certificado de vacunacion + proximos refuerzos
-- ------------------------------------------------------------
-- Backend del modulo "Certificado de vacunacion" (PerfilMedico).
--
--   1) PR_Medico_PerfilMedico_FichaMascota (CREATE OR ALTER, ADITIVO)
--        * Reproduce el script 56 (responsable binario) y AGREGA
--          una sola columna: Especie = raza.raza_TipoAnimal.
--        * Refugio.tbMascotas NO tiene masc_Especie; la especie
--          sale del catalogo de razas (raza_TipoAnimal = 'Perro'/'Gato').
--        * Es aditivo: Dapper ignora/mapea la columna extra, no rompe
--          la vista Index de PerfilMedico.
--
--   2) PR_Medico_Vacuna_HistorialPorMascota @masc_Id
--        * Historial de vacunas aplicadas de una mascota.
--        * Une AMBAS fuentes de aplicacion (UNION, sin duplicados):
--            - Medico.tbCitaMedica_tbVacunas (multi-vacuna por cita)
--            - Medico.tbCitaMedica.vac_Id     (vacuna unica por cita)
--          La cita aporta la fecha (cita_FechaConsulta).
--        * ProximoRefuerzo = FechaAplicada + vacu_PeriodoRefuerzo.
--          vacu_PeriodoRefuerzo es NVARCHAR con texto en espanol
--          ("Anual", "Cada 3 anos", "Cada 6 meses"...). Se interpreta;
--          si no es interpretable -> ProximoRefuerzo NULL.
--        * Estado: 'Vencida' (refuerzo < hoy), 'Proxima' (<= 30 dias),
--          'Vigente' (resto o sin refuerzo).
--
--   3) PR_Medico_AlertaMedica_VacunasProximas (@masc_Id INT = NULL)
--        * Recordatorios in-app: vacunas cuyo proximo refuerzo cae
--          dentro de los proximos 30 dias o ya estan vencidas.
--        * Usa la ULTIMA aplicacion por (mascota, vacuna).
--        * Si @masc_Id viene, filtra por esa mascota.
--
-- Idempotente (CREATE OR ALTER). SET QUOTED_IDENTIFIER / ANSI_NULLS ON.
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

-- =============================================================
-- 1) Ficha de mascota: responsable binario (script 56) + Especie
-- =============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_PerfilMedico_FichaMascota]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT TOP 1
        masc.masc_Id,
        masc.masc_Nombre,
        raza.raza_Descripcion                                   AS Raza,
        CAST(masc.masc_Edad AS VARCHAR) + ' año(s)'             AS Edad,
        masc.masc_Sexo                                          AS Sexo,
        CAST(0 AS BIT)                                          AS EsEsterilizada,
        NULL                                                    AS Microchip,
        masc.masc_Peso                                          AS Peso,
        (
            SELECT TOP 1 sol2.sol_Nombres + ' ' + sol2.sol_Apellidos
            FROM   [Refugio].[tbSolicitudes]  sol2
            INNER JOIN [Refugio].[tbAdopciones] adop2
                   ON  adop2.sol_Id         = sol2.sol_Id
            WHERE  sol2.masc_Id             = masc.masc_Id
              AND  adop2.adop_EsAprobado    = 1
              AND  adop2.adop_EsEliminado   = 0
              AND  sol2.sol_EsEliminado     = 0
            ORDER BY adop2.adop_FechaCrea DESC
        )                                                       AS Adoptante,
        refg.refg_Nombre                                        AS Refugio,
        (
            SELECT MAX(c.cita_FechaConsulta)
            FROM   [Medico].[tbCitaMedica] c
            WHERE  c.masc_Id = masc.masc_Id AND c.cita_EsEliminado = 0
        )                                                       AS UltimaVisita,
        CASE
            WHEN EXISTS (
                SELECT 1 FROM [Medico].[tbTratamientos] t
                WHERE  t.masc_Id = masc.masc_Id
                  AND  t.trat_Estado    = 'Activo'
                  AND  t.trat_EsEliminado = 0
            ) THEN 'En Tratamiento'
            ELSE 'Saludable'
        END                                                     AS EstadoSalud,
        (
            SELECT COUNT(*)
            FROM   [Medico].[tbCitaMedica] c
            WHERE  c.masc_Id = masc.masc_Id AND c.cita_EsEliminado = 0
        )                                                       AS TotalCitas,
        (
            SELECT COUNT(*)
            FROM   [Medico].[tbTratamientos] t
            WHERE  t.masc_Id = masc.masc_Id
              AND  t.trat_Estado    = 'Activo'
              AND  t.trat_EsEliminado = 0
        )                                                       AS TratamientosActivos,
        (
            SELECT COUNT(*)
            FROM   [Medico].[tbCitaMedica] c
            WHERE  c.masc_Id      = masc.masc_Id
              AND  c.vac_Id       IS NOT NULL
              AND  c.cita_EsEliminado = 0
              AND  ISNULL(c.cita_ProximaCita, '2099-01-01') > GETDATE()
        )                                                       AS VacunasAlDia,
        -- ===== Responsable binario (script 56, aditivo) =====
        CASE WHEN masc.cli_Id IS NOT NULL THEN 'Cliente' ELSE 'Interno' END AS ResponsableTipo,
        CASE WHEN masc.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(cli.cli_Nombres + ' ' + ISNULL(cli.cli_Apellidos, '')))
             ELSE refg.refg_Nombre END                          AS ResponsableNombre,
        CASE WHEN masc.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(ISNULL(cli.cli_Telefono, '') +
                  CASE WHEN cli.cli_Correo IS NOT NULL THEN ' · ' + cli.cli_Correo ELSE '' END))
             ELSE NULL END                                      AS ResponsableContacto,
        -- ===== Especie (script 87, aditivo): catalogo de razas =====
        raza.raza_TipoAnimal                                    AS Especie
    FROM  [Refugio].[tbMascotas]  AS masc
    LEFT JOIN [Refugio].[tbRazas]        AS raza ON masc.raza_Id = raza.raza_Id
    LEFT JOIN [Refugio].[tbRefugios]     AS refg ON masc.refg_Id = refg.refg_Id
    LEFT JOIN [Veterinaria].[tbClientes] AS cli  ON masc.cli_Id  = cli.cli_Id
    WHERE masc.masc_Id         = @masc_Id
      AND masc.masc_EsEliminado = 0
END
GO

-- =============================================================
-- 2) Historial de vacunas por mascota (para el certificado)
-- =============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Vacuna_HistorialPorMascota]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON

    ;WITH Aplicadas AS
    (
        -- Fuente A: tabla puente multi-vacuna por cita
        SELECT  c.masc_Id,
                cv.vac_Id,
                c.cita_FechaConsulta AS FechaAplicada
        FROM    [Medico].[tbCitaMedica_tbVacunas] cv
        INNER JOIN [Medico].[tbCitaMedica] c ON c.cita_Id = cv.cita_Id
        WHERE   c.cita_EsEliminado = 0
          AND   c.masc_Id          = @masc_Id

        UNION   -- sin duplicados si una cita existe en ambas fuentes

        -- Fuente B: vacuna unica directa en la cita
        SELECT  c.masc_Id,
                c.vac_Id,
                c.cita_FechaConsulta AS FechaAplicada
        FROM    [Medico].[tbCitaMedica] c
        WHERE   c.cita_EsEliminado = 0
          AND   c.masc_Id          = @masc_Id
          AND   c.vac_Id           IS NOT NULL
    )
    SELECT
        v.vac_Descripcion           AS VacunaNombre,
        a.FechaAplicada             AS FechaAplicada,
        pr.ProximoRefuerzo          AS ProximoRefuerzo,
        CASE
            WHEN pr.ProximoRefuerzo IS NULL                                        THEN N'Vigente'
            WHEN pr.ProximoRefuerzo <  CAST(GETDATE() AS DATE)                     THEN N'Vencida'
            WHEN pr.ProximoRefuerzo <= DATEADD(DAY, 30, CAST(GETDATE() AS DATE))   THEN N'Próxima'
            ELSE N'Vigente'
        END                         AS Estado
    FROM Aplicadas a
    INNER JOIN [Refugio].[tbVacunas] v ON v.vac_Id = a.vac_Id
    -- --- interpretacion del periodo de refuerzo (texto NVARCHAR) ---
    CROSS APPLY (SELECT p = LOWER(LTRIM(RTRIM(ISNULL(v.vacu_PeriodoRefuerzo, N''))))) np
    CROSS APPLY (SELECT firstDigit = PATINDEX('%[0-9]%', np.p)) fd
    CROSS APPLY (SELECT numStr = CASE WHEN fd.firstDigit = 0 THEN N''
                                      ELSE SUBSTRING(np.p, fd.firstDigit, 10) END) ns
    CROSS APPLY (SELECT num = CASE WHEN ns.numStr = N'' THEN NULL
                                   ELSE TRY_CONVERT(INT, LEFT(ns.numStr,
                                        PATINDEX('%[^0-9]%', ns.numStr + 'x') - 1)) END) nu
    CROSS APPLY (SELECT ProximoRefuerzo =
        CASE
            WHEN np.p = N''                                                     THEN NULL
            WHEN np.p LIKE N'%anual%'                                           THEN DATEADD(MONTH, 12, a.FechaAplicada)
            WHEN np.p LIKE N'%semestral%'                                       THEN DATEADD(MONTH, 6,  a.FechaAplicada)
            WHEN np.p LIKE N'%trimestral%'                                      THEN DATEADD(MONTH, 3,  a.FechaAplicada)
            WHEN np.p LIKE N'%bimestral%'                                       THEN DATEADD(MONTH, 2,  a.FechaAplicada)
            WHEN np.p LIKE N'%mensual%'                                         THEN DATEADD(MONTH, 1,  a.FechaAplicada)
            WHEN (np.p LIKE N'%año%' OR np.p LIKE N'%ano%') AND nu.num IS NOT NULL THEN DATEADD(YEAR,  nu.num, a.FechaAplicada)
            WHEN np.p LIKE N'%semana%' AND nu.num IS NOT NULL                   THEN DATEADD(WEEK,  nu.num, a.FechaAplicada)
            WHEN np.p LIKE N'%mes%'    AND nu.num IS NOT NULL                   THEN DATEADD(MONTH, nu.num, a.FechaAplicada)
            WHEN (np.p LIKE N'%día%' OR np.p LIKE N'%dia%') AND nu.num IS NOT NULL THEN DATEADD(DAY,   nu.num, a.FechaAplicada)
            ELSE NULL
        END) pr
    WHERE v.vac_EsEliminado = 0
    ORDER BY a.FechaAplicada DESC
END
GO

-- =============================================================
-- 3) Vacunas proximas / vencidas (recordatorios in-app)
-- =============================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_AlertaMedica_VacunasProximas]
    @masc_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON

    ;WITH Aplicadas AS
    (
        SELECT  c.masc_Id, cv.vac_Id, c.cita_FechaConsulta AS FechaAplicada
        FROM    [Medico].[tbCitaMedica_tbVacunas] cv
        INNER JOIN [Medico].[tbCitaMedica] c ON c.cita_Id = cv.cita_Id
        WHERE   c.cita_EsEliminado = 0
          AND  (@masc_Id IS NULL OR c.masc_Id = @masc_Id)

        UNION

        SELECT  c.masc_Id, c.vac_Id, c.cita_FechaConsulta AS FechaAplicada
        FROM    [Medico].[tbCitaMedica] c
        WHERE   c.cita_EsEliminado = 0
          AND   c.vac_Id IS NOT NULL
          AND  (@masc_Id IS NULL OR c.masc_Id = @masc_Id)
    ),
    Ultima AS
    (
        -- Ultima aplicacion por (mascota, vacuna)
        SELECT masc_Id, vac_Id, MAX(FechaAplicada) AS FechaAplicada
        FROM   Aplicadas
        GROUP BY masc_Id, vac_Id
    )
    SELECT
        masc.masc_Nombre            AS MascotaNombre,
        u.masc_Id                   AS masc_Id,
        v.vac_Descripcion           AS VacunaNombre,
        u.FechaAplicada             AS FechaAplicada,
        pr.ProximoRefuerzo          AS ProximoRefuerzo,
        CASE
            WHEN pr.ProximoRefuerzo <  CAST(GETDATE() AS DATE)                   THEN N'Vencida'
            ELSE N'Próxima'
        END                         AS Estado
    FROM Ultima u
    INNER JOIN [Refugio].[tbVacunas]  v    ON v.vac_Id   = u.vac_Id
    INNER JOIN [Refugio].[tbMascotas] masc ON masc.masc_Id = u.masc_Id
    CROSS APPLY (SELECT p = LOWER(LTRIM(RTRIM(ISNULL(v.vacu_PeriodoRefuerzo, N''))))) np
    CROSS APPLY (SELECT firstDigit = PATINDEX('%[0-9]%', np.p)) fd
    CROSS APPLY (SELECT numStr = CASE WHEN fd.firstDigit = 0 THEN N''
                                      ELSE SUBSTRING(np.p, fd.firstDigit, 10) END) ns
    CROSS APPLY (SELECT num = CASE WHEN ns.numStr = N'' THEN NULL
                                   ELSE TRY_CONVERT(INT, LEFT(ns.numStr,
                                        PATINDEX('%[^0-9]%', ns.numStr + 'x') - 1)) END) nu
    CROSS APPLY (SELECT ProximoRefuerzo =
        CASE
            WHEN np.p = N''                                                     THEN NULL
            WHEN np.p LIKE N'%anual%'                                           THEN DATEADD(MONTH, 12, u.FechaAplicada)
            WHEN np.p LIKE N'%semestral%'                                       THEN DATEADD(MONTH, 6,  u.FechaAplicada)
            WHEN np.p LIKE N'%trimestral%'                                      THEN DATEADD(MONTH, 3,  u.FechaAplicada)
            WHEN np.p LIKE N'%bimestral%'                                       THEN DATEADD(MONTH, 2,  u.FechaAplicada)
            WHEN np.p LIKE N'%mensual%'                                         THEN DATEADD(MONTH, 1,  u.FechaAplicada)
            WHEN (np.p LIKE N'%año%' OR np.p LIKE N'%ano%') AND nu.num IS NOT NULL THEN DATEADD(YEAR,  nu.num, u.FechaAplicada)
            WHEN np.p LIKE N'%semana%' AND nu.num IS NOT NULL                   THEN DATEADD(WEEK,  nu.num, u.FechaAplicada)
            WHEN np.p LIKE N'%mes%'    AND nu.num IS NOT NULL                   THEN DATEADD(MONTH, nu.num, u.FechaAplicada)
            WHEN (np.p LIKE N'%día%' OR np.p LIKE N'%dia%') AND nu.num IS NOT NULL THEN DATEADD(DAY,   nu.num, u.FechaAplicada)
            ELSE NULL
        END) pr
    WHERE masc.masc_EsEliminado = 0
      AND v.vac_EsEliminado     = 0
      AND pr.ProximoRefuerzo IS NOT NULL
      AND pr.ProximoRefuerzo <= DATEADD(DAY, 30, CAST(GETDATE() AS DATE))
    ORDER BY pr.ProximoRefuerzo ASC
END
GO

PRINT '=========================================='
PRINT 'Script 87 completado:'
PRINT ' - PR_Medico_PerfilMedico_FichaMascota (+ Especie)'
PRINT ' - PR_Medico_Vacuna_HistorialPorMascota'
PRINT ' - PR_Medico_AlertaMedica_VacunasProximas'
PRINT '=========================================='
GO
