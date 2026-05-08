-- Actualiza el SP de listado para incluir agregados de detalles
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesMercancia_List]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ROW_NUMBER() OVER(ORDER BY ISNULL(rm.recep_FechaModifica, rm.recep_FechaCrea) DESC) AS Fila,
        rm.recep_Id,
        rm.recep_Descripcion,
        rm.recep_Fecha,
        rm.refg_Id,
        r.refg_Nombre,
        rm.recep_TipoRecepcion,
        rm.recep_NumeroDocumento,
        ISNULL(det.TotalItems,      0)    AS recep_TotalItems,
        ISNULL(det.ValorTotal,      0.00) AS recep_ValorTotal,
        ISNULL(det.ItemsPorVencer,  0)    AS recep_ItemsPorVencer
    FROM
        [Inventario].[tbRecepcionesMercancia] rm
        INNER JOIN [Refugio].[tbRefugios] r ON rm.refg_Id = r.refg_Id
        LEFT JOIN (
            SELECT
                recep_Id,
                SUM(recdet_Cantidad)                   AS TotalItems,
                SUM(recdet_Cantidad * recdet_PrecioUnitario) AS ValorTotal,
                SUM(CASE
                        WHEN recdet_FechaVencimiento IS NOT NULL
                         AND recdet_FechaVencimiento >= CAST(GETDATE() AS DATE)
                         AND recdet_FechaVencimiento <= DATEADD(DAY, 30, CAST(GETDATE() AS DATE))
                        THEN recdet_Cantidad ELSE 0
                    END)                               AS ItemsPorVencer
            FROM [Inventario].[tbRecepcionesDetalles]
            WHERE recdet_EsEliminado = 0
            GROUP BY recep_Id
        ) det ON rm.recep_Id = det.recep_Id
    WHERE
        rm.recep_EsEliminado = 0
    ORDER BY ISNULL(rm.recep_FechaModifica, rm.recep_FechaCrea) DESC;
END
GO
