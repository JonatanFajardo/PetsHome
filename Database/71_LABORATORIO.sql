/*
=============================================
SCRIPT: Modulo LABORATORIO (examenes de laboratorio estructurados)
AUTOR:  Sistema PetsHome
FECHA:  2026-10-27
DETALLE: Tablas tbLaboratorio (encabezado) y tbLaboratorioDetalle
         (parametros con rango de referencia) + SPs de lectura/escritura.
         Aditivo: NO reemplaza Medico.tbCitaMedica.cita_ResultadosExamenes
         (texto libre); esta es la version estructurada.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE PETSHOMEDB
GO

-- =====================================================
-- TABLA: Medico.tbLaboratorio (encabezado del examen)
-- =====================================================
IF OBJECT_ID('Medico.tbLaboratorio', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbLaboratorio]
    (
        lab_Id              INT IDENTITY(1,1) NOT NULL,
        masc_Id             INT NOT NULL,
        cita_Id             INT NULL,
        lab_Examen          NVARCHAR(150) NOT NULL,
        lab_Tipo            NVARCHAR(50)  NOT NULL,          -- Sangre / Orina / Heces / Imagen / Otro
        lab_Estado          NVARCHAR(20)  NOT NULL DEFAULT 'Pendiente', -- Pendiente|Completado|Revisado (sin tildes)
        lab_FechaSolicitud  DATETIME      NOT NULL DEFAULT GETDATE(),
        lab_FechaResultado  DATETIME      NULL,
        lab_Veterinario     INT           NULL,              -- usu_Id que solicita/atiende (lente por rol)
        lab_Observaciones   NVARCHAR(1000) NULL,
        lab_EsEliminado     BIT           NOT NULL DEFAULT 0,
        lab_UsuarioCrea     INT           NOT NULL,
        lab_FechaCrea       DATETIME      NOT NULL DEFAULT GETDATE(),
        lab_UsuarioModifica INT           NULL,
        lab_FechaModifica   DATETIME      NULL,
        CONSTRAINT PK_tbLaboratorio PRIMARY KEY (lab_Id),
        CONSTRAINT FK_tbLaboratorio_Mascota   FOREIGN KEY (masc_Id)        REFERENCES [Refugio].[tbMascotas](masc_Id),
        CONSTRAINT FK_tbLaboratorio_Cita      FOREIGN KEY (cita_Id)        REFERENCES [Medico].[tbCitaMedica](cita_Id),
        CONSTRAINT FK_tbLaboratorio_UsuCrea   FOREIGN KEY (lab_UsuarioCrea) REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    )
END
GO

-- =====================================================
-- TABLA: Medico.tbLaboratorioDetalle (parametros)
-- =====================================================
IF OBJECT_ID('Medico.tbLaboratorioDetalle', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbLaboratorioDetalle]
    (
        labd_Id             INT IDENTITY(1,1) NOT NULL,
        lab_Id              INT NOT NULL,
        labd_Parametro      NVARCHAR(100) NOT NULL,          -- ej "Hematocrito"
        labd_Valor          NVARCHAR(100) NOT NULL,          -- numerico o texto
        labd_Unidad         NVARCHAR(30)  NULL,
        labd_RangoMin       DECIMAL(18,4) NULL,
        labd_RangoMax       DECIMAL(18,4) NULL,
        labd_FueraRango     BIT           NOT NULL DEFAULT 0, -- se calcula al guardar
        labd_EsEliminado    BIT           NOT NULL DEFAULT 0,
        labd_UsuarioCrea    INT           NOT NULL,
        labd_FechaCrea      DATETIME      NOT NULL DEFAULT GETDATE(),
        labd_UsuarioModifica INT          NULL,
        labd_FechaModifica  DATETIME      NULL,
        CONSTRAINT PK_tbLaboratorioDetalle PRIMARY KEY (labd_Id),
        CONSTRAINT FK_tbLaboratorioDetalle_Lab FOREIGN KEY (lab_Id) REFERENCES [Medico].[tbLaboratorio](lab_Id)
    )
END
GO

-- =====================================================
-- SP: PR_Medico_Laboratorio_List
--   Lista examenes. Filtros opcionales por mascota y por
--   veterinario (lente de rol: vet ve los suyos + sin asignar).
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Laboratorio_List]
    @masc_Id INT = NULL,
    @vet_Id  INT = NULL
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(ORDER BY lab.lab_FechaSolicitud DESC) AS Fila,
        lab.lab_Id,
        lab.masc_Id,
        masc.masc_Nombre            AS Mascota,
        lab.cita_Id,
        lab.lab_Examen,
        lab.lab_Tipo,
        lab.lab_Estado,
        lab.lab_FechaSolicitud,
        lab.lab_FechaResultado,
        lab.lab_Veterinario,
        vet.usu_Nombre              AS Veterinario,
        lab.lab_Observaciones,
        (SELECT COUNT(1) FROM [Medico].[tbLaboratorioDetalle] d
            WHERE d.lab_Id = lab.lab_Id AND d.labd_EsEliminado = 0)      AS TotalParametros,
        (SELECT COUNT(1) FROM [Medico].[tbLaboratorioDetalle] d
            WHERE d.lab_Id = lab.lab_Id AND d.labd_EsEliminado = 0
              AND d.labd_FueraRango = 1)                                 AS ParametrosFueraRango
    FROM [Medico].[tbLaboratorio] AS lab
    INNER JOIN [Refugio].[tbMascotas]  AS masc ON lab.masc_Id = masc.masc_Id
    LEFT  JOIN [Seguridad].[tbUsuarios] AS vet ON lab.lab_Veterinario = vet.usu_Id
    WHERE lab.lab_EsEliminado = 0
      AND (@masc_Id IS NULL OR lab.masc_Id = @masc_Id)
      AND (@vet_Id  IS NULL OR lab.lab_Veterinario = @vet_Id OR lab.lab_Veterinario IS NULL)
    ORDER BY lab.lab_FechaSolicitud DESC
END
GO

-- =====================================================
-- SP: PR_Medico_Laboratorio_Find (para pre-llenar el form)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Laboratorio_Find]
    @lab_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        lab.lab_Id, lab.masc_Id, lab.cita_Id, lab.lab_Examen, lab.lab_Tipo,
        lab.lab_Estado, lab.lab_FechaSolicitud, lab.lab_FechaResultado,
        lab.lab_Veterinario, lab.lab_Observaciones
    FROM [Medico].[tbLaboratorio] AS lab
    WHERE lab.lab_Id = @lab_Id AND lab.lab_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_Laboratorio_Detail (encabezado para la vista detalle)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Laboratorio_Detail]
    @lab_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        lab.lab_Id, lab.masc_Id, masc.masc_Nombre AS Mascota,
        lab.cita_Id, lab.lab_Examen, lab.lab_Tipo, lab.lab_Estado,
        lab.lab_FechaSolicitud, lab.lab_FechaResultado,
        lab.lab_Veterinario, vet.usu_Nombre AS Veterinario,
        lab.lab_Observaciones,
        usuCrea.usu_Nombre AS UsuarioCreacion, lab.lab_FechaCrea,
        usuMod.usu_Nombre  AS UsuarioModificacion, lab.lab_FechaModifica
    FROM [Medico].[tbLaboratorio] AS lab
    INNER JOIN [Refugio].[tbMascotas]   AS masc    ON lab.masc_Id = masc.masc_Id
    LEFT  JOIN [Seguridad].[tbUsuarios] AS vet     ON lab.lab_Veterinario = vet.usu_Id
    LEFT  JOIN [Seguridad].[tbUsuarios] AS usuCrea ON lab.lab_UsuarioCrea = usuCrea.usu_Id
    LEFT  JOIN [Seguridad].[tbUsuarios] AS usuMod  ON lab.lab_UsuarioModifica = usuMod.usu_Id
    WHERE lab.lab_Id = @lab_Id AND lab.lab_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_Laboratorio_Insert (retorna id + CodeStatus)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Laboratorio_Insert]
    @masc_Id           INT,
    @cita_Id           INT = NULL,
    @lab_Examen        NVARCHAR(150),
    @lab_Tipo          NVARCHAR(50),
    @lab_Estado        NVARCHAR(20) = 'Pendiente',
    @lab_Veterinario   INT = NULL,
    @lab_Observaciones NVARCHAR(1000) = NULL,
    @lab_UsuarioCrea   INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF @masc_Id IS NULL OR NOT EXISTS (SELECT 1 FROM [Refugio].[tbMascotas] WHERE masc_Id = @masc_Id)
        BEGIN
            SELECT -1 AS CodeStatus, 'La mascota indicada no existe.' AS MessageStatus, 0 AS lab_Id
            RETURN
        END

        INSERT INTO [Medico].[tbLaboratorio]
            (masc_Id, cita_Id, lab_Examen, lab_Tipo, lab_Estado,
             lab_FechaSolicitud, lab_FechaResultado, lab_Veterinario,
             lab_Observaciones, lab_UsuarioCrea, lab_FechaCrea)
        VALUES
            (@masc_Id, @cita_Id, @lab_Examen, @lab_Tipo,
             ISNULL(NULLIF(LTRIM(RTRIM(@lab_Estado)), ''), 'Pendiente'),
             GETDATE(),
             CASE WHEN @lab_Estado IN ('Completado','Revisado') THEN GETDATE() ELSE NULL END,
             @lab_Veterinario, @lab_Observaciones, @lab_UsuarioCrea, GETDATE())

        SELECT 1 AS CodeStatus, 'Examen de laboratorio registrado.' AS MessageStatus,
               CAST(SCOPE_IDENTITY() AS INT) AS lab_Id
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus, 0 AS lab_Id
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Laboratorio_Update (edita encabezado)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Laboratorio_Update]
    @lab_Id             INT,
    @masc_Id            INT,
    @cita_Id            INT = NULL,
    @lab_Examen         NVARCHAR(150),
    @lab_Tipo           NVARCHAR(50),
    @lab_Veterinario    INT = NULL,
    @lab_Observaciones  NVARCHAR(1000) = NULL,
    @lab_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbLaboratorio]
        SET masc_Id            = @masc_Id,
            cita_Id            = @cita_Id,
            lab_Examen         = @lab_Examen,
            lab_Tipo           = @lab_Tipo,
            lab_Veterinario    = @lab_Veterinario,
            lab_Observaciones  = @lab_Observaciones,
            lab_UsuarioModifica = @lab_UsuarioModifica,
            lab_FechaModifica  = GETDATE()
        WHERE lab_Id = @lab_Id AND lab_EsEliminado = 0

        SELECT 1 AS CodeStatus, 'Examen actualizado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Laboratorio_CambiarEstado
--   Pendiente -> Completado -> Revisado. Al pasar a
--   Completado/Revisado fija la fecha de resultado si falta.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Laboratorio_CambiarEstado]
    @lab_Id   INT,
    @lab_Estado NVARCHAR(20),
    @usuario  INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF @lab_Estado NOT IN ('Pendiente','Completado','Revisado')
        BEGIN
            SELECT -1 AS CodeStatus, 'Estado no valido.' AS MessageStatus
            RETURN
        END

        UPDATE [Medico].[tbLaboratorio]
        SET lab_Estado = @lab_Estado,
            lab_FechaResultado = CASE
                WHEN @lab_Estado IN ('Completado','Revisado') AND lab_FechaResultado IS NULL
                    THEN GETDATE() ELSE lab_FechaResultado END,
            lab_UsuarioModifica = @usuario,
            lab_FechaModifica = GETDATE()
        WHERE lab_Id = @lab_Id AND lab_EsEliminado = 0

        SELECT 1 AS CodeStatus, 'Estado actualizado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Laboratorio_Delete (soft delete encabezado + detalle)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Laboratorio_Delete]
    @lab_Id   INT,
    @usuario  INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbLaboratorioDetalle]
        SET labd_EsEliminado = 1, labd_UsuarioModifica = @usuario, labd_FechaModifica = GETDATE()
        WHERE lab_Id = @lab_Id

        UPDATE [Medico].[tbLaboratorio]
        SET lab_EsEliminado = 1, lab_UsuarioModifica = @usuario, lab_FechaModifica = GETDATE()
        WHERE lab_Id = @lab_Id

        SELECT 1 AS CodeStatus, 'Examen eliminado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_LaboratorioDetalle_List (parametros por examen)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_LaboratorioDetalle_List]
    @lab_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(ORDER BY d.labd_Id ASC) AS Fila,
        d.labd_Id, d.lab_Id, d.labd_Parametro, d.labd_Valor, d.labd_Unidad,
        d.labd_RangoMin, d.labd_RangoMax, d.labd_FueraRango
    FROM [Medico].[tbLaboratorioDetalle] AS d
    WHERE d.lab_Id = @lab_Id AND d.labd_EsEliminado = 0
    ORDER BY d.labd_Id ASC
END
GO

-- =====================================================
-- SP: PR_Medico_LaboratorioDetalle_Insert (agrega parametro + marca fuera de rango)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_LaboratorioDetalle_Insert]
    @lab_Id           INT,
    @labd_Parametro   NVARCHAR(100),
    @labd_Valor       NVARCHAR(100),
    @labd_Unidad      NVARCHAR(30) = NULL,
    @labd_RangoMin    DECIMAL(18,4) = NULL,
    @labd_RangoMax    DECIMAL(18,4) = NULL,
    @labd_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [Medico].[tbLaboratorio] WHERE lab_Id = @lab_Id AND lab_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'El examen indicado no existe.' AS MessageStatus
            RETURN
        END

        DECLARE @valorNum DECIMAL(18,4) = TRY_CONVERT(DECIMAL(18,4), @labd_Valor)
        DECLARE @fuera BIT = 0
        IF @valorNum IS NOT NULL
        BEGIN
            IF (@labd_RangoMin IS NOT NULL AND @valorNum < @labd_RangoMin)
               OR (@labd_RangoMax IS NOT NULL AND @valorNum > @labd_RangoMax)
                SET @fuera = 1
        END

        INSERT INTO [Medico].[tbLaboratorioDetalle]
            (lab_Id, labd_Parametro, labd_Valor, labd_Unidad, labd_RangoMin,
             labd_RangoMax, labd_FueraRango, labd_UsuarioCrea, labd_FechaCrea)
        VALUES
            (@lab_Id, @labd_Parametro, @labd_Valor, @labd_Unidad, @labd_RangoMin,
             @labd_RangoMax, @fuera, @labd_UsuarioCrea, GETDATE())

        SELECT 1 AS CodeStatus, 'Parametro agregado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_LaboratorioDetalle_Delete (quita un parametro)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_LaboratorioDetalle_Delete]
    @labd_Id  INT,
    @usuario  INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbLaboratorioDetalle]
        SET labd_EsEliminado = 1, labd_UsuarioModifica = @usuario, labd_FechaModifica = GETDATE()
        WHERE labd_Id = @labd_Id

        SELECT 1 AS CodeStatus, 'Parametro eliminado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

PRINT 'Modulo LABORATORIO: tablas y SPs creados correctamente.'
GO
