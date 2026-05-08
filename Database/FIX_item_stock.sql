-- Item 5: agregar itm_StockMinimo, actualizar List SP, crear PorVencer SP

ALTER TABLE [Inventario].[tbItems]
    ADD itm_StockMinimo DECIMAL(10,2) NOT NULL
        CONSTRAINT DF_tbItems_StockMinimo DEFAULT 0;
GO

CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Items_List]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ROW_NUMBER() OVER(ORDER BY ISNULL(i.itm_FechaModifica, i.itm_FechaCrea) DESC) AS Fila,
        i.itm_Id,
        i.itm_Codigo,
        i.itm_Descripcion,
        c.cat_Descripcion,
        i.itm_Precio,
        i.itm_StockMinimo,
        ISNULL(stock.TotalRecibido, 0) AS itm_StockActual
    FROM [Inventario].[tbItems] i
    INNER JOIN [Inventario].[tbCategorias] c ON i.cat_Id = c.cat_Id
    LEFT JOIN (
        SELECT itm_Id, SUM(recdet_Cantidad) AS TotalRecibido
        FROM [Inventario].[tbRecepcionesDetalles]
        WHERE recdet_EsEliminado = 0
        GROUP BY itm_Id
    ) stock ON i.itm_Id = stock.itm_Id
    WHERE i.itm_EsEliminado = 0
    ORDER BY ISNULL(i.itm_FechaModifica, i.itm_FechaCrea) DESC;
END
GO

CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Items_PorVencer]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        i.itm_Id,
        i.itm_Codigo,
        i.itm_Descripcion,
        c.cat_Descripcion,
        rd.recdet_NumeroLote,
        rd.recdet_Cantidad,
        rd.recdet_FechaVencimiento,
        DATEDIFF(DAY, CAST(GETDATE() AS DATE), rd.recdet_FechaVencimiento) AS DiasRestantes
    FROM [Inventario].[tbRecepcionesDetalles] rd
    INNER JOIN [Inventario].[tbItems] i ON rd.itm_Id = i.itm_Id
    INNER JOIN [Inventario].[tbCategorias] c ON i.cat_Id = c.cat_Id
    WHERE rd.recdet_EsEliminado = 0
      AND rd.recdet_FechaVencimiento IS NOT NULL
      AND rd.recdet_FechaVencimiento >= CAST(GETDATE() AS DATE)
      AND rd.recdet_FechaVencimiento <= DATEADD(DAY, 30, CAST(GETDATE() AS DATE))
    ORDER BY rd.recdet_FechaVencimiento ASC;
END
GO
