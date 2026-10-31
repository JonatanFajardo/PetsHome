/* =====================================================================
   94c_SEED_VETERINARIA.sql  -  Seed ampliado, dominio VETERINARIA (PetsHome)
   Tablas: tbClientes, Refugio.tbMascotas (de clientes), tbFacturas(+Detalle),
           tbPagos, tbCargoDispensacion, tbSalidasInventario
   Orden: clientes -> mascotas -> facturas -> detalle -> totales -> pagos ->
          monto pagado -> cargos -> salidas.
   DEPENDE de 94a (recetas [SEED94]) solo para tbCargoDispensacion; si no se
   corrio 94a, ese bloque simplemente no inserta nada.
   Idempotente: por cli_Identidad, marca [SEED94] o fact_Numero 'SEED94-%'.
   ===================================================================== */
SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRAN;

/* Valores de texto/codigo que NO vienen en el esquema (SUPUESTOS - ajustar) */
DECLARE @stPagada   VARCHAR(20) = 'Pagada';
DECLARE @stParcial  VARCHAR(20) = 'PorCobrar';
DECLARE @stPend     VARCHAR(20) = 'PorCobrar';
DECLARE @tS         VARCHAR(20) = 'Servicio';   -- fdet_Tipo servicio
DECLARE @tP         VARCHAR(20) = 'Producto';   -- fdet_Tipo producto
DECLARE @cargoOrigen NVARCHAR(30) = 'Medicamento';   -- cargo_TipoOrigen
DECLARE @mascOrigen INT = 1;                    -- masc_Origen para mascota de cliente
DECLARE @iva        DECIMAL(5,2) = 0.15;        -- impuesto

/* ---------- 1. CLIENTES (+6) ---------- */
INSERT INTO Veterinaria.tbClientes
 (cli_Identidad,cli_Nombres,cli_Apellidos,cli_Telefono,cli_Correo,cli_Direccion,
  cli_EsConsumidorFinal,cli_EsEliminado,cli_UsuarioCrea,cli_FechaCrea)
SELECT v.ident,v.nom,v.ape,v.tel,v.correo,v.dir,0,0,1,DATEADD(DAY,v.dias,GETDATE())
FROM (VALUES
 ('0501-1992-04512','Patricia','Mejia','9876-1020','patricia.mejia@correo.com','Col. Rio de Piedras, San Pedro Sula',-70),
 ('0501-1988-07733','Jorge','Castillo','9945-3341','jorge.castillo@correo.com','Barrio Guamilito, San Pedro Sula',-65),
 ('0502-1995-01290','Daniela','Flores','3312-8870','daniela.flores@correo.com','Res. Los Alamos, Choloma',-60),
 ('0501-1985-09944','Ricardo','Bonilla','9788-4415','ricardo.bonilla@correo.com','Col. Sula, San Pedro Sula',-55),
 ('0501-2000-03087','Karla','Zelaya','8890-2236','karla.zelaya@correo.com','Barrio Suyapa, San Pedro Sula',-50),
 ('0510-1979-00561','Fernando','Aguilar','9623-7710','fernando.aguilar@correo.com','Col. Jardines del Valle, San Pedro Sula',-45)
) v(ident,nom,ape,tel,correo,dir,dias)
WHERE NOT EXISTS (SELECT 1 FROM Veterinaria.tbClientes c WHERE c.cli_Identidad = v.ident);

/* ---------- 2. MASCOTAS DE CLIENTES (+6) ----------
   raza_Id, tall_Id, proc_Id van NULL (catalogos desconocidos). refg_Id NULL = mascota externa. */
IF NOT EXISTS (SELECT 1 FROM Refugio.tbMascotas WHERE masc_Historia LIKE '[[]SEED94]%')
  INSERT INTO Refugio.tbMascotas
   (masc_Nombre,masc_Edad,masc_Sexo,masc_Peso,masc_Color,masc_Historia,refg_Id,
    masc_EsAdoptado,masc_EsReservado,masc_EsEliminado,masc_UsuarioCrea,masc_FechaCrea,masc_Origen,cli_Id)
  SELECT v.nom,v.edad,v.sexo,v.peso,v.color,'[SEED94] Mascota de cliente externo',NULL,
         0,0,0,1,DATEADD(DAY,v.dias,GETDATE()),@mascOrigen,
         (SELECT TOP 1 c.cli_Id FROM Veterinaria.tbClientes c WHERE c.cli_Identidad = v.ident)
  FROM (VALUES
   ('Nala',2,'H',3.80,'Gris','0501-1992-04512',-68),
   ('Rex',4,'M',28.50,'Negro y fuego','0501-1988-07733',-63),
   ('Mia',1,'H',3.20,'Blanco','0502-1995-01290',-58),
   ('Chispa',3,'H',9.40,'Cafe','0501-1985-09944',-53),
   ('Oso',5,'M',34.00,'Marron','0501-2000-03087',-48),
   ('Kira',2,'H',21.50,'Dorado','0510-1979-00561',-43)
  ) v(nom,edad,sexo,peso,color,ident,dias);

