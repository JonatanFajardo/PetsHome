/*
=============================================================================
SCRIPT: 105_PROCEDIMIENTOS_ESTRUCTURADOS.sql
DETALLE: Convierte el texto libre de procedimientos
         (Medico.tbCitaMedica.cita_ProcedimientosRealizados) en una entidad
         estructurada Medico.tbProcedimientos, replicando el modelo de OpenVPM
         (packages/db/schema/clinical.ts tabla "procedures"):
           name, description, performedBy, anesthesiaUsed, durationMinutes, notes.
         Politica de validacion (OpenVPM procedure-policy.ts):
           nombre req <=255, descripcion <=5000, anestesia <=2000,
           notas <=5000, duracion 1..1440 min.

         COBRO: un procedimiento con serv_Id ligado es un item facturable de la
         visita (OpenVPM visit-work-items.ts). Se REUTILIZA el SP existente
         [Veterinaria].[PR_Veterinaria_CargoDispensacion_EncolarServicio]
         (no se crea cobro nuevo): el encolado lo dispara la capa C#.

         ADITIVO y backward-compatible: NO toca el texto libre existente ni el
         flujo de la consulta. Idempotente (IF OBJECT_ID / CREATE OR ALTER).
         Textos SIN acentos (evita mojibake).
=============================================================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE PETSHOMEDB
GO

-- =====================================================
-- TABLA: Medico.tbProcedimientos
-- =====================================================
IF OBJECT_ID('Medico.tbProcedimientos', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbProcedimientos]
    (
        proc_Id             INT IDENTITY(1,1) NOT NULL,
        masc_Id             INT            NOT NULL,
        cita_Id             INT            NULL,
        proc_Nombre         NVARCHAR(255)  NOT NULL,          -- OpenVPM: name (req, 255)
        proc_Descripcion    NVARCHAR(MAX)  NULL,              -- OpenVPM: description
        proc_RealizadoPor   INT            NULL,              -- OpenVPM: performedBy (usu_Id)
        proc_Anestesia      NVARCHAR(2000) NULL,              -- OpenVPM: anesthesiaUsed
        proc_DuracionMinutos INT           NULL,              -- OpenVPM: durationMinutes
        proc_Notas          NVARCHAR(MAX)  NULL,              -- OpenVPM: notes
        proc_Fecha          DATETIME       NOT NULL DEFAULT GETDATE(),
        serv_Id             INT            NULL,              -- liga al servicio facturable
        proc_EsEliminado    BIT            NOT NULL DEFAULT 0,
        proc_UsuarioCrea    INT            NOT NULL,
        proc_FechaCrea      DATETIME       NOT NULL DEFAULT GETDATE(),
        proc_UsuarioModifica INT           NULL,
        proc_FechaModifica  DATETIME       NULL,
        CONSTRAINT PK_tbProcedimientos PRIMARY KEY (proc_Id),
        CONSTRAINT CK_tbProcedimientos_Duracion
            CHECK (proc_DuracionMinutos IS NULL OR (proc_DuracionMinutos BETWEEN 1 AND 1440)),
        CONSTRAINT FK_tbProcedimientos_Mascota
            FOREIGN KEY (masc_Id)          REFERENCES [Refugio].[tbMascotas](masc_Id),
        CONSTRAINT FK_tbProcedimientos_Cita
            FOREIGN KEY (cita_Id)          REFERENCES [Medico].[tbCitaMedica](cita_Id),
        CONSTRAINT FK_tbProcedimientos_RealizadoPor
            FOREIGN KEY (proc_RealizadoPor) REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT FK_tbProcedimientos_Servicio
            FOREIGN KEY (serv_Id)          REFERENCES [Veterinaria].[tbServicios](serv_Id),
        CONSTRAINT FK_tbProcedimientos_UsuCrea
            FOREIGN KEY (proc_UsuarioCrea) REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    )
    PRINT 'Tabla Medico.tbProcedimientos creada.'
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tbProcedimientos_Mascota' AND object_id = OBJECT_ID('Medico.tbProcedimientos'))
    CREATE INDEX IX_tbProcedimientos_Mascota ON [Medico].[tbProcedimientos](masc_Id) WHERE proc_EsEliminado = 0;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tbProcedimientos_Cita' AND object_id = OBJECT_ID('Medico.tbProcedimientos'))
    CREATE INDEX IX_tbProcedimientos_Cita ON [Medico].[tbProcedimientos](cita_Id) WHERE proc_EsEliminado = 0;
GO

-- =====================================================
-- SP: PR_Medico_Procedimiento_Insert (retorna proc_Id + CodeStatus)
--   Valida contra la politica de OpenVPM (procedure-policy.ts).
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Procedimiento_Insert]
    @masc_Id             INT,
    @cita_Id             INT = NULL,
    @proc_Nombre         NVARCHAR(255),
    @proc_Descripcion    NVARCHAR(MAX)  = NULL,
    @proc_RealizadoPor   INT            = NULL,
    @proc_Anestesia      NVARCHAR(2000) = NULL,
    @proc_DuracionMinutos INT           = NULL,
    @proc_Notas          NVARCHAR(MAX)  = NULL,
    @serv_Id             INT            = NULL,
    @proc_UsuarioCrea    INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        SET @proc_Nombre = LTRIM(RTRIM(@proc_Nombre))

        IF @masc_Id IS NULL OR NOT EXISTS (SELECT 1 FROM [Refugio].[tbMascotas] WHERE masc_Id = @masc_Id)
        BEGIN
            SELECT -1 AS CodeStatus, 'La mascota indicada no existe.' AS MessageStatus, 0 AS proc_Id
            RETURN
        END
        IF @proc_Nombre IS NULL OR @proc_Nombre = ''
        BEGIN
            SELECT -1 AS CodeStatus, 'El nombre del procedimiento es obligatorio.' AS MessageStatus, 0 AS proc_Id
            RETURN
        END
        IF LEN(@proc_Nombre) > 255
        BEGIN
            SELECT -1 AS CodeStatus, 'El nombre no puede superar 255 caracteres.' AS MessageStatus, 0 AS proc_Id
            RETURN
        END
        IF @proc_Descripcion IS NOT NULL AND LEN(@proc_Descripcion) > 5000
        BEGIN
            SELECT -1 AS CodeStatus, 'La descripcion no puede superar 5000 caracteres.' AS MessageStatus, 0 AS proc_Id
            RETURN
        END
        IF @proc_Anestesia IS NOT NULL AND LEN(@proc_Anestesia) > 2000
        BEGIN
            SELECT -1 AS CodeStatus, 'La anestesia no puede superar 2000 caracteres.' AS MessageStatus, 0 AS proc_Id
            RETURN
        END
        IF @proc_Notas IS NOT NULL AND LEN(@proc_Notas) > 5000
        BEGIN
            SELECT -1 AS CodeStatus, 'Las notas no pueden superar 5000 caracteres.' AS MessageStatus, 0 AS proc_Id
            RETURN
        END
        IF @proc_DuracionMinutos IS NOT NULL AND (@proc_DuracionMinutos < 1 OR @proc_DuracionMinutos > 1440)
        BEGIN
            SELECT -1 AS CodeStatus, 'La duracion debe estar entre 1 y 1440 minutos.' AS MessageStatus, 0 AS proc_Id
            RETURN
        END

        INSERT INTO [Medico].[tbProcedimientos]
            (masc_Id, cita_Id, proc_Nombre, proc_Descripcion, proc_RealizadoPor,
             proc_Anestesia, proc_DuracionMinutos, proc_Notas, proc_Fecha, serv_Id,
             proc_UsuarioCrea, proc_FechaCrea)
        VALUES
            (@masc_Id, @cita_Id, @proc_Nombre,
             NULLIF(LTRIM(RTRIM(@proc_Descripcion)), ''), @proc_RealizadoPor,
             NULLIF(LTRIM(RTRIM(@proc_Anestesia)), ''), @proc_DuracionMinutos,
             NULLIF(LTRIM(RTRIM(@proc_Notas)), ''), GETDATE(), @serv_Id,
             @proc_UsuarioCrea, GETDATE())

        SELECT 1 AS CodeStatus, 'Procedimiento registrado.' AS MessageStatus,
               CAST(SCOPE_IDENTITY() AS INT) AS proc_Id
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus, 0 AS proc_Id
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Procedimiento_Update
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Procedimiento_Update]
    @proc_Id             INT,
    @proc_Nombre         NVARCHAR(255),
    @proc_Descripcion    NVARCHAR(MAX)  = NULL,
    @proc_RealizadoPor   INT            = NULL,
    @proc_Anestesia      NVARCHAR(2000) = NULL,
    @proc_DuracionMinutos INT           = NULL,
    @proc_Notas          NVARCHAR(MAX)  = NULL,
    @serv_Id             INT            = NULL,
    @proc_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        SET @proc_Nombre = LTRIM(RTRIM(@proc_Nombre))

        IF NOT EXISTS (SELECT 1 FROM [Medico].[tbProcedimientos] WHERE proc_Id = @proc_Id AND proc_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'El procedimiento indicado no existe.' AS MessageStatus
            RETURN
        END
        IF @proc_Nombre IS NULL OR @proc_Nombre = ''
        BEGIN
            SELECT -1 AS CodeStatus, 'El nombre del procedimiento es obligatorio.' AS MessageStatus
            RETURN
        END
        IF LEN(@proc_Nombre) > 255
        BEGIN SELECT -1 AS CodeStatus, 'El nombre no puede superar 255 caracteres.' AS MessageStatus; RETURN END
        IF @proc_Descripcion IS NOT NULL AND LEN(@proc_Descripcion) > 5000
        BEGIN SELECT -1 AS CodeStatus, 'La descripcion no puede superar 5000 caracteres.' AS MessageStatus; RETURN END
        IF @proc_Anestesia IS NOT NULL AND LEN(@proc_Anestesia) > 2000
        BEGIN SELECT -1 AS CodeStatus, 'La anestesia no puede superar 2000 caracteres.' AS MessageStatus; RETURN END
        IF @proc_Notas IS NOT NULL AND LEN(@proc_Notas) > 5000
        BEGIN SELECT -1 AS CodeStatus, 'Las notas no pueden superar 5000 caracteres.' AS MessageStatus; RETURN END
        IF @proc_DuracionMinutos IS NOT NULL AND (@proc_DuracionMinutos < 1 OR @proc_DuracionMinutos > 1440)
        BEGIN SELECT -1 AS CodeStatus, 'La duracion debe estar entre 1 y 1440 minutos.' AS MessageStatus; RETURN END

        UPDATE [Medico].[tbProcedimientos]
        SET proc_Nombre          = @proc_Nombre,
            proc_Descripcion     = NULLIF(LTRIM(RTRIM(@proc_Descripcion)), ''),
            proc_RealizadoPor    = @proc_RealizadoPor,
            proc_Anestesia       = NULLIF(LTRIM(RTRIM(@proc_Anestesia)), ''),
            proc_DuracionMinutos = @proc_DuracionMinutos,
            proc_Notas           = NULLIF(LTRIM(RTRIM(@proc_Notas)), ''),
            serv_Id              = @serv_Id,
            proc_UsuarioModifica = @proc_UsuarioModifica,
            proc_FechaModifica   = GETDATE()
        WHERE proc_Id = @proc_Id AND proc_EsEliminado = 0

        SELECT 1 AS CodeStatus, 'Procedimiento actualizado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Procedimiento_Delete (soft delete)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Procedimiento_Delete]
    @proc_Id  INT,
    @usuario  INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbProcedimientos]
        SET proc_EsEliminado = 1, proc_UsuarioModifica = @usuario, proc_FechaModifica = GETDATE()
        WHERE proc_Id = @proc_Id

        SELECT 1 AS CodeStatus, 'Procedimiento eliminado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Procedimiento_Find (para pre-llenar el form)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Procedimiento_Find]
    @proc_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        p.proc_Id, p.masc_Id, p.cita_Id, p.proc_Nombre, p.proc_Descripcion,
        p.proc_RealizadoPor, p.proc_Anestesia, p.proc_DuracionMinutos,
        p.proc_Notas, p.proc_Fecha, p.serv_Id
    FROM [Medico].[tbProcedimientos] AS p
    WHERE p.proc_Id = @proc_Id AND p.proc_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_Procedimiento_Detail (vista detalle con nombres)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Procedimiento_Detail]
    @proc_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        p.proc_Id, p.masc_Id, masc.masc_Nombre AS Mascota, p.cita_Id,
        p.proc_Nombre, p.proc_Descripcion,
        p.proc_RealizadoPor, vet.usu_Nombre AS RealizadoPorNombre,
        p.proc_Anestesia, p.proc_DuracionMinutos, p.proc_Notas, p.proc_Fecha,
        p.serv_Id, serv.serv_Nombre AS ServicioNombre, serv.serv_Precio AS ServicioPrecio,
        usuCrea.usu_Nombre AS UsuarioCreacion, p.proc_FechaCrea,
        usuMod.usu_Nombre  AS UsuarioModificacion, p.proc_FechaModifica
    FROM [Medico].[tbProcedimientos] AS p
    INNER JOIN [Refugio].[tbMascotas]      AS masc    ON p.masc_Id = masc.masc_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]    AS vet     ON p.proc_RealizadoPor = vet.usu_Id
    LEFT  JOIN [Veterinaria].[tbServicios] AS serv    ON p.serv_Id = serv.serv_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]    AS usuCrea ON p.proc_UsuarioCrea = usuCrea.usu_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]    AS usuMod  ON p.proc_UsuarioModifica = usuMod.usu_Id
    WHERE p.proc_Id = @proc_Id AND p.proc_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_Procedimiento_List (historial por mascota)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Procedimiento_List]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(ORDER BY p.proc_Fecha DESC) AS Fila,
        p.proc_Id, p.masc_Id, masc.masc_Nombre AS Mascota, p.cita_Id,
        p.proc_Nombre, p.proc_Descripcion,
        p.proc_RealizadoPor, vet.usu_Nombre AS RealizadoPorNombre,
        p.proc_Anestesia, p.proc_DuracionMinutos, p.proc_Notas, p.proc_Fecha,
        p.serv_Id, serv.serv_Nombre AS ServicioNombre, serv.serv_Precio AS ServicioPrecio
    FROM [Medico].[tbProcedimientos] AS p
    INNER JOIN [Refugio].[tbMascotas]      AS masc ON p.masc_Id = masc.masc_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]    AS vet  ON p.proc_RealizadoPor = vet.usu_Id
    LEFT  JOIN [Veterinaria].[tbServicios] AS serv ON p.serv_Id = serv.serv_Id
    WHERE p.proc_EsEliminado = 0 AND p.masc_Id = @masc_Id
    ORDER BY p.proc_Fecha DESC
END
GO

-- =====================================================
-- SP: PR_Medico_Procedimiento_ListPorCita (los de una visita)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Procedimiento_ListPorCita]
    @cita_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(ORDER BY p.proc_Fecha ASC) AS Fila,
        p.proc_Id, p.masc_Id, masc.masc_Nombre AS Mascota, p.cita_Id,
        p.proc_Nombre, p.proc_Descripcion,
        p.proc_RealizadoPor, vet.usu_Nombre AS RealizadoPorNombre,
        p.proc_Anestesia, p.proc_DuracionMinutos, p.proc_Notas, p.proc_Fecha,
        p.serv_Id, serv.serv_Nombre AS ServicioNombre, serv.serv_Precio AS ServicioPrecio
    FROM [Medico].[tbProcedimientos] AS p
    INNER JOIN [Refugio].[tbMascotas]      AS masc ON p.masc_Id = masc.masc_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]    AS vet  ON p.proc_RealizadoPor = vet.usu_Id
    LEFT  JOIN [Veterinaria].[tbServicios] AS serv ON p.serv_Id = serv.serv_Id
    WHERE p.proc_EsEliminado = 0 AND p.cita_Id = @cita_Id
    ORDER BY p.proc_Fecha ASC
END
GO

PRINT 'Modulo PROCEDIMIENTOS ESTRUCTURADOS: tabla y SPs creados correctamente.'
PRINT 'Permisos: el controlador reutiliza las pantallas "En consulta" y "Perfil medico de mascota" (NO es pantalla nueva) -> no requiere re-login.'
GO
