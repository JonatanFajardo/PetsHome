-- 87_FACTURACION_RESUMEN_AR.sql
-- Resumen de Cuentas por Cobrar (A/R) para la pantalla /Facturacion.
-- Replica las tarjetas del /billing de OpenVPM (arSummary): Por cobrar,
-- Vencido y Cobrado este mes. Devuelve UNA fila con 3 decimales.
--
-- Definiciones (identicas a OpenVPM arSummary):
--   PorCobrar  = SUMA de saldos (fact_Total - fact_MontoPagado) de facturas
--                NO pagadas (saldo > 0), no eliminadas, estado <> 'Anulada'.
--   Vencido    = de esas, las que ya pasaron su fecha de vencimiento
--                (fact_Vencimiento < hoy y saldo > 0).
--   CobradoMes = SUMA de pagos recibidos desde el inicio del mes actual
--                (SUM(pago_Monto) de tbPagos con pago_Fecha >= inicio de mes,
--                 no eliminados).
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF OBJECT_ID('[Veterinaria].[PR_Veterinaria_Facturas_ResumenAR]', 'P') IS NOT NULL
    DROP PROCEDURE [Veterinaria].[PR_Veterinaria_Facturas_ResumenAR];
GO

CREATE PROCEDURE [Veterinaria].[PR_Veterinaria_Facturas_ResumenAR]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Hoy date = CAST(GETDATE() AS date);
    DECLARE @InicioMes date = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

    SELECT
        -- Por cobrar: saldo de facturas no pagadas (saldo > 0)
        PorCobrar = ISNULL((
            SELECT SUM(f.fact_Total - f.fact_MontoPagado)
            FROM [Veterinaria].[tbFacturas] f
            WHERE f.fact_EsEliminado = 0
              AND f.fact_Estado <> 'Anulada'
              AND (f.fact_Total - f.fact_MontoPagado) > 0
        ), 0),
        -- Vencido: de las anteriores, las que pasaron su fecha de vencimiento
        Vencido = ISNULL((
            SELECT SUM(f.fact_Total - f.fact_MontoPagado)
            FROM [Veterinaria].[tbFacturas] f
            WHERE f.fact_EsEliminado = 0
              AND f.fact_Estado <> 'Anulada'
              AND (f.fact_Total - f.fact_MontoPagado) > 0
              AND f.fact_Vencimiento IS NOT NULL
              AND f.fact_Vencimiento < @Hoy
        ), 0),
        -- Cobrado este mes: pagos recibidos desde el inicio del mes actual
        CobradoMes = ISNULL((
            SELECT SUM(p.pago_Monto)
            FROM [Veterinaria].[tbPagos] p
            WHERE p.pago_EsEliminado = 0
              AND p.pago_Fecha >= @InicioMes
        ), 0);
END
GO
