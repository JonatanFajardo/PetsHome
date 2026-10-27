-- ============================================================
-- Script 70: Signos vitales ampliados para la consulta (formato SOAP)
-- ------------------------------------------------------------
-- La pantalla "En consulta" se reorganiza en los 4 bloques clinicos SOAP
--   S (Subjetivo)  -> motivo / tipo de consulta
--   O (Objetivo)   -> signos vitales medidos por el veterinario
--   A (Assessment) -> diagnostico / evaluacion
--   P (Plan)       -> recetas, procedimientos, proxima cita
--
-- El bloque Objetivo enriquece los signos vitales. Las columnas de
-- frecuencia cardiaca y respiratoria YA existian (cita_FrecuenciaCardiaca,
-- cita_FrecuenciaRespiratoria); aqui solo se agregan las que faltaban:
--
--   cita_Mucosas           NVARCHAR(50)  -> color de mucosas (enum sin tildes)
--   cita_LlenadoCapilar    DECIMAL(3,1)  -> tiempo de llenado capilar (segundos)
--   cita_EscalaDolor       INT           -> escala de dolor 0 a 10
--   cita_CondicionCorporal DECIMAL(3,1)  -> condicion corporal (BCS) 1 a 9
--
-- Todas son OPCIONALES (NULL). No rompen el flujo existente.
-- Enums ALMACENADOS sin tildes (se muestran con tilde en la UI).
-- Ejecutar UNA sola vez en SQL Server.
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
-- 1. Columnas nuevas de signos vitales (idempotente)
-- ------------------------------------------------------------
IF COL_LENGTH('Medico.tbCitaMedica', 'cita_Mucosas') IS NULL
    ALTER TABLE Medico.tbCitaMedica ADD cita_Mucosas NVARCHAR(50) NULL
GO
IF COL_LENGTH('Medico.tbCitaMedica', 'cita_LlenadoCapilar') IS NULL
    ALTER TABLE Medico.tbCitaMedica ADD cita_LlenadoCapilar DECIMAL(3,1) NULL
GO
IF COL_LENGTH('Medico.tbCitaMedica', 'cita_EscalaDolor') IS NULL
    ALTER TABLE Medico.tbCitaMedica ADD cita_EscalaDolor INT NULL
GO
IF COL_LENGTH('Medico.tbCitaMedica', 'cita_CondicionCorporal') IS NULL
    ALTER TABLE Medico.tbCitaMedica ADD cita_CondicionCorporal DECIMAL(3,1) NULL
GO

-- ------------------------------------------------------------
-- 2. SP Insert: agrega los nuevos vitales (FC/FR ya estaban)
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_CitaMedica_Insert]
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
    @cita_UsuarioCrea               INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Refugio.tbMascotas WHERE masc_Id = @masc_Id AND masc_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'La mascota no fue encontrada.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            INSERT INTO Medico.tbCitaMedica
                (masc_Id, cita_FechaConsulta, tipoCon_Id, grav_Id, cita_MotivoConsulta,
                 cita_Diagnostico, cita_Peso, cita_Temperatura, cita_FrecuenciaCardiaca,
                 cita_FrecuenciaRespiratoria, cita_Mucosas, cita_LlenadoCapilar,
                 cita_EscalaDolor, cita_CondicionCorporal, com_Id, vac_Id,
                 cita_ProcedimientosRealizados, cita_ResultadosExamenes, cita_ProximaCita,
                 cita_MotivoProximaCita, cita_UsuarioCrea, cita_FechaCrea)
            VALUES
                (@masc_Id, @cita_FechaConsulta, @tipoCon_Id, @grav_Id, @cita_MotivoConsulta,
                 @cita_Diagnostico, @cita_Peso, @cita_Temperatura, @cita_FrecuenciaCardiaca,
                 @cita_FrecuenciaRespiratoria, @cita_Mucosas, @cita_LlenadoCapilar,
                 @cita_EscalaDolor, @cita_CondicionCorporal, @com_Id, @vac_Id,
                 @cita_ProcedimientosRealizados, @cita_ResultadosExamenes, @cita_ProximaCita,
                 @cita_MotivoProximaCita, @cita_UsuarioCrea, GETDATE())
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Cita medica creada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ------------------------------------------------------------
-- 3. SP Update: mismos vitales nuevos
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

PRINT 'Script 70 aplicado: vitales SOAP (Mucosas, LlenadoCapilar, EscalaDolor, CondicionCorporal) + SPs Insert/Update actualizados.'
GO
