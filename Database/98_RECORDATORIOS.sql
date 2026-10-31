/*
=============================================
SCRIPT 98: Modulo RECORDATORIOS / RECALLS (care-reminders de OpenVPM)
AUTOR:  Sistema PetsHome
FECHA:  2026-10-30
DETALLE:
  Replica el concepto "care-reminders" de OpenVPM adaptado a este stack
  (Dapper + SP, sin EF). Un recordatorio es una TAREA INTERNA de la clinica:
  "a esta mascota le toca algo en tal fecha". NO envia correo ni SMS; solo
  encola trabajo de seguimiento para que el equipo lo atienda.

  Un recordatorio tiene:
    - mascota (masc_Id)
    - tipo   : 'VacunaRefuerzo' | 'Seguimiento' | 'Control'
    - fecha objetivo (rec_FechaObjetivo) = cuando toca
    - estado : 'Pendiente' | 'Enviado' | 'Hecho' | 'Descartado'
    - nota   (texto libre)
    - origen (de que salio, ej. 'Vacuna #12' o 'Cita #45')

  AUTO-GENERACION (PR_Medico_Recordatorios_Generar), idempotente:
    1) VacunaRefuerzo: por cada ULTIMA aplicacion de vacuna (mascota, vacuna)
       con proximo refuerzo calculable -> crea un recordatorio con
       rec_FechaObjetivo = proximo refuerzo. Reusa EXACTAMENTE la logica de
       interpretacion del periodo de refuerzo del script 87
       (PR_Medico_Vacuna_HistorialPorMascota / _AlertaMedica_VacunasProximas).
    2) Seguimiento: por cada cita con proxima cita marcada (cita_ProximaCita)
       a futuro -> crea un recordatorio con rec_FechaObjetivo = esa fecha y
       rec_Nota = cita_MotivoProximaCita.
    La pantalla filtra por ventana (vencidos / proximos / rango); por eso la
    generacion crea TODOS los refuerzos conocidos (futuros o vencidos), no
    solo los de 30 dias: el "lente" lo pone la vista.

  Idempotencia: indice unico filtrado por (masc_Id, rec_Tipo, rec_Origen).
  Re-ejecutar no duplica; actualiza la fecha de los que siguen 'Pendiente'.

  Aditivo: no reemplaza nada. Convive con tbCitaMedica, tbVacunas,
  tbListaEspera y las alertas de vacunas existentes (no las toca).

  Schema: Medico. Es trabajo CLINICO (refuerzos de vacuna / seguimientos),
  vive junto a tbCitaMedica / tbListaEspera y sus fuentes son clinicas; por
  eso va en Medico y no en General.

  Estados y tipos SIN tildes en BD; la UI los muestra con tilde.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE PETSHOMEDB
GO

-- =====================================================
-- TABLA: Medico.tbRecordatorios
-- =====================================================
IF OBJECT_ID('Medico.tbRecordatorios', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbRecordatorios]
    (
        rec_Id               INT IDENTITY(1,1) NOT NULL,
        masc_Id              INT            NOT NULL,                 -- mascota a la que le toca algo
        rec_Tipo             NVARCHAR(30)   NOT NULL,                 -- VacunaRefuerzo|Seguimiento|Control
        rec_FechaObjetivo    DATE           NOT NULL,                 -- cuando toca (due)
        rec_Estado           NVARCHAR(20)   NOT NULL DEFAULT 'Pendiente', -- Pendiente|Enviado|Hecho|Descartado
        rec_Nota             NVARCHAR(300)  NULL,
        rec_Origen           NVARCHAR(60)   NULL,                     -- de que salio (ej. 'Vacuna #12', 'Cita #45', 'Manual')
        rec_FechaCompletado  DATETIME       NULL,                     -- cuando se marco Hecho/Descartado
        rec_EsEliminado      BIT            NOT NULL DEFAULT 0,
        rec_UsuarioCrea      INT            NOT NULL,
        rec_FechaCrea        DATETIME       NOT NULL DEFAULT GETDATE(),
        rec_UsuarioModifica  INT            NULL,
        rec_FechaModifica    DATETIME       NULL,
        CONSTRAINT PK_tbRecordatorios PRIMARY KEY (rec_Id),
        CONSTRAINT FK_tbRecordatorios_Mascota FOREIGN KEY (masc_Id)         REFERENCES [Refugio].[tbMascotas](masc_Id),
        CONSTRAINT FK_tbRecordatorios_UsuCrea FOREIGN KEY (rec_UsuarioCrea) REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT CK_tbRecordatorios_Tipo   CHECK (rec_Tipo   IN ('VacunaRefuerzo','Seguimiento','Control')),
        CONSTRAINT CK_tbRecordatorios_Estado CHECK (rec_Estado IN ('Pendiente','Enviado','Hecho','Descartado'))
    )
    PRINT 'Tabla Medico.tbRecordatorios creada.'
END
ELSE
    PRINT 'La tabla Medico.tbRecordatorios ya existia, se omite.'
GO

-- Indice UNICO filtrado: evita duplicados por (mascota, tipo, origen).
-- Es la clave de la idempotencia: re-generar no crea otra fila del mismo origen.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_tbRecordatorios_Origen')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_tbRecordatorios_Origen
        ON [Medico].[tbRecordatorios] (masc_Id, rec_Tipo, rec_Origen)
        WHERE rec_EsEliminado = 0 AND rec_Origen IS NOT NULL
    PRINT 'Indice unico UX_tbRecordatorios_Origen creado.'
END
GO

-- Indice de apoyo para la cola de pendientes ordenada por fecha objetivo.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tbRecordatorios_Pendientes')
BEGIN
    CREATE NONCLUSTERED INDEX IX_tbRecordatorios_Pendientes
        ON [Medico].[tbRecordatorios] (rec_FechaObjetivo)
        INCLUDE (masc_Id, rec_Tipo, rec_Estado)
        WHERE rec_Estado = 'Pendiente' AND rec_EsEliminado = 0
    PRINT 'Indice IX_tbRecordatorios_Pendientes creado.'
END
GO

-- =====================================================
-- SP: PR_Medico_Recordatorios_Generar
--   Auto-genera recordatorios 'Pendiente' a partir de la data real, SIN
--   duplicar (idempotente). Devuelve cuantos creo.
--     @usuario  -> usuario que dispara la generacion (auditoria)
--     @Generados (SELECT final) -> # de recordatorios nuevos creados
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Recordatorios_Generar]
    @usuario INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @hoy DATE = CAST(GETDATE() AS DATE)
        DECLARE @generados INT = 0

        -- Usuario de auditoria por defecto: el admin (usu_Id mas bajo) si no vino
        IF @usuario IS NULL OR NOT EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE usu_Id = @usuario)
            SET @usuario = (SELECT MIN(usu_Id) FROM [Seguridad].[tbUsuarios])

        -- ---- Candidatos a recordatorio (tipo, mascota, fecha, origen, nota) ----
        IF OBJECT_ID('tempdb..#cand') IS NOT NULL DROP TABLE #cand
        CREATE TABLE #cand
        (
            masc_Id    INT,
            rec_Tipo   NVARCHAR(30)  COLLATE DATABASE_DEFAULT,
            fecha      DATE,
            rec_Origen NVARCHAR(60)  COLLATE DATABASE_DEFAULT,
            rec_Nota   NVARCHAR(300) COLLATE DATABASE_DEFAULT
        )

        -- ========= 1) VacunaRefuerzo: ultima aplicacion por (mascota, vacuna) =========
        ;WITH Aplicadas AS
        (
            SELECT  c.masc_Id, cv.vac_Id, c.cita_FechaConsulta AS FechaAplicada
            FROM    [Medico].[tbCitaMedica_tbVacunas] cv
            INNER JOIN [Medico].[tbCitaMedica] c ON c.cita_Id = cv.cita_Id
            WHERE   c.cita_EsEliminado = 0

            UNION

            SELECT  c.masc_Id, c.vac_Id, c.cita_FechaConsulta AS FechaAplicada
            FROM    [Medico].[tbCitaMedica] c
            WHERE   c.cita_EsEliminado = 0
              AND   c.vac_Id IS NOT NULL
        ),
        Ultima AS
        (
            SELECT masc_Id, vac_Id, MAX(FechaAplicada) AS FechaAplicada
            FROM   Aplicadas
            GROUP BY masc_Id, vac_Id
        )
        INSERT INTO #cand (masc_Id, rec_Tipo, fecha, rec_Origen, rec_Nota)
        SELECT
            u.masc_Id,
            'VacunaRefuerzo',
            CAST(pr.ProximoRefuerzo AS DATE),
            'Vacuna #' + CAST(u.vac_Id AS NVARCHAR(10)),
            LEFT(N'Refuerzo de ' + v.vac_Descripcion, 300)
        FROM Ultima u
        INNER JOIN [Refugio].[tbVacunas]  v    ON v.vac_Id   = u.vac_Id
        INNER JOIN [Refugio].[tbMascotas] masc ON masc.masc_Id = u.masc_Id
        -- --- interpretacion del periodo de refuerzo (texto NVARCHAR), igual que script 87 ---
        CROSS APPLY (SELECT p = LOWER(LTRIM(RTRIM(ISNULL(v.vacu_PeriodoRefuerzo, N''))))) np
        CROSS APPLY (SELECT firstDigit = PATINDEX('%[0-9]%', np.p)) fd
        CROSS APPLY (SELECT numStr = CASE WHEN fd.firstDigit = 0 THEN N''
                                          ELSE SUBSTRING(np.p, fd.firstDigit, 10) END) ns
        CROSS APPLY (SELECT num = CASE WHEN ns.numStr = N'' THEN NULL
                                       ELSE TRY_CONVERT(INT, LEFT(ns.numStr,
                                            PATINDEX('%[^0-9]%', ns.numStr + 'x') - 1)) END) nu
        CROSS APPLY (SELECT ProximoRefuerzo =
            CASE
                WHEN np.p = N''                                                       THEN NULL
                WHEN np.p LIKE N'%anual%'                                             THEN DATEADD(MONTH, 12, u.FechaAplicada)
                WHEN np.p LIKE N'%semestral%'                                         THEN DATEADD(MONTH, 6,  u.FechaAplicada)
                WHEN np.p LIKE N'%trimestral%'                                        THEN DATEADD(MONTH, 3,  u.FechaAplicada)
                WHEN np.p LIKE N'%bimestral%'                                         THEN DATEADD(MONTH, 2,  u.FechaAplicada)
                WHEN np.p LIKE N'%mensual%'                                           THEN DATEADD(MONTH, 1,  u.FechaAplicada)
                WHEN (np.p LIKE N'%año%' OR np.p LIKE N'%ano%') AND nu.num IS NOT NULL THEN DATEADD(YEAR,  nu.num, u.FechaAplicada)
                WHEN np.p LIKE N'%semana%' AND nu.num IS NOT NULL                     THEN DATEADD(WEEK,  nu.num, u.FechaAplicada)
                WHEN np.p LIKE N'%mes%'    AND nu.num IS NOT NULL                     THEN DATEADD(MONTH, nu.num, u.FechaAplicada)
                WHEN (np.p LIKE N'%día%' OR np.p LIKE N'%dia%') AND nu.num IS NOT NULL THEN DATEADD(DAY,   nu.num, u.FechaAplicada)
                ELSE NULL
            END) pr
        WHERE masc.masc_EsEliminado = 0
          AND v.vac_EsEliminado     = 0
          AND pr.ProximoRefuerzo IS NOT NULL

        -- ========= 2) Seguimiento: proxima cita marcada a futuro =========
        INSERT INTO #cand (masc_Id, rec_Tipo, fecha, rec_Origen, rec_Nota)
        SELECT
            c.masc_Id,
            'Seguimiento',
            CAST(c.cita_ProximaCita AS DATE),
            'Cita #' + CAST(c.cita_Id AS NVARCHAR(10)),
            LEFT(ISNULL(NULLIF(LTRIM(RTRIM(c.cita_MotivoProximaCita)), ''), N'Seguimiento de consulta'), 300)
        FROM [Medico].[tbCitaMedica] c
        INNER JOIN [Refugio].[tbMascotas] masc ON masc.masc_Id = c.masc_Id
        WHERE c.cita_EsEliminado = 0
          AND masc.masc_EsEliminado = 0
          AND c.cita_ProximaCita IS NOT NULL
          AND CAST(c.cita_ProximaCita AS DATE) >= @hoy

        -- ---- Actualiza la fecha de los que ya existen y siguen 'Pendiente' ----
        UPDATE r
        SET r.rec_FechaObjetivo   = c.fecha,
            r.rec_Nota            = c.rec_Nota,
            r.rec_UsuarioModifica = @usuario,
            r.rec_FechaModifica   = GETDATE()
        FROM [Medico].[tbRecordatorios] r
        INNER JOIN #cand c
               ON c.masc_Id    = r.masc_Id
              AND c.rec_Tipo   = r.rec_Tipo
              AND c.rec_Origen = r.rec_Origen
        WHERE r.rec_EsEliminado = 0
          AND r.rec_Estado = 'Pendiente'
          AND r.rec_FechaObjetivo <> c.fecha

        -- ---- Inserta los que aun no existen (idempotente por el indice unico) ----
        INSERT INTO [Medico].[tbRecordatorios]
            (masc_Id, rec_Tipo, rec_FechaObjetivo, rec_Estado, rec_Nota, rec_Origen, rec_UsuarioCrea, rec_FechaCrea)
        SELECT
            c.masc_Id, c.rec_Tipo, c.fecha, 'Pendiente', c.rec_Nota, c.rec_Origen, @usuario, GETDATE()
        FROM #cand c
        WHERE NOT EXISTS (
            SELECT 1 FROM [Medico].[tbRecordatorios] r
            WHERE r.masc_Id    = c.masc_Id
              AND r.rec_Tipo   = c.rec_Tipo
              AND r.rec_Origen = c.rec_Origen
              AND r.rec_EsEliminado = 0
        )

        SET @generados = @@ROWCOUNT

        IF OBJECT_ID('tempdb..#cand') IS NOT NULL DROP TABLE #cand

        SELECT 1 AS CodeStatus,
               N'Generados ' + CAST(@generados AS NVARCHAR(10)) + N' recordatorio(s) nuevo(s).' AS MessageStatus,
               @generados AS Generados
    END TRY
    BEGIN CATCH
        IF OBJECT_ID('tempdb..#cand') IS NOT NULL DROP TABLE #cand
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus, 0 AS Generados
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Recordatorios_List
--   Lista recordatorios con filtros opcionales:
--     @estado  : NULL/''=todos, o Pendiente|Enviado|Hecho|Descartado
--     @desde   : rec_FechaObjetivo >= @desde
--     @hasta   : rec_FechaObjetivo <= @hasta
--     @masc_Id : filtra por mascota
--   Orden: pendientes primero, luego por fecha objetivo ascendente.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Recordatorios_List]
    @estado  NVARCHAR(20) = NULL,
    @desde   DATE = NULL,
    @hasta   DATE = NULL,
    @masc_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON
    DECLARE @hoy DATE = CAST(GETDATE() AS DATE)

    SELECT
        rec.rec_Id,
        rec.masc_Id,
        masc.masc_Nombre            AS Mascota,
        masc.cli_Id,
        CASE WHEN masc.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(cli.cli_Nombres + ' ' + ISNULL(cli.cli_Apellidos, '')))
             ELSE 'Interno (refugio)' END AS Responsable,
        CASE WHEN masc.cli_Id IS NOT NULL THEN cli.cli_Telefono ELSE NULL END AS Telefono,
        rec.rec_Tipo,
        rec.rec_FechaObjetivo,
        rec.rec_Estado,
        rec.rec_Nota,
        rec.rec_Origen,
        rec.rec_FechaCompletado,
        DATEDIFF(DAY, @hoy, rec.rec_FechaObjetivo) AS DiasRestantes,
        CASE WHEN rec.rec_Estado = 'Pendiente' AND rec.rec_FechaObjetivo < @hoy
             THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS EsVencido,
        rec.rec_FechaCrea
    FROM [Medico].[tbRecordatorios] AS rec
    INNER JOIN [Refugio].[tbMascotas]    AS masc ON rec.masc_Id = masc.masc_Id
    LEFT  JOIN [Veterinaria].[tbClientes] AS cli ON masc.cli_Id = cli.cli_Id
    WHERE rec.rec_EsEliminado = 0
      AND (@estado  IS NULL OR @estado = '' OR rec.rec_Estado = @estado)
      AND (@desde   IS NULL OR rec.rec_FechaObjetivo >= @desde)
      AND (@hasta   IS NULL OR rec.rec_FechaObjetivo <= @hasta)
      AND (@masc_Id IS NULL OR rec.masc_Id = @masc_Id)
    ORDER BY
        CASE rec.rec_Estado WHEN 'Pendiente' THEN 0 WHEN 'Enviado' THEN 1 ELSE 2 END,
        rec.rec_FechaObjetivo ASC
END
GO

-- =====================================================
-- SP: PR_Medico_Recordatorios_Detail
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Recordatorios_Detail]
    @rec_Id INT
AS
BEGIN
    SET NOCOUNT ON
    DECLARE @hoy DATE = CAST(GETDATE() AS DATE)

    SELECT
        rec.rec_Id,
        rec.masc_Id,
        masc.masc_Nombre            AS Mascota,
        masc.cli_Id,
        CASE WHEN masc.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(cli.cli_Nombres + ' ' + ISNULL(cli.cli_Apellidos, '')))
             ELSE 'Interno (refugio)' END AS Responsable,
        CASE WHEN masc.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(ISNULL(cli.cli_Telefono, '') +
                  CASE WHEN cli.cli_Correo IS NOT NULL THEN ' · ' + cli.cli_Correo ELSE '' END))
             ELSE NULL END AS ResponsableContacto,
        rec.rec_Tipo,
        rec.rec_FechaObjetivo,
        rec.rec_Estado,
        rec.rec_Nota,
        rec.rec_Origen,
        rec.rec_FechaCompletado,
        DATEDIFF(DAY, @hoy, rec.rec_FechaObjetivo) AS DiasRestantes,
        usuCrea.usu_Nombre AS UsuarioCreacion, rec.rec_FechaCrea,
        usuMod.usu_Nombre  AS UsuarioModificacion, rec.rec_FechaModifica
    FROM [Medico].[tbRecordatorios] AS rec
    INNER JOIN [Refugio].[tbMascotas]    AS masc    ON rec.masc_Id = masc.masc_Id
    LEFT  JOIN [Veterinaria].[tbClientes] AS cli    ON masc.cli_Id = cli.cli_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]  AS usuCrea ON rec.rec_UsuarioCrea = usuCrea.usu_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]  AS usuMod  ON rec.rec_UsuarioModifica = usuMod.usu_Id
    WHERE rec.rec_Id = @rec_Id AND rec.rec_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_Recordatorios_MarcarHecho
--   Pendiente/Enviado -> Hecho. No se puede re-marcar un Descartado.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Recordatorios_MarcarHecho]
    @rec_Id  INT,
    @usuario INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @estadoActual NVARCHAR(20) = (SELECT rec_Estado FROM [Medico].[tbRecordatorios]
                                              WHERE rec_Id = @rec_Id AND rec_EsEliminado = 0)
        IF @estadoActual IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, 'El recordatorio no existe.' AS MessageStatus
            RETURN
        END
        IF @estadoActual = 'Descartado'
        BEGIN
            SELECT -2 AS CodeStatus, 'Un recordatorio descartado no puede marcarse como hecho.' AS MessageStatus
            RETURN
        END
        IF @estadoActual = 'Hecho'
        BEGIN
            SELECT 1 AS CodeStatus, 'El recordatorio ya estaba hecho.' AS MessageStatus
            RETURN
        END

        UPDATE [Medico].[tbRecordatorios]
        SET rec_Estado = 'Hecho',
            rec_FechaCompletado = GETDATE(),
            rec_UsuarioModifica = @usuario,
            rec_FechaModifica = GETDATE()
        WHERE rec_Id = @rec_Id AND rec_EsEliminado = 0

        SELECT 1 AS CodeStatus, 'Recordatorio marcado como hecho.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Recordatorios_Descartar
--   Pendiente/Enviado -> Descartado (ya no aplica). Opcionalmente una nota.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Recordatorios_Descartar]
    @rec_Id  INT,
    @usuario INT,
    @nota    NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @estadoActual NVARCHAR(20) = (SELECT rec_Estado FROM [Medico].[tbRecordatorios]
                                              WHERE rec_Id = @rec_Id AND rec_EsEliminado = 0)
        IF @estadoActual IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, 'El recordatorio no existe.' AS MessageStatus
            RETURN
        END
        IF @estadoActual = 'Hecho'
        BEGIN
            SELECT -2 AS CodeStatus, 'Un recordatorio ya hecho no puede descartarse.' AS MessageStatus
            RETURN
        END
        IF @estadoActual = 'Descartado'
        BEGIN
            SELECT 1 AS CodeStatus, 'El recordatorio ya estaba descartado.' AS MessageStatus
            RETURN
        END

        UPDATE [Medico].[tbRecordatorios]
        SET rec_Estado = 'Descartado',
            rec_FechaCompletado = GETDATE(),
            rec_Nota = ISNULL(NULLIF(LTRIM(RTRIM(@nota)), ''), rec_Nota),
            rec_UsuarioModifica = @usuario,
            rec_FechaModifica = GETDATE()
        WHERE rec_Id = @rec_Id AND rec_EsEliminado = 0

        SELECT 1 AS CodeStatus, 'Recordatorio descartado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Recordatorios_MarcarEnviado (opcional)
--   Pendiente -> Enviado (se aviso al responsable; sigue pendiente de hacerse).
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Recordatorios_MarcarEnviado]
    @rec_Id  INT,
    @usuario INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @estadoActual NVARCHAR(20) = (SELECT rec_Estado FROM [Medico].[tbRecordatorios]
                                              WHERE rec_Id = @rec_Id AND rec_EsEliminado = 0)
        IF @estadoActual IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, 'El recordatorio no existe.' AS MessageStatus
            RETURN
        END
        IF @estadoActual <> 'Pendiente'
        BEGIN
            SELECT -2 AS CodeStatus, 'Solo un recordatorio pendiente puede marcarse como enviado.' AS MessageStatus
            RETURN
        END

        UPDATE [Medico].[tbRecordatorios]
        SET rec_Estado = 'Enviado',
            rec_UsuarioModifica = @usuario,
            rec_FechaModifica = GETDATE()
        WHERE rec_Id = @rec_Id AND rec_EsEliminado = 0

        SELECT 1 AS CodeStatus, 'Recordatorio marcado como enviado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- PANTALLA + PERMISOS (grupo clinico, modu_Id 3 como el resto de Clinica)
-- Matriz rol x funcion:
--   Administrador (1): Consultar, Insertar, Editar, Eliminar
--   Veterinario   (4): Consultar, Insertar, Editar            (gestiona sus recalls; no elimina)
--   Recepcionista (7): Consultar, Insertar, Editar            (ayuda a llamar/seguir)
-- El permiso tambien puede re-asignarse con FIX_asignar_pantallas_admin.sql + re-login.
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Recordatorios')
BEGIN
    INSERT INTO [Seguridad].[tbPantallas] (pan_Descripcion, pan_Grupo, pan_EsActivo, modu_Id)
    VALUES ('Recordatorios', 'Medicamento', 1, 3)
    PRINT 'Pantalla creada: Recordatorios'
END
ELSE
    PRINT 'La pantalla Recordatorios ya existia, se omite el INSERT'
GO

DECLARE @pan INT = (SELECT pan_Id FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Recordatorios')

INSERT INTO [Seguridad].[tbRolesPantallas]
    (rol_Id, pan_Id, ropan_EsActivo, ropan_Consultar, ropan_Insertar, ropan_Editar, ropan_Eliminar)
SELECT r.Rol_Id, @pan, 0, 0, 0, 0, 0
FROM [Seguridad].[tbRoles] r
WHERE r.Rol_Id IN (1, 4, 7)
  AND NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesPantallas] rp WHERE rp.rol_Id = r.Rol_Id AND rp.pan_Id = @pan)

-- Administrador: todo
UPDATE [Seguridad].[tbRolesPantallas]
SET ropan_EsActivo = 1, ropan_Consultar = 1, ropan_Insertar = 1, ropan_Editar = 1, ropan_Eliminar = 1
WHERE pan_Id = @pan AND rol_Id = 1

-- Veterinario y Recepcionista: consultar/insertar/editar (gestionan, no eliminan)
UPDATE [Seguridad].[tbRolesPantallas]
SET ropan_EsActivo = 1, ropan_Consultar = 1, ropan_Insertar = 1, ropan_Editar = 1, ropan_Eliminar = 0
WHERE pan_Id = @pan AND rol_Id IN (4, 7)

PRINT 'Matriz de permisos aplicada para Recordatorios'
GO

SELECT r.Rol_Descripcion, rp.ropan_Consultar, rp.ropan_Insertar, rp.ropan_Editar, rp.ropan_Eliminar
FROM [Seguridad].[tbRolesPantallas] rp
JOIN [Seguridad].[tbRoles] r ON rp.rol_Id = r.Rol_Id
JOIN [Seguridad].[tbPantallas] p ON rp.pan_Id = p.pan_Id
WHERE p.pan_Descripcion = 'Recordatorios'
ORDER BY r.Rol_Id
GO

PRINT 'Modulo RECORDATORIOS (script 98): tabla, indices, SPs y pantalla creados correctamente.'
GO
