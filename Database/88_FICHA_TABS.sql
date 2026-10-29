-- =============================================================================
-- 88_FICHA_TABS.sql
-- Ficha del paciente (PerfilMedico): SPs para las pestanas nuevas vs OpenVPM.
--   Facturas | Signos vitales | Historial de peso | Laboratorio
-- Todos por mascota (@masc_Id). Idempotentes (CREATE OR ALTER).
-- =============================================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

USE PETSHOMEDB;
GO

-- -----------------------------------------------------------------------------
-- 1) Facturas por mascota
--    Saldo = fact_Total - fact_MontoPagado (COALESCE por si no hay pago).
--    No eliminadas, orden por fecha descendente.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Facturas_PorMascota]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        f.fact_Numero                                   AS Numero,
        f.fact_Fecha                                    AS Fecha,
        f.fact_Total                                    AS Total,
        (f.fact_Total - COALESCE(f.fact_MontoPagado, 0)) AS Saldo,
        f.fact_Estado                                   AS Estado
    FROM   [Veterinaria].[tbFacturas] f
    WHERE  f.masc_Id         = @masc_Id
      AND  f.fact_EsEliminado = 0
    ORDER BY f.fact_Fecha DESC;
END
GO

-- -----------------------------------------------------------------------------
-- 2) Signos vitales por mascota
--    Una fila por cita que tenga al menos un signo vital registrado.
--    Orden por fecha descendente.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_CitaMedica_VitalesPorMascota]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.cita_FechaConsulta          AS Fecha,
        c.cita_Peso                   AS Peso,
        c.cita_Temperatura            AS Temperatura,
        c.cita_FrecuenciaCardiaca     AS FrecuenciaCardiaca,
        c.cita_FrecuenciaRespiratoria AS FrecuenciaRespiratoria,
        c.cita_Mucosas                AS Mucosas,
        c.cita_LlenadoCapilar         AS LlenadoCapilar,
        c.cita_EscalaDolor            AS EscalaDolor,
        c.cita_CondicionCorporal      AS CondicionCorporal
    FROM   [Medico].[tbCitaMedica] c
    WHERE  c.masc_Id         = @masc_Id
      AND  c.cita_EsEliminado = 0
      AND  (
               c.cita_Peso                   IS NOT NULL
            OR c.cita_Temperatura            IS NOT NULL
            OR c.cita_FrecuenciaCardiaca     IS NOT NULL
            OR c.cita_FrecuenciaRespiratoria IS NOT NULL
            OR c.cita_Mucosas                IS NOT NULL
            OR c.cita_LlenadoCapilar         IS NOT NULL
            OR c.cita_EscalaDolor            IS NOT NULL
            OR c.cita_CondicionCorporal      IS NOT NULL
          )
    ORDER BY c.cita_FechaConsulta DESC;
END
GO

-- -----------------------------------------------------------------------------
-- 3) Historial de peso por mascota
--    Solo citas con peso registrado. Orden ASCENDENTE para la grafica.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_CitaMedica_PesoPorMascota]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.cita_FechaConsulta AS Fecha,
        c.cita_Peso          AS Peso
    FROM   [Medico].[tbCitaMedica] c
    WHERE  c.masc_Id         = @masc_Id
      AND  c.cita_EsEliminado = 0
      AND  c.cita_Peso        IS NOT NULL
    ORDER BY c.cita_FechaConsulta ASC;
END
GO

-- -----------------------------------------------------------------------------
-- 4) Laboratorio por mascota
--    No eliminados, orden por fecha de solicitud descendente.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Laboratorio_PorMascota]
    @masc_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        l.lab_Examen         AS Examen,
        l.lab_Tipo           AS Tipo,
        l.lab_Estado         AS Estado,
        l.lab_FechaSolicitud AS FechaSolicitud,
        l.lab_FechaResultado AS FechaResultado,
        l.lab_Observaciones  AS Observaciones
    FROM   [Medico].[tbLaboratorio] l
    WHERE  l.masc_Id        = @masc_Id
      AND  l.lab_EsEliminado = 0
    ORDER BY l.lab_FechaSolicitud DESC;
END
GO
