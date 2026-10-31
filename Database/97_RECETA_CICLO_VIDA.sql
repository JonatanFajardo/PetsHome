/*
=============================================
SCRIPT: 97 - Ciclo de vida de la receta
              (bitacora de eventos + transiciones + auto-vencido)
AUTOR: Sistema PetsHome
FECHA: 2026-10-30
Idempotente. Replica el concepto de prescription_events de OpenVPM
(packages/db/drizzle/0048_prescription_lifecycle.sql) adaptado a
espanol SIN acentos. Los estados ya existen en receta_Estado
('Activa' | 'Completada' | 'Vencida' | 'Cancelada'); aqui se agrega
el LOG de eventos, las acciones de transicion y el auto-vencido.
=============================================
*/
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

USE PETSHOMEDB
GO

/* =========================================================
   PARTE 1 - Tabla Medico.tbRecetaEvento (bitacora de eventos)
   ========================================================= */
IF OBJECT_ID('Medico.tbRecetaEvento', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbRecetaEvento]
    (
        evento_Id         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tbRecetaEvento PRIMARY KEY,
        receta_Id         INT NOT NULL CONSTRAINT FK_tbRecetaEvento_Receta
                              FOREIGN KEY REFERENCES [Medico].[tbRecetas](receta_Id),
        evento_Tipo       NVARCHAR(30) NOT NULL,   -- Creada|ResurtidoAutorizado|ResurtidoEntregado|Completada|Cancelada|Vencida
        evento_Nota       NVARCHAR(300) NULL,
        evento_Fecha      DATETIME NOT NULL CONSTRAINT DF_tbRecetaEvento_Fecha DEFAULT (GETDATE()),
        evento_Usuario    INT NOT NULL,
        evento_EsEliminado BIT NOT NULL CONSTRAINT DF_tbRecetaEvento_EsEliminado DEFAULT (0),
        CONSTRAINT CK_tbRecetaEvento_Tipo CHECK
            (evento_Tipo IN (N'Creada', N'ResurtidoAutorizado', N'ResurtidoEntregado',
                             N'Completada', N'Cancelada', N'Vencida'))
    )
END
GO

CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Receta_RegistrarEvento]
    @receta_Id INT,
    @tipo      NVARCHAR(30),
    @nota      NVARCHAR(300) = NULL,
    @usuario   INT
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [Medico].[tbRecetaEvento] (receta_Id, evento_Tipo, evento_Nota, evento_Usuario)
    VALUES (@receta_Id, @tipo, @nota, @usuario)
END
GO

/* =========================================================
   PARTE 2 - PR_Medico_Receta_Completar
   Pone la receta en 'Completada' (solo si esta 'Activa') y
   registra el evento 'Completada'.
   ========================================================= */
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Receta_Completar]
    @receta_Id INT,
    @usuario   INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @estado NVARCHAR(50);

        SELECT @estado = receta_Estado
        FROM [Medico].[tbRecetas]
        WHERE receta_Id = @receta_Id AND receta_EsEliminado = 0;

        IF @estado IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, N'La receta no fue encontrada.' AS MessageStatus
            RETURN
        END

        IF @estado <> N'Activa'
        BEGIN
            SELECT -1 AS CodeStatus,
                   N'Solo se puede completar una receta activa. Estado actual: ' + @estado + N'.' AS MessageStatus
            RETURN
        END

        BEGIN TRANSACTION
            UPDATE [Medico].[tbRecetas]
            SET receta_Estado        = N'Completada',
                receta_UsuarioModifica = @usuario,
                receta_FechaModifica   = GETDATE()
            WHERE receta_Id = @receta_Id;

            EXEC [Medico].[PR_Medico_Receta_RegistrarEvento]
                 @receta_Id = @receta_Id, @tipo = N'Completada', @nota = NULL, @usuario = @usuario;
        COMMIT TRANSACTION

        SELECT 1 AS CodeStatus, N'Receta marcada como completada.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

