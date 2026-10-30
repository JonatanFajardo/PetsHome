/*
=============================================
SCRIPT 89: Reportes (clon de la pantalla /reports de OpenVPM)
AUTOR: Sistema PetsHome
FECHA: 2026-10-29
DESCRIPCION:
  SPs para los 4 reportes de la pantalla "Reportes":
    1. [Veterinaria].[PR_Veterinaria_Reportes_Ingresos]      -> serie diaria de ingresos
    2. [Medico].[PR_Medico_Reportes_Citas]                   -> KPIs de citas (1 fila)
    3. [Medico].[PR_Medico_Reportes_CitasPorDoctor]          -> citas por veterinario
    4. [Veterinaria].[PR_Veterinaria_Reportes_TopServicios]  -> top servicios facturados
    5. [Inventario].[PR_Inventario_Reportes_StockBajo]       -> items con stock <= minimo
  Idempotente (CREATE OR ALTER). CERO cambios de esquema.

  Columnas reales usadas (verificadas en vivo):
    - Veterinaria.tbFacturas: fact_Total, fact_Fecha(datetime), fact_Estado, fact_EsEliminado
    - Veterinaria.tbFacturaDetalle: fdet_Tipo('Servicio'/'Producto'), serv_Id, fdet_Cantidad, fdet_Subtotal, fact_Id
    - Veterinaria.tbServicios: serv_Id, serv_Nombre
    - Medico.tbCitaMedica: cita_FechaConsulta, cita_Estado('En espera'/'En atencion'/'Finalizada'),
                           cita_Veterinario(int -> Seguridad.tbUsuarios.usu_Id)
    - Seguridad.tbUsuarios: usu_Id, Usu_Nombre (nombre mostrado del veterinario)
    - Inventario.tbItems: itm_Descripcion (nombre), itm_StockMinimo (minimo). NO tiene stock actual.
    - Inventario.tbExistencias: exi_Cantidad (stock actual por refugio; se suma por item)

  Notas de negocio:
    - fact_Estado real: 'Emitida'/'Pagada'/'PorCobrar' (aun no existe 'Anulada'); se excluye 'Anulada' de todas formas.
    - cita_Estado real: solo 'En espera'/'En atencion'/'Finalizada' (no hay 'Cancelada' ni 'NoShow');
      Atendidas='Finalizada', Canceladas='Cancelada' (0 hoy), NoShow=0 (PetsHome no tiene ese estado).
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

-- =====================================================
-- 1. Ingresos: serie diaria SUM(fact_Total) por dia
--    (el total del periodo / periodo anterior los calcula el backend
--     llamando de nuevo con otro rango; aqui solo se devuelve la serie)
-- =====================================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Reportes_Ingresos]
    @desde DATE,
    @hasta DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST(f.fact_Fecha AS DATE)            AS Fecha,
        CAST(SUM(f.fact_Total) AS DECIMAL(18,2)) AS Monto
    FROM [Veterinaria].[tbFacturas] f
    WHERE f.fact_EsEliminado = 0
      AND f.fact_Estado <> 'Anulada'
      AND CAST(f.fact_Fecha AS DATE) BETWEEN @desde AND @hasta
    GROUP BY CAST(f.fact_Fecha AS DATE)
    ORDER BY Fecha;
END
GO

-- =====================================================
-- 2. Citas: KPIs en una sola fila
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Reportes_Citas]
    @desde DATE,
    @hasta DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(*)                                                               AS Total,
        SUM(CASE WHEN c.cita_Estado = 'Finalizada' THEN 1 ELSE 0 END)          AS Atendidas,
        0                                                                      AS NoShow,
        SUM(CASE WHEN c.cita_Estado = 'Cancelada'  THEN 1 ELSE 0 END)          AS Canceladas
    FROM [Medico].[tbCitaMedica] c
    WHERE c.cita_EsEliminado = 0
      AND CAST(c.cita_FechaConsulta AS DATE) BETWEEN @desde AND @hasta;
END
GO

-- =====================================================
-- 3. CitasPorDoctor: conteo de citas por veterinario
--    (cita_Veterinario -> Seguridad.tbUsuarios.usu_Id; sin asignar = 'Sin asignar')
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Reportes_CitasPorDoctor]
    @desde DATE,
    @hasta DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ISNULL(CAST(u.Usu_Nombre AS NVARCHAR(200)), 'Sin asignar') AS Doctor,
        COUNT(*)                                                   AS Cantidad
    FROM [Medico].[tbCitaMedica] c
    LEFT JOIN [Seguridad].[tbUsuarios] u ON c.cita_Veterinario = u.usu_Id
    WHERE c.cita_EsEliminado = 0
      AND CAST(c.cita_FechaConsulta AS DATE) BETWEEN @desde AND @hasta
    GROUP BY ISNULL(CAST(u.Usu_Nombre AS NVARCHAR(200)), 'Sin asignar')
    ORDER BY Cantidad DESC;
END
GO

-- =====================================================
-- 4. TopServicios: cantidad e ingreso por servicio facturado
-- =====================================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Reportes_TopServicios]
    @desde DATE,
    @hasta DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.serv_Nombre                                AS Servicio,
        CAST(SUM(fd.fdet_Cantidad) AS DECIMAL(18,2)) AS Cantidad,
        CAST(SUM(fd.fdet_Subtotal) AS DECIMAL(18,2)) AS Ingreso
    FROM [Veterinaria].[tbFacturaDetalle] fd
    INNER JOIN [Veterinaria].[tbFacturas]  f ON fd.fact_Id = f.fact_Id
    INNER JOIN [Veterinaria].[tbServicios] s ON fd.serv_Id = s.serv_Id
    WHERE fd.fdet_Tipo = 'Servicio'
      AND f.fact_EsEliminado = 0
      AND f.fact_Estado <> 'Anulada'
      AND CAST(f.fact_Fecha AS DATE) BETWEEN @desde AND @hasta
    GROUP BY s.serv_Nombre
    ORDER BY Cantidad DESC;
END
GO

-- =====================================================
-- 5. StockBajo: items donde el stock actual <= stock minimo
--    Stock actual = SUM(exi_Cantidad) de tbExistencias (sumado entre refugios)
--    (solo stock bajo; PetsHome no tiene caducidad todavia)
-- =====================================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Reportes_StockBajo]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        i.itm_Descripcion                               AS Item,
        CAST(ISNULL(SUM(e.exi_Cantidad), 0) AS INT)     AS Stock,
        i.itm_StockMinimo                               AS Minimo
    FROM [Inventario].[tbItems] i
    LEFT JOIN [Inventario].[tbExistencias] e ON i.itm_Id = e.itm_Id
    WHERE i.itm_EsEliminado = 0
    GROUP BY i.itm_Id, i.itm_Descripcion, i.itm_StockMinimo
    HAVING ISNULL(SUM(e.exi_Cantidad), 0) <= i.itm_StockMinimo
    ORDER BY (i.itm_StockMinimo - ISNULL(SUM(e.exi_Cantidad), 0)) DESC,
             Stock ASC;
END
GO

PRINT 'Script 89: 5 SPs de Reportes creados';
GO