/* IDs de los clientes/mascotas nuevos que usan las facturas 9 y 10 */
DECLARE @cPat INT = (SELECT cli_Id FROM Veterinaria.tbClientes WHERE cli_Identidad='0501-1992-04512');
DECLARE @mNal INT = (SELECT TOP 1 masc_Id FROM Refugio.tbMascotas WHERE masc_Nombre='Nala' AND cli_Id=@cPat AND masc_Historia LIKE '[[]SEED94]%');
DECLARE @cDan INT = (SELECT cli_Id FROM Veterinaria.tbClientes WHERE cli_Identidad='0502-1995-01290');
DECLARE @mMia INT = (SELECT TOP 1 masc_Id FROM Refugio.tbMascotas WHERE masc_Nombre='Mia' AND cli_Id=@cDan AND masc_Historia LIKE '[[]SEED94]%');

/* ---------- 3. FACTURAS (10) + DETALLE + PAGOS ---------- */
IF NOT EXISTS (SELECT 1 FROM Veterinaria.tbFacturas WHERE fact_Numero LIKE 'SEED94-%')
BEGIN
  -- Encabezados (totales en 0; se recalculan desde el detalle)
  INSERT INTO Veterinaria.tbFacturas
   (fact_Numero,cli_Id,masc_Id,refg_Id,fact_Fecha,fact_Subtotal,fact_Impuesto,fact_Total,fact_Estado,
    fact_EsEliminado,fact_UsuarioCrea,fact_FechaCrea,fact_Vencimiento,fact_MontoPagado)
  SELECT v.num,v.cli,v.masc,v.refg,DATEADD(DAY,v.dias,GETDATE()),0,0,0,v.estado,
         0,1,DATEADD(DAY,v.dias,GETDATE()),
         CASE WHEN v.estado = @stPagada THEN NULL ELSE CAST(DATEADD(DAY,v.dias+30,GETDATE()) AS date) END,0
  FROM (VALUES
   ('SEED94-F001',3,22,1,-40,@stPagada),
   ('SEED94-F002',4,23,1,-35,@stPagada),
   ('SEED94-F003',7,26,2,-30,@stPagada),
   ('SEED94-F004',2,27,1,-25,@stParcial),
   ('SEED94-F005',2,21,1,-20,@stPagada),
   ('SEED94-F006',5,24,2,-8, @stPend),
   ('SEED94-F007',6,25,2,-4, @stParcial),
   ('SEED94-F008',1,NULL,1,-3,@stPagada),
   ('SEED94-F009',@cPat,@mNal,1,-2,@stPagada),
   ('SEED94-F010',@cDan,@mMia,2,-1,@stPend)
  ) v(num,cli,masc,refg,dias,estado);

  -- Detalle
  INSERT INTO Veterinaria.tbFacturaDetalle
   (fact_Id,fdet_Tipo,serv_Id,itm_Id,fdet_Descripcion,fdet_Cantidad,fdet_PrecioUnit,fdet_Subtotal)
  SELECT f.fact_Id,v.tipo,v.serv,v.itm,v.descr,v.cant,v.precio,v.cant*v.precio
  FROM (VALUES
   ('SEED94-F001',@tS,8,NULL,'Consulta general',1,300.00),
   ('SEED94-F001',@tP,NULL,7,'Antibiotico Amoxicilina 100ml',1,250.00),
   ('SEED94-F002',@tS,5,NULL,'Hemograma completo',1,600.00),
   ('SEED94-F002',@tS,9,NULL,'Desparasitacion',1,200.00),
   ('SEED94-F002',@tP,NULL,6,'Antiparasitario Interno 50ml',1,320.00),
   ('SEED94-F003',@tS,8,NULL,'Consulta general',1,300.00),
   ('SEED94-F003',@tS,7,NULL,'Coprologico',1,250.00),
   ('SEED94-F003',@tP,NULL,7,'Antibiotico Amoxicilina 100ml',1,250.00),
   ('SEED94-F004',@tS,3,NULL,'Vacuna Quintuple',1,450.00),
   ('SEED94-F004',@tP,NULL,8,'Antipulgas Topico x3 pipetas',1,520.00),
   ('SEED94-F005',@tS,8,NULL,'Consulta general',1,300.00),
   ('SEED94-F005',@tS,7,NULL,'Coprologico',1,250.00),
   ('SEED94-F005',@tP,NULL,6,'Antiparasitario Interno 50ml',1,320.00),
   ('SEED94-F006',@tS,8,NULL,'Consulta general',1,300.00),
   ('SEED94-F006',@tS,5,NULL,'Hemograma completo',1,600.00),
   ('SEED94-F006',@tP,NULL,8,'Antipulgas Topico x3 pipetas',1,520.00),
   ('SEED94-F007',@tS,8,NULL,'Consulta general',1,300.00),
   ('SEED94-F007',@tS,6,NULL,'Quimica sanguinea',1,850.00),
   ('SEED94-F007',@tP,NULL,7,'Antibiotico Amoxicilina 100ml',1,250.00),
   ('SEED94-F008',@tP,NULL,1,'Concentrado Premium Adulto 15kg',1,1850.00),
   ('SEED94-F008',@tP,NULL,9,'Pelota de Goma Mediana',2,60.00),
   ('SEED94-F009',@tS,2,NULL,'Vacuna Rabia',1,350.00),
   ('SEED94-F010',@tS,4,NULL,'Vacuna Triple Felina',1,420.00),
   ('SEED94-F010',@tS,8,NULL,'Consulta general',1,300.00)
  ) v(num,tipo,serv,itm,descr,cant,precio)
  JOIN Veterinaria.tbFacturas f ON f.fact_Numero = v.num;

  -- Totales desde el detalle
  UPDATE f SET
     fact_Subtotal = t.sub,
     fact_Impuesto = ROUND(t.sub*@iva,2),
     fact_Total    = t.sub + ROUND(t.sub*@iva,2)
  FROM Veterinaria.tbFacturas f
  JOIN (SELECT fact_Id, SUM(fdet_Subtotal) AS sub FROM Veterinaria.tbFacturaDetalle GROUP BY fact_Id) t
    ON t.fact_Id = f.fact_Id
  WHERE f.fact_Numero LIKE 'SEED94-%';

  -- Pagos: monto NULL = total completo; monto negativo = total menos ese valor (saldo)
  INSERT INTO Veterinaria.tbPagos
   (fact_Id,pago_Monto,pago_Metodo,pago_RecibidoPor,pago_Fecha,pago_Referencia,pago_Notas,
    pago_EsEliminado,pago_UsuarioCrea,pago_FechaCrea)
  SELECT f.fact_Id,
         CASE WHEN v.monto IS NULL THEN f.fact_Total
              WHEN v.monto < 0 THEN f.fact_Total + v.monto
              ELSE v.monto END,
         v.metodo,7,DATEADD(DAY,v.offd,f.fact_Fecha),v.ref,v.notas,0,1,DATEADD(DAY,v.offd,f.fact_Fecha)
  FROM (VALUES
   ('SEED94-F001',NULL,'Efectivo',NULL,'Pago completo',0),
   ('SEED94-F002',NULL,'TarjetaCredito','TRJ-48211','Pago completo',0),
   ('SEED94-F003',200.00,'Efectivo',NULL,'Abono inicial',0),
   ('SEED94-F003',-200.00,'Transferencia','TRF-77120','Saldo cancelado',3),
   ('SEED94-F004',400.00,'Efectivo',NULL,'Abono, queda saldo',0),
   ('SEED94-F005',NULL,'Efectivo',NULL,'Pago completo',0),
   ('SEED94-F007',600.00,'TarjetaCredito','TRJ-50934','Abono, queda saldo',0),
   ('SEED94-F008',NULL,'Efectivo',NULL,'Venta de mostrador',0),
   ('SEED94-F009',NULL,'Transferencia','TRF-78005','Pago completo',0)
  ) v(num,monto,metodo,ref,notas,offd)
  JOIN Veterinaria.tbFacturas f ON f.fact_Numero = v.num;

  -- Monto pagado = suma de pagos
  UPDATE f SET fact_MontoPagado = ISNULL(p.pagado,0)
  FROM Veterinaria.tbFacturas f
  LEFT JOIN (SELECT fact_Id, SUM(pago_Monto) AS pagado FROM Veterinaria.tbPagos
             WHERE pago_EsEliminado = 0 GROUP BY fact_Id) p ON p.fact_Id = f.fact_Id
  WHERE f.fact_Numero LIKE 'SEED94-%';