/* =========================================================
   PARTE 3 - PR_Medico_Receta_Cancelar
   Valida el motivo (>=5 caracteres), pone 'Cancelada' (solo si
   esta 'Activa') y registra el evento 'Cancelada' con nota=motivo.
   ========================================================= */
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Receta_Cancelar]
    @receta_Id INT,
    @usuario   INT,
    @motivo    NVARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF @motivo IS NULL OR LEN(LTRIM(RTRIM(@motivo))) < 5
        BEGIN
            SELECT -1 AS CodeStatus, N'Debe indicar un motivo de al menos 5 caracteres.' AS MessageStatus
            RETURN
        END

        DECLARE @estado NVARCHAR(50);

        SELECT @estado = receta_Estado
        FROM [Medico].[tbRecetas]
        WHERE receta_Id = @receta_Id AND receta_EsEliminado = 0;

        IF @estado IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, N'La receta no fue encontrada.' AS MessageStatus
            RETURN
        END

        IF @estado <> N'Activa'
        BEGIN
            SELECT -1 AS CodeStatus,
                   N'Solo se puede cancelar una receta activa. Estado actual: ' + @estado + N'.' AS MessageStatus
            RETURN
        END

        BEGIN TRANSACTION
            UPDATE [Medico].[tbRecetas]
            SET receta_Estado        = N'Cancelada',
                receta_UsuarioModifica = @usuario,
                receta_FechaModifica   = GETDATE()
            WHERE receta_Id = @receta_Id;

            EXEC [Medico].[PR_Medico_Receta_RegistrarEvento]
                 @receta_Id = @receta_Id, @tipo = N'Cancelada',
                 @nota = @motivo, @usuario = @usuario;
        COMMIT TRANSACTION

        SELECT 1 AS CodeStatus, N'Receta cancelada.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

/* =========================================================
   PARTE 4 - PR_Medico_Receta_VencerAutomatico
   Marca 'Vencida' todas las recetas 'Activa' con receta_FechaFin
   anterior a hoy, y registra un evento 'Vencida' por cada una.
   Pensado para correr periodicamente; por ahora manual.
   ========================================================= */
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Receta_VencerAutomatico]
    @usuario INT = 1
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @afectadas TABLE (receta_Id INT);

        BEGIN TRANSACTION
            UPDATE [Medico].[tbRecetas]
            SET receta_Estado        = N'Vencida',
                receta_UsuarioModifica = @usuario,
                receta_FechaModifica   = GETDATE()
            OUTPUT inserted.receta_Id INTO @afectadas(receta_Id)
            WHERE receta_EsEliminado = 0
              AND receta_Estado = N'Activa'
              AND receta_FechaFin IS NOT NULL
              AND receta_FechaFin < CAST(GETDATE() AS DATE);

            INSERT INTO [Medico].[tbRecetaEvento] (receta_Id, evento_Tipo, evento_Nota, evento_Usuario)
            SELECT receta_Id, N'Vencida', N'Vencida automaticamente por fecha de fin.', @usuario
            FROM @afectadas;
        COMMIT TRANSACTION

        DECLARE @n INT = (SELECT COUNT(1) FROM @afectadas);
        SELECT 1 AS CodeStatus,
               N'Recetas vencidas: ' + CAST(@n AS NVARCHAR(10)) + N'.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

/* =========================================================
   PARTE 5 - PR_Medico_Receta_ListEventos
   Devuelve la bitacora de una receta ordenada por fecha.
   ========================================================= */
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Receta_ListEventos]
    @receta_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ev.evento_Id,
        ev.receta_Id,
        ev.evento_Tipo,
        ev.evento_Nota,
        ev.evento_Fecha,
        ev.evento_Usuario,
        usr.usu_Nombre AS UsuarioNombre
    FROM [Medico].[tbRecetaEvento] AS ev
    LEFT JOIN [Seguridad].[tbUsuarios] AS usr
        ON ev.evento_Usuario = usr.usu_Id
    WHERE ev.receta_Id = @receta_Id
      AND ev.evento_EsEliminado = 0
    ORDER BY ev.evento_Fecha ASC, ev.evento_Id ASC
END
GO

