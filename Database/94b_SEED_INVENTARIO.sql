/* =====================================================================
   94b_SEED_INVENTARIO.sql  -  Seed ampliado, dominio INVENTARIO (PetsHome)
   Tablas: tbRecepcionesMercancia(+Detalles), tbEntradas(+Detalles),
           tbSalidas(+Detalles), tbMovimientos
   Idempotente: recepciones por recep_NumeroDocumento 'SEED94-%';
                entradas/salidas/movimientos por marca [SEED94].
   AJUSTAR si hace falta (codigos de tipo que no vienen en el esquema):
   ===================================================================== */
SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRAN;

DECLARE @recCompra   CHAR(1) = 'C';  -- tipo recepcion: compra     (SUPUESTO)
DECLARE @recDonacion CHAR(1) = 'D';  -- tipo recepcion: donacion   (SUPUESTO)
DECLARE @salUso      CHAR(1) = 'U';  -- tipo salida: uso interno   (SUPUESTO)
DECLARE @movEntrada  CHAR(1) = 'E';  -- tipo movimiento: entrada   (SUPUESTO)
DECLARE @movSalida   CHAR(1) = 'S';  -- tipo movimiento: salida    (SUPUESTO)
DECLARE @saldoBase   INT     = 0;    -- saldo inicial asumido por item/refugio

/* ---------- 1. RECEPCIONES DE MERCANCIA (5) + DETALLES ---------- */
IF NOT EXISTS (SELECT 1 FROM Inventario.tbRecepcionesMercancia WHERE recep_NumeroDocumento LIKE 'SEED94-%')
BEGIN
  INSERT INTO Inventario.tbRecepcionesMercancia
   (recep_Descripcion,recep_Fecha,refg_Id,recep_EsEliminado,recep_UsuarioCrea,recep_FechaCrea,
    recep_TipoRecepcion,recep_OrigenId,recep_NumeroDocumento)
  VALUES
   ('Compra mensual de concentrados',DATEADD(DAY,-62,GETDATE()),1,0,1,DATEADD(DAY,-62,GETDATE()),@recCompra,NULL,'SEED94-001'),
   ('Compra de medicinas veterinarias',DATEADD(DAY,-57,GETDATE()),2,0,1,DATEADD(DAY,-57,GETDATE()),@recCompra,NULL,'SEED94-002'),
   ('Donacion de alimento de empresa local',DATEADD(DAY,-48,GETDATE()),3,0,1,DATEADD(DAY,-48,GETDATE()),@recDonacion,NULL,'SEED94-003'),
   ('Compra de accesorios y juguetes',DATEADD(DAY,-33,GETDATE()),1,0,1,DATEADD(DAY,-33,GETDATE()),@recCompra,NULL,'SEED94-004'),
   ('Donacion de antiparasitarios',DATEADD(DAY,-18,GETDATE()),2,0,1,DATEADD(DAY,-18,GETDATE()),@recDonacion,NULL,'SEED94-005');

  INSERT INTO Inventario.tbRecepcionesDetalles
   (recep_Id,itm_Id,recdet_Cantidad,recdet_EsEliminado,recdet_UsuarioCrea,recdet_FechaCrea,
    recdet_PrecioUnitario,recdet_FechaVencimiento,recdet_NumeroLote)
  SELECT r.recep_Id, v.itm, v.cant, 0, 1, r.recep_FechaCrea, v.precio,
         CASE WHEN v.venc IS NULL THEN NULL ELSE CAST(DATEADD(DAY,v.venc,GETDATE()) AS date) END, v.lote
  FROM (VALUES
   ('SEED94-001',1,20,1450.00,300,'LT-C001'),
   ('SEED94-001',2,15,780.00,280,'LT-C002'),
   ('SEED94-001',4,20,720.00,320,'LT-C003'),
   ('SEED94-002',6,30,210.00,400,'LT-M001'),
   ('SEED94-002',7,25,165.00,360,'LT-M002'),
   ('SEED94-002',8,20,380.00,420,'LT-M003'),
   ('SEED94-003',3,40,420.00,200,'LT-D001'),
   ('SEED94-003',5,15,390.00,240,'LT-D002'),
   ('SEED94-004',9,12,45.00,NULL,NULL),
   ('SEED94-004',12,10,85.00,NULL,NULL),
   ('SEED94-004',13,6,210.00,NULL,NULL),
   ('SEED94-005',6,20,0.00,380,'LT-D003'),
   ('SEED94-005',8,15,0.00,400,'LT-D004')
  ) v(doc,itm,cant,precio,venc,lote)
  JOIN Inventario.tbRecepcionesMercancia r ON r.recep_NumeroDocumento = v.doc;