END

/* ---------- 4. CARGOS DE DISPENSACION (hasta 8, desde recetas [SEED94] de 94a) ----------
   Si la receta tiene su producto en una factura del mismo cliente+mascota queda
   'Facturado' y enlazada; si no, 'Pendiente'. */
IF NOT EXISTS (SELECT 1 FROM Veterinaria.tbCargoDispensacion c
               JOIN Medico.tbRecetas r ON r.receta_Id = c.receta_Id
               WHERE r.receta_Instrucciones LIKE '[[]SEED94]%')
  INSERT INTO Veterinaria.tbCargoDispensacion
   (receta_Id,masc_Id,cli_Id,cita_Id,itm_Id,cargo_Cantidad,cargo_DescripcionSnap,cargo_PrecioUnitSnap,
    cargo_Estado,fact_Id,fdet_Id,cargo_ResueltoPor,cargo_FechaResuelto,cargo_MotivoResolucion,
    cargo_EsEliminado,cargo_UsuarioCrea,cargo_FechaCrea,cargo_TipoOrigen,cargo_OrigenId)
  SELECT r.receta_Id,r.masc_Id,m.cli_Id,r.cita_Id,r.itm_Id,1,
         CASE r.itm_Id WHEN 6 THEN 'Antiparasitario Interno 50ml'
                       WHEN 7 THEN 'Antibiotico Amoxicilina 100ml'
                       WHEN 8 THEN 'Antipulgas Topico x3 pipetas' END,
         CASE r.itm_Id WHEN 6 THEN 320.00 WHEN 7 THEN 250.00 WHEN 8 THEN 520.00 END,
         CASE WHEN d.fdet_Id IS NULL THEN 'Pendiente' ELSE 'Facturado' END,
         CASE WHEN d.fdet_Id IS NULL THEN NULL ELSE f.fact_Id END, d.fdet_Id,
         CASE WHEN d.fdet_Id IS NULL THEN NULL ELSE 7 END,
         CASE WHEN d.fdet_Id IS NULL THEN NULL ELSE f.fact_Fecha END,
         NULL,  -- cargo_MotivoResolucion: CK_cargo_Shape exige NULL salvo 'Condonado'
         0,1,r.receta_FechaCrea,@cargoOrigen,r.receta_Id
  FROM Medico.tbRecetas r
  JOIN Refugio.tbMascotas m ON m.masc_Id = r.masc_Id AND m.cli_Id IS NOT NULL
  LEFT JOIN Veterinaria.tbFacturas f ON f.cli_Id = m.cli_Id AND f.masc_Id = r.masc_Id
                                    AND f.fact_Numero LIKE 'SEED94-%'
  LEFT JOIN Veterinaria.tbFacturaDetalle d ON d.fact_Id = f.fact_Id AND d.itm_Id = r.itm_Id
  WHERE r.receta_Instrucciones LIKE '[[]SEED94]%';

