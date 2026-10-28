/*
=============================================
SCRIPT: Modulo CONSENTIMIENTOS (autorizaciones firmadas por el dueño)
AUTOR:  Sistema PetsHome
FECHA:  2026-10-27
DETALLE: Tabla Medico.tbConsentimiento + SPs de lectura/escritura.
         El dueño (cliente) autoriza un procedimiento (General, Cirugia,
         Anestesia, Eutanasia). Se registra estado (Pendiente|Firmado|
         Rechazado), quien firma y cuando.
         Enums ALMACENADOS sin tildes; la UI los muestra con tilde.
         Aditivo: no reemplaza nada. Convive con la ficha de responsable
         binario (script 56): el cli_Id se resuelve de la mascota.
         FIRMA DIGITAL grafica (trazo) FUERA de alcance v1: se guarda el
         nombre del firmante + fecha + estado "Firmado".
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE PETSHOMEDB
GO

-- =====================================================
-- TABLA: Medico.tbConsentimiento
-- =====================================================
IF OBJECT_ID('Medico.tbConsentimiento', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbConsentimiento]
    (
        cons_Id              INT IDENTITY(1,1) NOT NULL,
        masc_Id              INT NOT NULL,
        cli_Id               INT NULL,                       -- dueño/cliente (se resuelve de la mascota)
        cita_Id              INT NULL,                       -- consulta opcional en la que se solicita
        cons_Tipo            NVARCHAR(20)  NOT NULL,         -- General|Cirugia|Anestesia|Eutanasia (sin tildes)
        cons_Estado          NVARCHAR(20)  NOT NULL DEFAULT 'Pendiente', -- Pendiente|Firmado|Rechazado (sin tildes)
        cons_Texto           NVARCHAR(MAX) NOT NULL,         -- declaracion/texto del consentimiento
        cons_FirmanteNombre  NVARCHAR(150) NULL,             -- quien firma (dueño o responsable)
        cons_FechaFirma      DATETIME      NULL,             -- fecha de firma o rechazo
        cons_Notas           NVARCHAR(1000) NULL,
        cons_EsEliminado     BIT           NOT NULL DEFAULT 0,
        cons_UsuarioCrea     INT           NOT NULL,
        cons_FechaCrea       DATETIME      NOT NULL DEFAULT GETDATE(),
        cons_UsuarioModifica INT           NULL,
        cons_FechaModifica   DATETIME      NULL,
        CONSTRAINT PK_tbConsentimiento PRIMARY KEY (cons_Id),
        CONSTRAINT FK_tbConsentimiento_Mascota FOREIGN KEY (masc_Id)          REFERENCES [Refugio].[tbMascotas](masc_Id),
        CONSTRAINT FK_tbConsentimiento_Cliente FOREIGN KEY (cli_Id)           REFERENCES [Veterinaria].[tbClientes](cli_Id),
        CONSTRAINT FK_tbConsentimiento_Cita    FOREIGN KEY (cita_Id)          REFERENCES [Medico].[tbCitaMedica](cita_Id),
        CONSTRAINT FK_tbConsentimiento_UsuCrea FOREIGN KEY (cons_UsuarioCrea) REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT CK_tbConsentimiento_Tipo   CHECK (cons_Tipo   IN ('General','Cirugia','Anestesia','Eutanasia')),
        CONSTRAINT CK_tbConsentimiento_Estado CHECK (cons_Estado IN ('Pendiente','Firmado','Rechazado'))
    )
    PRINT 'Tabla Medico.tbConsentimiento creada.'
END
ELSE
    PRINT 'La tabla Medico.tbConsentimiento ya existia, se omite.'
GO

-- =====================================================
-- SP: PR_Medico_Consentimiento_List
--   Lista consentimientos. Filtros opcionales por mascota,
--   cliente y estado.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consentimiento_List]
    @masc_Id INT = NULL,
    @cli_Id  INT = NULL,
    @estado  NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(ORDER BY cons.cons_FechaCrea DESC) AS Fila,
        cons.cons_Id,
        cons.masc_Id,
        masc.masc_Nombre            AS Mascota,
        cons.cli_Id,
        CASE WHEN cons.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(cli.cli_Nombres + ' ' + ISNULL(cli.cli_Apellidos, '')))
             ELSE 'Interno (refugio)' END AS Responsable,
        cons.cita_Id,
        cons.cons_Tipo,
        cons.cons_Estado,
        cons.cons_FirmanteNombre,
        cons.cons_FechaFirma,
        cons.cons_FechaCrea
    FROM [Medico].[tbConsentimiento] AS cons
    INNER JOIN [Refugio].[tbMascotas]   AS masc ON cons.masc_Id = masc.masc_Id
    LEFT  JOIN [Veterinaria].[tbClientes] AS cli ON cons.cli_Id = cli.cli_Id
    WHERE cons.cons_EsEliminado = 0
      AND (@masc_Id IS NULL OR cons.masc_Id = @masc_Id)
      AND (@cli_Id  IS NULL OR cons.cli_Id  = @cli_Id)
      AND (@estado  IS NULL OR cons.cons_Estado = @estado)
    ORDER BY cons.cons_FechaCrea DESC
END
GO

-- =====================================================
-- SP: PR_Medico_Consentimiento_Find (pre-llenar el form; solo mientras Pendiente)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consentimiento_Find]
    @cons_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        cons.cons_Id, cons.masc_Id, cons.cli_Id, cons.cita_Id,
        cons.cons_Tipo, cons.cons_Estado, cons.cons_Texto, cons.cons_Notas
    FROM [Medico].[tbConsentimiento] AS cons
    WHERE cons.cons_Id = @cons_Id AND cons.cons_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_Consentimiento_Detail (vista detalle)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consentimiento_Detail]
    @cons_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        cons.cons_Id, cons.masc_Id, masc.masc_Nombre AS Mascota,
        cons.cli_Id,
        CASE WHEN cons.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(cli.cli_Nombres + ' ' + ISNULL(cli.cli_Apellidos, '')))
             ELSE 'Interno (refugio)' END AS Responsable,
        CASE WHEN cons.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(ISNULL(cli.cli_Telefono, '') +
                  CASE WHEN cli.cli_Correo IS NOT NULL THEN ' · ' + cli.cli_Correo ELSE '' END))
             ELSE NULL END AS ResponsableContacto,
        cons.cita_Id, cons.cons_Tipo, cons.cons_Estado, cons.cons_Texto,
        cons.cons_FirmanteNombre, cons.cons_FechaFirma, cons.cons_Notas,
        usuCrea.usu_Nombre AS UsuarioCreacion, cons.cons_FechaCrea,
        usuMod.usu_Nombre  AS UsuarioModificacion, cons.cons_FechaModifica
    FROM [Medico].[tbConsentimiento] AS cons
    INNER JOIN [Refugio].[tbMascotas]    AS masc    ON cons.masc_Id = masc.masc_Id
    LEFT  JOIN [Veterinaria].[tbClientes] AS cli    ON cons.cli_Id = cli.cli_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]  AS usuCrea ON cons.cons_UsuarioCrea = usuCrea.usu_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]  AS usuMod  ON cons.cons_UsuarioModifica = usuMod.usu_Id
    WHERE cons.cons_Id = @cons_Id AND cons.cons_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_Consentimiento_Insert (retorna id + CodeStatus)
--   El cli_Id (dueño) se resuelve de la mascota si no se envia.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consentimiento_Insert]
    @masc_Id          INT,
    @cli_Id           INT = NULL,
    @cita_Id          INT = NULL,
    @cons_Tipo        NVARCHAR(20),
    @cons_Texto       NVARCHAR(MAX),
    @cons_Notas       NVARCHAR(1000) = NULL,
    @cons_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF @masc_Id IS NULL OR NOT EXISTS (SELECT 1 FROM [Refugio].[tbMascotas] WHERE masc_Id = @masc_Id)
        BEGIN
            SELECT -1 AS CodeStatus, 'La mascota indicada no existe.' AS MessageStatus, 0 AS cons_Id
            RETURN
        END

        IF @cons_Tipo NOT IN ('General','Cirugia','Anestesia','Eutanasia')
        BEGIN
            SELECT -2 AS CodeStatus, 'Tipo de consentimiento no valido.' AS MessageStatus, 0 AS cons_Id
            RETURN
        END

        IF @cons_Texto IS NULL OR LTRIM(RTRIM(@cons_Texto)) = ''
        BEGIN
            SELECT -3 AS CodeStatus, 'El texto del consentimiento es obligatorio.' AS MessageStatus, 0 AS cons_Id
            RETURN
        END

        -- Resolver el dueño desde la mascota si no vino explicito
        DECLARE @cliResolved INT = ISNULL(@cli_Id, (SELECT cli_Id FROM [Refugio].[tbMascotas] WHERE masc_Id = @masc_Id))

        INSERT INTO [Medico].[tbConsentimiento]
            (masc_Id, cli_Id, cita_Id, cons_Tipo, cons_Estado, cons_Texto,
             cons_Notas, cons_UsuarioCrea, cons_FechaCrea)
        VALUES
            (@masc_Id, @cliResolved, @cita_Id, @cons_Tipo, 'Pendiente', @cons_Texto,
             @cons_Notas, @cons_UsuarioCrea, GETDATE())

        SELECT 1 AS CodeStatus, 'Consentimiento registrado.' AS MessageStatus,
               CAST(SCOPE_IDENTITY() AS INT) AS cons_Id
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus, 0 AS cons_Id
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Consentimiento_Update
--   Edita texto/tipo/notas SOLO mientras esta Pendiente
--   (una vez firmado/rechazado, el texto es inmutable).
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consentimiento_Update]
    @cons_Id             INT,
    @cons_Tipo           NVARCHAR(20),
    @cons_Texto          NVARCHAR(MAX),
    @cons_Notas          NVARCHAR(1000) = NULL,
    @cons_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @estadoActual NVARCHAR(20) = (SELECT cons_Estado FROM [Medico].[tbConsentimiento]
                                              WHERE cons_Id = @cons_Id AND cons_EsEliminado = 0)
        IF @estadoActual IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, 'El consentimiento no existe.' AS MessageStatus
            RETURN
        END
        IF @estadoActual <> 'Pendiente'
        BEGIN
            SELECT -2 AS CodeStatus, 'Un consentimiento firmado o rechazado no se puede editar.' AS MessageStatus
            RETURN
        END
        IF @cons_Tipo NOT IN ('General','Cirugia','Anestesia','Eutanasia')
        BEGIN
            SELECT -3 AS CodeStatus, 'Tipo de consentimiento no valido.' AS MessageStatus
            RETURN
        END

        UPDATE [Medico].[tbConsentimiento]
        SET cons_Tipo  = @cons_Tipo,
            cons_Texto = @cons_Texto,
            cons_Notas = @cons_Notas,
            cons_UsuarioModifica = @cons_UsuarioModifica,
            cons_FechaModifica = GETDATE()
        WHERE cons_Id = @cons_Id AND cons_EsEliminado = 0

        SELECT 1 AS CodeStatus, 'Consentimiento actualizado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Consentimiento_CambiarEstado
--   Pendiente -> Firmado (requiere nombre del firmante) o
--   Pendiente -> Rechazado. Estados terminales: no se revierten.
--   Fija cons_FechaFirma con la fecha del evento.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consentimiento_CambiarEstado]
    @cons_Id   INT,
    @cons_Estado NVARCHAR(20),
    @firmante  NVARCHAR(150) = NULL,
    @usuario   INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF @cons_Estado NOT IN ('Firmado','Rechazado')
        BEGIN
            SELECT -1 AS CodeStatus, 'Estado destino no valido.' AS MessageStatus
            RETURN
        END

        DECLARE @estadoActual NVARCHAR(20) = (SELECT cons_Estado FROM [Medico].[tbConsentimiento]
                                              WHERE cons_Id = @cons_Id AND cons_EsEliminado = 0)
        IF @estadoActual IS NULL
        BEGIN
            SELECT -2 AS CodeStatus, 'El consentimiento no existe.' AS MessageStatus
            RETURN
        END
        IF @estadoActual <> 'Pendiente'
        BEGIN
            SELECT -3 AS CodeStatus, 'Solo un consentimiento pendiente puede firmarse o rechazarse.' AS MessageStatus
            RETURN
        END
        IF @cons_Estado = 'Firmado' AND (@firmante IS NULL OR LTRIM(RTRIM(@firmante)) = '')
        BEGIN
            SELECT -4 AS CodeStatus, 'Debe indicar el nombre de quien firma.' AS MessageStatus
            RETURN
        END

        UPDATE [Medico].[tbConsentimiento]
        SET cons_Estado = @cons_Estado,
            cons_FirmanteNombre = CASE WHEN @cons_Estado = 'Firmado' THEN @firmante
                                       ELSE NULLIF(LTRIM(RTRIM(@firmante)), '') END,
            cons_FechaFirma = GETDATE(),
            cons_UsuarioModifica = @usuario,
            cons_FechaModifica = GETDATE()
        WHERE cons_Id = @cons_Id AND cons_EsEliminado = 0

        SELECT 1 AS CodeStatus,
               CASE WHEN @cons_Estado = 'Firmado' THEN 'Consentimiento firmado.' ELSE 'Consentimiento rechazado.' END AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_Consentimiento_Delete (soft delete / anular)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consentimiento_Delete]
    @cons_Id  INT,
    @usuario  INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbConsentimiento]
        SET cons_EsEliminado = 1, cons_UsuarioModifica = @usuario, cons_FechaModifica = GETDATE()
        WHERE cons_Id = @cons_Id

        SELECT 1 AS CodeStatus, 'Consentimiento anulado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

PRINT 'Modulo CONSENTIMIENTOS: tabla y SPs creados correctamente.'
GO
