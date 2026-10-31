-- ============================================================
-- Script 100: Firmar la nota clinica + Addenda (correcciones append-only)
-- ------------------------------------------------------------
-- Replica el modelo SOAP de OpenVPM:
--   * La nota clinica (SOAP + diagnostico) se puede FIRMAR/finalizar.
--   * Una vez firmada queda INMUTABLE: el texto original NO se altera.
--   * Las correcciones posteriores se agregan como ADDENDA (append-only):
--     se suman debajo, no reemplazan ni borran lo original.
--
-- OJO: esto NO es lo mismo que "Finalizar la VISITA".
--   - Finalizar visita  = estado de la cita (cita_Estado: En espera ->
--     En atencion -> Finalizada). Flujo del tablero. NO se toca aqui.
--   - Firmar la nota     = bloquear el EXPEDIENTE clinico de esa cita
--     para que su texto ya no se edite (solo addenda). Es lo de este script.
-- Son dos cosas distintas sobre la misma cita: una es el estado del turno,
-- la otra es el candado legal de la nota.
--
-- Idempotente (COL_LENGTH / OBJECT_ID guards, CREATE OR ALTER). GO entre lotes.
-- Ejecutar en SQL Server (PETSHOMEDB).
-- ============================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO
SET NOCOUNT ON
GO

-- ------------------------------------------------------------
-- 1. Columnas de firma en la cita (bloqueo de la nota clinica)
-- ------------------------------------------------------------
IF COL_LENGTH('Medico.tbCitaMedica', 'cita_Firmada') IS NULL
    ALTER TABLE Medico.tbCitaMedica ADD cita_Firmada BIT NOT NULL CONSTRAINT DF_tbCitaMedica_cita_Firmada DEFAULT(0)
GO
IF COL_LENGTH('Medico.tbCitaMedica', 'cita_FechaFirma') IS NULL
    ALTER TABLE Medico.tbCitaMedica ADD cita_FechaFirma DATETIME NULL
GO
IF COL_LENGTH('Medico.tbCitaMedica', 'cita_FirmadaPor') IS NULL
    ALTER TABLE Medico.tbCitaMedica ADD cita_FirmadaPor INT NULL
GO

-- ------------------------------------------------------------
-- 2. Tabla de addenda (correcciones append-only de la nota firmada)
-- ------------------------------------------------------------
IF OBJECT_ID('Medico.tbConsultaAddenda', 'U') IS NULL
BEGIN
    CREATE TABLE Medico.tbConsultaAddenda
    (
        add_Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tbConsultaAddenda PRIMARY KEY,
        cita_Id         INT           NOT NULL,
        add_Texto       NVARCHAR(2000) NOT NULL,
        add_Fecha       DATETIME      NOT NULL CONSTRAINT DF_tbConsultaAddenda_add_Fecha DEFAULT(GETDATE()),
        add_Usuario     INT           NULL,
        add_EsEliminado BIT           NOT NULL CONSTRAINT DF_tbConsultaAddenda_add_EsEliminado DEFAULT(0),
        CONSTRAINT FK_tbConsultaAddenda_tbCitaMedica
            FOREIGN KEY (cita_Id) REFERENCES Medico.tbCitaMedica(cita_Id)
    )
    CREATE INDEX IX_tbConsultaAddenda_cita ON Medico.tbConsultaAddenda(cita_Id, add_EsEliminado)
END
GO

-- ------------------------------------------------------------
-- 3. SP: Firmar la nota clinica de una cita
--    Marca firmada SOLO si no lo estaba (no re-firma ni cambia la fecha).
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consulta_FirmarNota]
    @cita_Id    INT,
    @usuario    INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Medico.tbCitaMedica WHERE cita_Id = @cita_Id AND cita_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'La cita medica no fue encontrada.' AS MessageStatus
            RETURN
        END

        IF EXISTS (SELECT 1 FROM Medico.tbCitaMedica WHERE cita_Id = @cita_Id AND cita_Firmada = 1)
        BEGIN
            -- Ya estaba firmada: no es error, pero no se re-firma ni se pisa la fecha.
            SELECT 1 AS CodeStatus, 'La nota ya estaba firmada.' AS MessageStatus
            RETURN
        END

        UPDATE Medico.tbCitaMedica
        SET cita_Firmada    = 1,
            cita_FechaFirma = GETDATE(),
            cita_FirmadaPor = @usuario
        WHERE cita_Id = @cita_Id

        SELECT 1 AS CodeStatus, 'Nota clinica firmada. Queda bloqueada; use addenda para corregir.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ------------------------------------------------------------
-- 4. SP: Agregar una addenda (correccion posterior)
--    Permitida SIEMPRE, incluso despues de firmar (ESE es el punto).
--    Append-only: nunca edita ni borra el texto original de la nota.
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consulta_AgregarAddenda]
    @cita_Id    INT,
    @texto      NVARCHAR(2000),
    @usuario    INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Medico.tbCitaMedica WHERE cita_Id = @cita_Id AND cita_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'La cita medica no fue encontrada.' AS MessageStatus
            RETURN
        END

        IF @texto IS NULL OR LEN(LTRIM(RTRIM(@texto))) = 0
        BEGIN
            SELECT -2 AS CodeStatus, 'El texto de la correccion no puede estar vacio.' AS MessageStatus
            RETURN
        END

        INSERT INTO Medico.tbConsultaAddenda (cita_Id, add_Texto, add_Usuario)
        VALUES (@cita_Id, LTRIM(RTRIM(@texto)), @usuario)

        SELECT 1 AS CodeStatus, 'Correccion (addenda) agregada.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ------------------------------------------------------------