/* ---------- 5. SALIDAS DE INVENTARIO (veterinaria) ----------
   Una por cada producto vendido en facturas SEED94 + 2 salidas sin factura. */
IF NOT EXISTS (SELECT 1 FROM Veterinaria.tbSalidasInventario WHERE sal_Motivo LIKE '%[[]SEED94]')
BEGIN
  INSERT INTO Veterinaria.tbSalidasInventario
   (itm_Id,fact_Id,sal_Cantidad,sal_Motivo,sal_Fecha,sal_EsEliminado,sal_UsuarioCrea,sal_FechaCrea)
  SELECT d.itm_Id,f.fact_Id,d.fdet_Cantidad,'Venta [SEED94]',f.fact_Fecha,0,1,f.fact_FechaCrea
  FROM Veterinaria.tbFacturaDetalle d
  JOIN Veterinaria.tbFacturas f ON f.fact_Id = d.fact_Id
  WHERE f.fact_Numero LIKE 'SEED94-%' AND d.itm_Id IS NOT NULL;

  INSERT INTO Veterinaria.tbSalidasInventario
   (itm_Id,fact_Id,sal_Cantidad,sal_Motivo,sal_Fecha,sal_EsEliminado,sal_UsuarioCrea,sal_FechaCrea)
  VALUES
   (15,NULL,1,'Clinica [SEED94]',DATEADD(DAY,-14,GETDATE()),0,1,DATEADD(DAY,-14,GETDATE())),
   (7,NULL,2,'Danado [SEED94]',DATEADD(DAY,-9,GETDATE()),0,1,DATEADD(DAY,-9,GETDATE()));
END

COMMIT;
PRINT 'Seed VETERINARIA (94c) completado.';
