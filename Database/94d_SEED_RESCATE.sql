/* =====================================================================
   94d_SEED_RESCATE.sql  -  Seed ampliado, dominio RESCATE (PetsHome)
   Tablas: Rescate.tbReportesAbandono (10), Rescate.tbIngresos (8)
   El catalogo de tipos de reporte (reptip_Id) se descubre solo por la FK de
   tbReportesAbandono.reptip_Id; toma hasta 3 tipos existentes y los reparte.
   Si no hay FK o el catalogo esta vacio, el script falla con un mensaje claro.
   Idempotente: marca [SEED94] en repa_Observaciones / ingr_Observaciones.
   ===================================================================== */
SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRAN;

/* Texto de estado (SUPUESTO - ajustar si el sistema usa otros valores) */
DECLARE @stPend    VARCHAR(30) = 'Pendiente';
DECLARE @stAtend   VARCHAR(30) = 'En atencion';
DECLARE @stCerrado VARCHAR(30) = 'Atendido';

/* ---------- Descubrir catalogo de tipos de reporte ---------- */
DECLARE @tabla NVARCHAR(300), @col SYSNAME;
SELECT @tabla = QUOTENAME(OBJECT_SCHEMA_NAME(fkc.referenced_object_id)) + N'.' + QUOTENAME(OBJECT_NAME(fkc.referenced_object_id)),
       @col   = COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id)
FROM sys.foreign_key_columns fkc
WHERE fkc.parent_object_id = OBJECT_ID('Rescate.tbReportesAbandono')
  AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'reptip_Id';

IF @tabla IS NULL
  THROW 50001, 'No se encontro FK de reptip_Id en Rescate.tbReportesAbandono. Indica la tabla de tipos de reporte.', 1;