-- 5. SP: Listar addenda de una cita (mas antiguas primero: orden de lectura)
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consulta_ListAddenda]
    @cita_Id    INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        a.add_Id,
        a.cita_Id,
        a.add_Texto,
        a.add_Fecha,
        a.add_Usuario,
        CAST(u.usu_Nombre AS NVARCHAR(200)) AS AutorNombre
    FROM Medico.tbConsultaAddenda a
    LEFT JOIN Seguridad.tbUsuarios u ON a.add_Usuario = u.usu_Id
    WHERE a.cita_Id = @cita_Id
      AND a.add_EsEliminado = 0
    ORDER BY a.add_Id ASC
END
GO

-- ------------------------------------------------------------
-- 6. SP: Estado de firma de una cita (para la UI de la consulta)
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Consulta_FirmaEstado]
    @cita_Id    INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        c.cita_Id,
        c.cita_Firmada,
        c.cita_FechaFirma,
        c.cita_FirmadaPor,
        CAST(u.usu_Nombre AS NVARCHAR(200)) AS FirmadaPorNombre
    FROM Medico.tbCitaMedica c
    LEFT JOIN Seguridad.tbUsuarios u ON c.cita_FirmadaPor = u.usu_Id
    WHERE c.cita_Id = @cita_Id
      AND c.cita_EsEliminado = 0
END
GO

-- ------------------------------------------------------------
-- 7. Inmutabilidad en el guardado: si la nota esta firmada, el SP de
--    Update de la cita NO modifica el texto clinico (devuelve -9).
--    Se vuelve a declarar el SP completo (venia del script 70) agregando
--    solo el candado al inicio. El resto queda identico.
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_CitaMedica_Update]
    @cita_Id                        INT,
    @masc_Id                        INT,
    @cita_FechaConsulta             DATETIME,
    @tipoCon_Id                     INT           = NULL,
    @grav_Id                        INT           = NULL,
    @cita_MotivoConsulta            NVARCHAR(500) = NULL,
    @cita_Diagnostico               NVARCHAR(500) = NULL,
    @cita_Peso                      DECIMAL(5,2)  = NULL,
    @cita_Temperatura               DECIMAL(4,2)  = NULL,
    @cita_FrecuenciaCardiaca        INT           = NULL,
    @cita_FrecuenciaRespiratoria    INT           = NULL,
    @cita_Mucosas                   NVARCHAR(50)  = NULL,
    @cita_LlenadoCapilar            DECIMAL(3,1)  = NULL,
    @cita_EscalaDolor               INT           = NULL,
    @cita_CondicionCorporal         DECIMAL(3,1)  = NULL,
    @com_Id                         INT           = NULL,
    @vac_Id                         INT           = NULL,
    @cita_ProcedimientosRealizados  NVARCHAR(500) = NULL,
    @cita_ResultadosExamenes        NVARCHAR(500) = NULL,
    @cita_ProximaCita               DATETIME      = NULL,
    @cita_MotivoProximaCita         NVARCHAR(200) = NULL,
    @cita_UsuarioModifica           INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Medico.tbCitaMedica WHERE cita_Id = @cita_Id AND cita_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'La cita medica no fue encontrada.' AS MessageStatus
            RETURN
        END

        -- Candado de inmutabilidad: nota firmada no se edita (solo addenda).
        IF EXISTS (SELECT 1 FROM Medico.tbCitaMedica WHERE cita_Id = @cita_Id AND cita_Firmada = 1)
        BEGIN
            SELECT -9 AS CodeStatus, 'La nota clinica esta firmada y no se puede modificar. Agregue una addenda (correccion).' AS MessageStatus
            RETURN
        END

        BEGIN TRANSACTION
            UPDATE Medico.tbCitaMedica
            SET masc_Id                        = @masc_Id,
                cita_FechaConsulta             = @cita_FechaConsulta,
                tipoCon_Id                     = @tipoCon_Id,
                grav_Id                        = @grav_Id,
                cita_MotivoConsulta            = @cita_MotivoConsulta,
                cita_Diagnostico               = @cita_Diagnostico,
                cita_Peso                      = @cita_Peso,
                cita_Temperatura               = @cita_Temperatura,
                cita_FrecuenciaCardiaca        = @cita_FrecuenciaCardiaca,
                cita_FrecuenciaRespiratoria    = @cita_FrecuenciaRespiratoria,
                cita_Mucosas                   = @cita_Mucosas,
                cita_LlenadoCapilar            = @cita_LlenadoCapilar,
                cita_EscalaDolor               = @cita_EscalaDolor,
                cita_CondicionCorporal         = @cita_CondicionCorporal,
                com_Id                         = @com_Id,
                vac_Id                         = @vac_Id,
                cita_ProcedimientosRealizados  = @cita_ProcedimientosRealizados,
                cita_ResultadosExamenes        = @cita_ResultadosExamenes,
                cita_ProximaCita               = @cita_ProximaCita,
                cita_MotivoProximaCita         = @cita_MotivoProximaCita,
                cita_UsuarioModifica           = @cita_UsuarioModifica,
                cita_FechaModifica             = GETDATE()
            WHERE cita_Id = @cita_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Cita medica actualizada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

PRINT 'Script 100 aplicado: firma de nota clinica (cita_Firmada/FechaFirma/FirmadaPor), tabla Medico.tbConsultaAddenda, SPs FirmarNota/AgregarAddenda/ListAddenda/FirmaEstado y candado de inmutabilidad en PR_Medico_CitaMedica_Update.'
GO