/* =========================================================
   PARTE 6 - Registrar eventos en el alta y en el resurtido
   Se recrean los SP Insert y Resurtir (misma firma que el script 95)
   agregando el registro de eventos, para que la bitacora refleje
   todo el ciclo de vida.
   ========================================================= */

-- Insert: registra el evento 'Creada' tras insertar (conserva el SELECT del id)
IF OBJECT_ID('Medico.PR_Medico_Recetas_Insert', 'P') IS NOT NULL
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Insert]
GO
CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Insert]
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
    @receta_ResurtidosAutorizados INT = 0,
    @receta_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [Medico].[tbRecetas]
    (
        cita_Id, masc_Id, receta_Medicamento, tipoMed_Id, viaAdmin_Id,
        receta_Dosis, receta_Frecuencia, receta_Duracion, receta_Instrucciones,
        receta_FechaInicio, receta_FechaFin, receta_Estado, itm_Id,
        receta_ResurtidosAutorizados, receta_UsuarioCrea, receta_FechaCrea
    )
    VALUES
    (
        @cita_Id, @masc_Id, @receta_Medicamento, @tipoMed_Id, @viaAdmin_Id,
        @receta_Dosis, @receta_Frecuencia, @receta_Duracion, @receta_Instrucciones,
        @receta_FechaInicio, @receta_FechaFin, @receta_Estado, @itm_Id,
        ISNULL(@receta_ResurtidosAutorizados, 0), @receta_UsuarioCrea, GETDATE()
    )

    DECLARE @newId INT = CAST(SCOPE_IDENTITY() AS INT);

    EXEC [Medico].[PR_Medico_Receta_RegistrarEvento]
         @receta_Id = @newId, @tipo = N'Creada', @nota = NULL, @usuario = @receta_UsuarioCrea;

    SELECT @newId AS receta_Id
END
GO

-- Resurtir: registra el evento 'ResurtidoEntregado' dentro de la transaccion
IF OBJECT_ID('Medico.PR_Medico_Recetas_Resurtir', 'P') IS NOT NULL
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Resurtir]
GO
CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Resurtir]
    @receta_Id INT,
    @usuario   INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @auth INT, @usad INT;

        SELECT @auth = receta_ResurtidosAutorizados,
               @usad = receta_ResurtidosUsados
        FROM [Medico].[tbRecetas]
        WHERE receta_Id = @receta_Id AND receta_EsEliminado = 0;

        IF @auth IS NULL
        BEGIN
            SELECT -1 AS CodeStatus,
                   N'La receta no fue encontrada.' AS MessageStatus,
                   0 AS receta_ResurtidosUsados,
                   0 AS receta_ResurtidosAutorizados
            RETURN
        END

        IF @usad >= @auth
        BEGIN
            SELECT -1 AS CodeStatus,
                   N'No quedan resurtidos autorizados.' AS MessageStatus,
                   @usad AS receta_ResurtidosUsados,
                   @auth AS receta_ResurtidosAutorizados
            RETURN
        END

        BEGIN TRANSACTION
            UPDATE [Medico].[tbRecetas]
            SET receta_ResurtidosUsados = receta_ResurtidosUsados + 1
            WHERE receta_Id = @receta_Id;

            INSERT INTO [Medico].[tbRecetaResurtido] (receta_Id, rsrt_Usuario)
            VALUES (@receta_Id, @usuario);

            EXEC [Medico].[PR_Medico_Receta_RegistrarEvento]
                 @receta_Id = @receta_Id, @tipo = N'ResurtidoEntregado',
                 @nota = NULL, @usuario = @usuario;
        COMMIT TRANSACTION

        SELECT 1 AS CodeStatus,
               N'Resurtido registrado.' AS MessageStatus,
               (@usad + 1) AS receta_ResurtidosUsados,
               @auth AS receta_ResurtidosAutorizados
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus,
               0 AS receta_ResurtidosUsados, 0 AS receta_ResurtidosAutorizados
    END CATCH
END
GO

PRINT '=========================================='
PRINT 'Script 97: Ciclo de vida de recetas aplicado.'
PRINT '=========================================='
GO