CREATE TABLE #tipos (rn INT IDENTITY(1,1), id INT NOT NULL);
DECLARE @sql NVARCHAR(MAX) = N'INSERT INTO #tipos(id) SELECT TOP 3 ' + QUOTENAME(@col) + N' FROM ' + @tabla + N' ORDER BY 1';
EXEC (@sql);
DECLARE @n INT = (SELECT COUNT(*) FROM #tipos);
IF @n = 0
  THROW 50002, 'El catalogo de tipos de reporte esta vacio.', 1;

/* ---------- 1. REPORTES DE ABANDONO (10) ---------- */
IF NOT EXISTS (SELECT 1 FROM Rescate.tbReportesAbandono WHERE repa_Observaciones LIKE '[[]SEED94]%')
  INSERT INTO Rescate.tbReportesAbandono
   (reptip_Id,repa_NombreReportante,repa_TelefonoContacto,repa_Email,repa_FechaReporte,repa_UbicacionIncidente,
    repa_DescripcionAnimal,repa_EstadoAtencion,repa_Observaciones,repa_EsAnonimo,refg_Id,
    repa_EsEliminado,repa_UsuarioCrea,repa_FechaCrea)
  SELECT t.id,v.nom,v.tel,v.mail,DATEADD(DAY,v.dias,GETDATE()),v.ubic,v.descr,v.estado,v.obs,
         CASE WHEN v.nom IS NULL THEN 1 ELSE 0 END,v.refg,0,1,DATEADD(DAY,v.dias,GETDATE())
  FROM (VALUES
   (1,'Maria Gonzalez','9801-2233','maria.gonzalez@correo.com',-90,'Colonia Rivera Hernandez, cerca del mercado','Perro mediano color cafe, muy delgado',@stCerrado,'[SEED94] R01 - Reporte atendido en el dia',1),
   (2,NULL,NULL,NULL,-80,'Bulevar del Sur, frente a gasolinera','Gata con tres gatitos recien nacidos',@stCerrado,'[SEED94] R02 - Reporte anonimo',2),
   (3,'Pedro Lopez','9712-4455',NULL,-70,'Barrio Medina, 5 calle','Cachorro negro atropellado, cojea',@stCerrado,'[SEED94] R03 - Emergencia traslado inmediato',1),
   (4,NULL,NULL,NULL,-60,'Colonia Satelite','Perro grande atado sin agua ni sombra',@stCerrado,'[SEED94] R04 - Reporte anonimo',3),
   (5,'Sandra Vasquez','3345-9012','sandra.vasquez@correo.com',-50,'Zona universitaria','Gato atigrado con herida en la oreja',@stCerrado,'[SEED94] R05 - Herida superficial',2),
   (6,'Luis Mendoza','9988-1170',NULL,-40,'Aldea El Progreso, lote baldio','Perra con cria en lote baldio',@stCerrado,'[SEED94] R06 - Madre y 4 cachorros',3),
   (7,'Ana Torres','9654-3321','ana.torres@correo.com',-25,'Colonia Los Andes','Perro con sarna visible, pelo escaso',@stAtend,'[SEED94] R07 - En tratamiento',1),
   (8,NULL,NULL,NULL,-12,'Parque Central','Gato joven desorientado',@stAtend,'[SEED94] R08 - Reporte anonimo, en seguimiento',2),
   (9,'Carlos Ramos','8877-6543',NULL,-5,'Barrio Lempira','Cachorro abandonado dentro de una caja',@stPend,'[SEED94] R09 - Sin atender aun',1),
   (10,'Rosa Paz','9421-0098','rosa.paz@correo.com',-2,'Carretera a Choloma km 5','Perro herido a orilla de la via',@stPend,'[SEED94] R10 - Posible emergencia',3)
  ) v(k,nom,tel,mail,dias,ubic,descr,estado,obs,refg)
  JOIN #tipos t ON t.rn = ((v.k - 1) % @n) + 1;

/* ---------- 2. INGRESOS (8): 7 enlazados a reportes + 1 entrega directa ---------- */
IF NOT EXISTS (SELECT 1 FROM Rescate.tbIngresos WHERE ingr_Observaciones LIKE '[[]SEED94]%')
  INSERT INTO Rescate.tbIngresos
   (repa_Id,refg_Id,ingr_FechaIngreso,ingr_LugarRescate,ingr_CondicionInicial,ingr_PersonaRescatista,
    ingr_MedioTransporte,ingr_Observaciones,ingr_EsEmergencia,ingr_EsEliminado,ingr_UsuarioCrea,ingr_FechaCrea)
  SELECT r.repa_Id,
         ISNULL(r.refg_Id,v.refg),
         CASE WHEN r.repa_Id IS NULL THEN DATEADD(DAY,-15,GETDATE()) ELSE DATEADD(DAY,1,r.repa_FechaReporte) END,
         v.lugar,v.cond,v.persona,v.medio,v.obs,v.emerg,0,1,
         CASE WHEN r.repa_Id IS NULL THEN DATEADD(DAY,-15,GETDATE()) ELSE DATEADD(DAY,1,r.repa_FechaReporte) END
  FROM (VALUES
   ('01',1,'Colonia Rivera Hernandez','Desnutrido, deshidratado','Voluntario Refugio Las Lomas','Vehiculo del refugio','[SEED94] Ingreso estable, inicia dieta',0),
   ('02',2,'Bulevar del Sur','Madre estable, gatitos sanos','Equipo de rescate','Vehiculo del refugio','[SEED94] Se ingresa a la familia completa',0),
   ('03',1,'Barrio Medina','Fractura en pata trasera','Equipo de rescate','Vehiculo del refugio','[SEED94] Traslado inmediato a clinica',1),
   ('04',3,'Colonia Satelite','Deshidratado, sin heridas','Voluntaria Vida Animal','Vehiculo particular','[SEED94] Recuperacion rapida',0),
   ('05',2,'Zona universitaria','Herida leve en oreja','Equipo de rescate','Vehiculo del refugio','[SEED94] Curacion y antibiotico',0),
   ('06',3,'Aldea El Progreso','Madre delgada, cachorros de 3 semanas','Equipo de rescate','Vehiculo del refugio','[SEED94] Ingresan madre y 4 cachorros',0),
   ('07',1,'Colonia Los Andes','Sarna severa, pelo escaso','Voluntario Refugio Las Lomas','Vehiculo del refugio','[SEED94] En tratamiento dermatologico',0),
   (NULL,1,'Entrega directa en el refugio','Buen estado general','Familia que no puede conservarlo','A pie','[SEED94] Entrega voluntaria sin reporte previo',0)
  ) v(k,refg,lugar,cond,persona,medio,obs,emerg)
  LEFT JOIN Rescate.tbReportesAbandono r ON v.k IS NOT NULL AND r.repa_Observaciones LIKE '[[]SEED94] R' + v.k + ' -%';

DROP TABLE #tipos;
COMMIT;
PRINT 'Seed RESCATE (94d) completado.';