END

/* ---------- 2. ENTRADAS (5) + DETALLES + SALIDAS (5) + DETALLES + MOVIMIENTOS ---------- */
IF NOT EXISTS (SELECT 1 FROM Inventario.tbEntradas WHERE ent_Descripcion LIKE '[[]SEED94]%')
BEGIN
  -- Entradas
  INSERT INTO Inventario.tbEntradas (ent_Descripcion,refg_Id,ent_Fecha,ent_EsEliminado,ent_UsuarioCrea,ent_FechaCrea)
  VALUES
   ('[SEED94] Entrada 1 - Reposicion concentrados y antiparasitario',1,DATEADD(DAY,-60,GETDATE()),0,1,DATEADD(DAY,-60,GETDATE())),
   ('[SEED94] Entrada 2 - Reposicion alimento gato y antibiotico',2,DATEADD(DAY,-55,GETDATE()),0,1,DATEADD(DAY,-55,GETDATE())),
   ('[SEED94] Entrada 3 - Alimento humedo, antipulgas y champu',3,DATEADD(DAY,-50,GETDATE()),0,1,DATEADD(DAY,-50,GETDATE())),
   ('[SEED94] Entrada 4 - Antibiotico y antipulgas',1,DATEADD(DAY,-45,GETDATE()),0,1,DATEADD(DAY,-45,GETDATE())),
   ('[SEED94] Entrada 5 - Concentrado y antiparasitario',2,DATEADD(DAY,-40,GETDATE()),0,1,DATEADD(DAY,-40,GETDATE()));

  INSERT INTO Inventario.tbEntradasDetalles
   (ent_Id,itm_Id,entdet_Cantidad,entdet_EsEliminado,entdet_UsuarioCrea,entdet_FechaCrea)
  SELECT e.ent_Id, v.itm, v.cant, 0, 1, e.ent_FechaCrea
  FROM (VALUES
   (1,1,20),(1,2,15),(1,6,30),
   (2,4,20),(2,5,15),(2,7,25),
   (3,3,40),(3,8,20),(3,15,12),
   (4,7,20),(4,8,15),
   (5,1,10),(5,6,20)
  ) v(k,itm,cant)
  JOIN Inventario.tbEntradas e ON e.ent_Descripcion LIKE '[[]SEED94] Entrada '+CAST(v.k AS varchar(3))+' -%';

  -- Las entradas NO tienen trigger; subimos existencias a mano para que las
  -- salidas tengan stock (el trigger de salidas valida stock disponible).
  MERGE Inventario.tbExistencias AS e
  USING (
    SELECT d.itm_Id, en.refg_Id, SUM(d.entdet_Cantidad) AS cant
    FROM Inventario.tbEntradas en
    JOIN Inventario.tbEntradasDetalles d ON d.ent_Id = en.ent_Id
    WHERE en.ent_Descripcion LIKE '[[]SEED94]%'
    GROUP BY d.itm_Id, en.refg_Id
  ) AS src ON e.itm_Id = src.itm_Id AND e.refg_Id = src.refg_Id
  WHEN MATCHED THEN
    UPDATE SET e.exi_Cantidad = e.exi_Cantidad + src.cant, e.exi_UltimaActualizacion = GETDATE()
  WHEN NOT MATCHED THEN
    INSERT (itm_Id, refg_Id, exi_Cantidad, exi_UltimaActualizacion)
    VALUES (src.itm_Id, src.refg_Id, src.cant, GETDATE());

  -- Salidas
  INSERT INTO Inventario.tbSalidas (sal_Descripcion,sal_TipoSalida,refg_Id,sal_Fecha,sal_EsEliminado,sal_UsuarioCrea,sal_FechaCrea)
  VALUES
   ('[SEED94] Salida 1 - Consumo interno refugio',@salUso,1,DATEADD(DAY,-30,GETDATE()),0,1,DATEADD(DAY,-30,GETDATE())),
   ('[SEED94] Salida 2 - Tratamientos y alimentacion',@salUso,2,DATEADD(DAY,-25,GETDATE()),0,1,DATEADD(DAY,-25,GETDATE())),
   ('[SEED94] Salida 3 - Alimentacion y limpieza',@salUso,3,DATEADD(DAY,-20,GETDATE()),0,1,DATEADD(DAY,-20,GETDATE())),
   ('[SEED94] Salida 4 - Tratamientos',@salUso,1,DATEADD(DAY,-12,GETDATE()),0,1,DATEADD(DAY,-12,GETDATE())),
   ('[SEED94] Salida 5 - Consumo semanal',@salUso,2,DATEADD(DAY,-6,GETDATE()),0,1,DATEADD(DAY,-6,GETDATE()));

  INSERT INTO Inventario.tbSalidasDetalles
   (sal_Id,itm_Id,saldet_Cantidad,saldet_Observaciones,saldet_EsEliminado,saldet_UsuarioCrea,saldet_FechaCrea,
    saldet_RecetaId,saldet_PrecioUnitario,saldet_DescripcionItem)
  SELECT s.sal_Id, v.itm, v.cant, v.obs, 0, 1, s.sal_FechaCrea,
         NULL,  -- saldet_RecetaId NULL: hay indice unico UQ_saldet_Receta; no enlazar a receta aqui
         v.precio, v.descr
  FROM (VALUES
   (1,1, 3,'Alimentacion diaria',0,1450.00,'Concentrado Premium Adulto 15kg'),
   (1,6, 5,'Desparasitacion general',0,210.00,'Antiparasitario Interno 50ml'),
   (2,4, 4,'Alimentacion de gatos',0,720.00,'Concentrado Gato Adulto 8kg'),
   (2,7, 6,'Tratamiento con receta',1,165.00,'Antibiotico Amoxicilina 100ml'),
   (3,3, 8,'Alimentacion de cachorros',0,420.00,'Alimento Humedo Latas x12'),
   (3,15,2,'Banos de rutina',0,150.00,'Champu Antipulgas 500ml'),
   (4,7, 4,'Tratamiento con receta',1,165.00,'Antibiotico Amoxicilina 100ml'),
   (4,8, 3,'Control de pulgas',0,380.00,'Antipulgas Topico x3 pipetas'),
   (5,1, 2,'Alimentacion diaria',0,1450.00,'Concentrado Premium Adulto 15kg'),
   (5,6, 4,'Desparasitacion',0,210.00,'Antiparasitario Interno 50ml')
  ) v(k,itm,cant,obs,rec,precio,descr)
  JOIN Inventario.tbSalidas s ON s.sal_Descripcion LIKE '[[]SEED94] Salida '+CAST(v.k AS varchar(3))+' -%';

  -- NOTA: las SALIDAS ya generan su movimiento de inventario via el trigger
  -- TR_RegistrarMovimiento_Salida. No insertamos movimientos a mano (duplicaria).
END

COMMIT;
PRINT 'Seed INVENTARIO (94b) completado.';
