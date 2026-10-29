-- 86_FACTURAS_LIST_SALDO.sql
-- Amplia PR_Veterinaria_Facturas_List con MontoPagado, Saldo y Vencimiento
-- para que /Facturacion muestre las cuentas por cobrar y su saldo, y enlace
-- al comprobante donde se registran los pagos/abonos.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Facturas_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(ORDER BY f.fact_Id DESC) AS Fila,
        f.fact_Id,
        f.fact_Numero,
        f.cli_Id,
        LTRIM(RTRIM(ISNULL(c.cli_Nombres,'') + ' ' + ISNULL(c.cli_Apellidos,''))) AS Cliente,
        f.masc_Id,
        m.masc_Nombre AS Mascota,
        f.fact_Fecha,
        f.fact_Subtotal,
        f.fact_Impuesto,
        f.fact_Total,
        f.fact_Estado,
        f.fact_MontoPagado,
        (f.fact_Total - f.fact_MontoPagado) AS Saldo,
        f.fact_Vencimiento
    FROM [Veterinaria].[tbFacturas] f
    LEFT JOIN [Veterinaria].[tbClientes] c ON f.cli_Id = c.cli_Id
    LEFT JOIN [Refugio].[tbMascotas] m ON f.masc_Id = m.masc_Id
    WHERE f.fact_EsEliminado = 0
    ORDER BY f.fact_Id DESC
END
GO

-- Backfill: facturas ya 'Pagada' (anteriores a pagos) con MontoPagado=0 -> MontoPagado=Total
UPDATE Veterinaria.tbFacturas SET fact_MontoPagado = fact_Total WHERE fact_Estado = 'Pagada' AND fact_MontoPagado < fact_Total AND fact_EsEliminado = 0;
GO
