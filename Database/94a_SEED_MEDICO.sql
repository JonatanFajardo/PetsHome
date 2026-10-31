/* =====================================================================
   94a_SEED_MEDICO.sql  -  Seed ampliado, dominio MEDICO (PetsHome)
   Tablas: tbRecetas, tbTratamientos, tbMascotaAlergia, tbMascotaPadecimiento,
           tbConsentimiento, tbSustanciaMovimiento, tbLaboratorio(+Detalle)
   Idempotente: cada bloque se omite si ya existen filas con marca [SEED94].
   Catalogos (tipoMed_Id, viaAdmin_Id, tipoPar_Id) van en NULL porque no se
   conocen sus IDs.
   ===================================================================== */
SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON;
SET XACT_ABORT ON;
BEGIN TRAN;

/* ---------- 1. RECETAS (8) ---------- */
IF NOT EXISTS (SELECT 1 FROM Medico.tbRecetas WHERE receta_Instrucciones LIKE '[[]SEED94]%')
BEGIN
  INSERT INTO Medico.tbRecetas
   (cita_Id,masc_Id,receta_Medicamento,receta_Dosis,receta_Frecuencia,receta_Duracion,
    receta_Instrucciones,receta_FechaInicio,receta_FechaFin,receta_Estado,
    receta_EsEliminado,receta_UsuarioCrea,receta_FechaCrea,itm_Id,receta_Cantidad)
  VALUES
   (1,22,'Antibiotico Amoxicilina','10 mg/kg','Cada 12 horas','7 dias','[SEED94] Dar con alimento',DATEADD(DAY,-40,GETDATE()),DATEADD(DAY,-33,GETDATE()),'Completada',0,1,DATEADD(DAY,-40,GETDATE()),7,1),
   (2,23,'Antiparasitario Interno','1 ml por 5 kg','Dosis unica','1 dia','[SEED94] Repetir en 15 dias',DATEADD(DAY,-35,GETDATE()),DATEADD(DAY,-35,GETDATE()),'Completada',0,1,DATEADD(DAY,-35,GETDATE()),6,1),
   (3,26,'Antibiotico Amoxicilina','10 mg/kg','Cada 12 horas','10 dias','[SEED94] Completar tratamiento',DATEADD(DAY,-30,GETDATE()),DATEADD(DAY,-20,GETDATE()),'Completada',0,1,DATEADD(DAY,-30,GETDATE()),7,1),
   (4,27,'Antipulgas Topico','1 pipeta','Mensual','3 meses','[SEED94] Aplicar en el lomo',DATEADD(DAY,-25,GETDATE()),DATEADD(DAY,65,GETDATE()),'Activa',0,1,DATEADD(DAY,-25,GETDATE()),8,1),
   (5,21,'Antiparasitario Interno','1 ml por 5 kg','Dosis unica','1 dia','[SEED94] Ayuno de 2 horas',DATEADD(DAY,-20,GETDATE()),DATEADD(DAY,-20,GETDATE()),'Completada',0,1,DATEADD(DAY,-20,GETDATE()),6,1),
   (6,21,'Antibiotico Amoxicilina','10 mg/kg','Cada 12 horas','5 dias','[SEED94] Vigilar apetito',DATEADD(DAY,-15,GETDATE()),DATEADD(DAY,-10,GETDATE()),'Completada',0,1,DATEADD(DAY,-15,GETDATE()),7,1),
   (7,24,'Antipulgas Topico','1 pipeta','Mensual','3 meses','[SEED94] Evitar banar 48 horas',DATEADD(DAY,-8,GETDATE()),DATEADD(DAY,82,GETDATE()),'Activa',0,1,DATEADD(DAY,-8,GETDATE()),8,1),
   (8,25,'Antibiotico Amoxicilina','10 mg/kg','Cada 12 horas','7 dias','[SEED94] Control en una semana',DATEADD(DAY,-4,GETDATE()),DATEADD(DAY,3,GETDATE()),'Activa',0,1,DATEADD(DAY,-4,GETDATE()),7,1);
END

/* ---------- 2. TRATAMIENTOS (8) - enlazados a recetas SEED94 por cita ---------- */
IF NOT EXISTS (SELECT 1 FROM Medico.tbTratamientos WHERE trat_Observaciones LIKE '[[]SEED94]%')
BEGIN
  INSERT INTO Medico.tbTratamientos
   (masc_Id,cita_Id,receta_Id,trat_ParasitoDetectado,trat_Medicamento,trat_FechaAplicacion,
    trat_AplicadoPor,trat_ProximaDosis,trat_Estado,trat_Observaciones,
    trat_EsEliminado,trat_UsuarioCrea,trat_FechaCrea)
  SELECT v.masc_Id, v.cita_Id,
         (SELECT TOP 1 r.receta_Id FROM Medico.tbRecetas r
           WHERE r.cita_Id=v.cita_Id AND r.receta_Instrucciones LIKE '[[]SEED94]%'),
         v.parasito, v.med, DATEADD(DAY,v.dias,GETDATE()), v.aplicado,
         CASE WHEN v.prox IS NULL THEN NULL ELSE CAST(DATEADD(DAY,v.prox,GETDATE()) AS date) END,
         v.estado, '[SEED94] '+v.obs, 0, 1, DATEADD(DAY,v.dias,GETDATE())
  FROM (VALUES
   (22,1,'Ninguno','Amoxicilina',-40,'veterinario',NULL,'Aplicado','Tratamiento completado sin novedad'),
   (23,2,'Toxocara','Antiparasitario Interno',-35,'veterinario',-20,'Aplicado','Primera dosis'),
   (26,3,'Ninguno','Amoxicilina',-30,'roberto.esp',NULL,'Aplicado','Respuesta favorable'),
   (27,4,'Pulgas','Antipulgas Topico',-25,'sandra.esp',5,'Aplicado','Aplicacion mensual'),
   (21,5,'Giardia','Antiparasitario Interno',-20,'veterinario',-5,'Aplicado','Segunda dosis programada'),
   (21,6,'Ninguno','Amoxicilina',-15,'lucia.lomas',NULL,'Aplicado','Sin efectos adversos'),
   (24,7,'Pulgas','Antipulgas Topico',-8,'diego.esp',22,'Aplicado','Aplicacion mensual'),
   (25,8,'Ninguno','Amoxicilina',-4,'veterinario',NULL,'Programado','Seguimiento en curso')
  ) v(masc_Id,cita_Id,parasito,med,dias,aplicado,prox,estado,obs);
END

/* ---------- 3. ALERGIAS (8) ---------- */
IF NOT EXISTS (SELECT 1 FROM Medico.tbMascotaAlergia WHERE alrg_Reaccion LIKE '[[]SEED94]%')
  INSERT INTO Medico.tbMascotaAlergia
   (masc_Id,alrg_Sustancia,alrg_Reaccion,alrg_Gravedad,alrg_EsEliminado,alrg_UsuarioCrea,alrg_FechaCrea)
  VALUES
   (21,'Penicilina','[SEED94] Urticaria y picazon','Media',0,1,DATEADD(DAY,-60,GETDATE())),
   (22,'Polen','[SEED94] Estornudos y ojos llorosos','Leve',0,1,DATEADD(DAY,-55,GETDATE())),
   (23,'Pollo','[SEED94] Diarrea y vomito','Media',0,1,DATEADD(DAY,-50,GETDATE())),
   (24,'Picadura de pulga','[SEED94] Dermatitis intensa','Alta',0,1,DATEADD(DAY,-45,GETDATE())),
   (25,'Granos de cereal','[SEED94] Picazon cronica','Leve',0,1,DATEADD(DAY,-40,GETDATE())),
   (26,'Sulfas','[SEED94] Inflamacion facial','Alta',0,1,DATEADD(DAY,-35,GETDATE())),
   (27,'Polvo','[SEED94] Tos ocasional','Leve',0,1,DATEADD(DAY,-30,GETDATE())),
   (28,'Vacuna Quintuple','[SEED94] Letargo post vacuna','Media',0,1,DATEADD(DAY,-25,GETDATE()));

/* ---------- 4. PADECIMIENTOS (8) ---------- */
IF NOT EXISTS (SELECT 1 FROM Medico.tbMascotaPadecimiento WHERE pade_Nombre LIKE '%[[]SEED94]')
  INSERT INTO Medico.tbMascotaPadecimiento
   (masc_Id,pade_Nombre,pade_Estado,pade_Desde,pade_Hasta,pade_EsEliminado,pade_UsuarioCrea,pade_FechaCrea)
  VALUES
   (21,'Dermatitis atopica [SEED94]','Vigente',DATEADD(DAY,-200,GETDATE()),NULL,0,1,DATEADD(DAY,-60,GETDATE())),
   (22,'Otitis recurrente [SEED94]','Cronico',DATEADD(DAY,-300,GETDATE()),NULL,0,1,DATEADD(DAY,-55,GETDATE())),
   (23,'Gastritis [SEED94]','Superado',DATEADD(DAY,-120,GETDATE()),DATEADD(DAY,-90,GETDATE()),0,1,DATEADD(DAY,-50,GETDATE())),
   (24,'Sobrepeso [SEED94]','Vigente',DATEADD(DAY,-150,GETDATE()),NULL,0,1,DATEADD(DAY,-45,GETDATE())),
   (25,'Displasia de cadera leve [SEED94]','Cronico',DATEADD(DAY,-400,GETDATE()),NULL,0,1,DATEADD(DAY,-40,GETDATE())),
   (26,'Infeccion urinaria [SEED94]','Superado',DATEADD(DAY,-80,GETDATE()),DATEADD(DAY,-60,GETDATE()),0,1,DATEADD(DAY,-35,GETDATE())),
   (27,'Problema dental [SEED94]','Vigente',DATEADD(DAY,-100,GETDATE()),NULL,0,1,DATEADD(DAY,-30,GETDATE())),
   (28,'Desnutricion leve [SEED94]','Cronico',DATEADD(DAY,-70,GETDATE()),NULL,0,1,DATEADD(DAY,-25,GETDATE()));

/* ---------- 5. CONSENTIMIENTOS (8) ---------- */
IF NOT EXISTS (SELECT 1 FROM Medico.tbConsentimiento WHERE cons_Notas LIKE '[[]SEED94]%')
  INSERT INTO Medico.tbConsentimiento
   (masc_Id,cli_Id,cita_Id,cons_Tipo,cons_Estado,cons_Texto,cons_FirmanteNombre,cons_FechaFirma,cons_Notas,
    cons_EsEliminado,cons_UsuarioCrea,cons_FechaCrea)
  VALUES
   (22,3,1,'General','Firmado','Autorizo la realizacion del procedimiento indicado por el veterinario.','Juan Perez',DATEADD(DAY,-40,GETDATE()),'[SEED94] Sin observaciones',0,1,DATEADD(DAY,-40,GETDATE())),
   (23,4,2,'Anestesia','Firmado','Autorizo la aplicacion de anestesia general con sus riesgos informados.','Ana Martinez',DATEADD(DAY,-35,GETDATE()),'[SEED94] Ayuno confirmado',0,1,DATEADD(DAY,-35,GETDATE())),
   (26,7,3,'Cirugia','Firmado','Autorizo la cirugia propuesta y declaro haber recibido la informacion de riesgos.','Luis Hernandez',DATEADD(DAY,-30,GETDATE()),'[SEED94] Cirugia menor',0,1,DATEADD(DAY,-30,GETDATE())),
   (27,2,4,'General','Firmado','Autorizo la aplicacion del tratamiento indicado.','Maria Lopez',DATEADD(DAY,-25,GETDATE()),'[SEED94] Sin observaciones',0,1,DATEADD(DAY,-25,GETDATE())),
   (21,2,5,'Anestesia','Firmado','Autorizo la aplicacion de anestesia con sus riesgos informados.','Maria Lopez',DATEADD(DAY,-20,GETDATE()),'[SEED94] Mascota en ayuno',0,1,DATEADD(DAY,-20,GETDATE())),
   (24,5,7,'General','Firmado','Autorizo la realizacion del procedimiento indicado.','Carlos Rodriguez',DATEADD(DAY,-8,GETDATE()),'[SEED94] Sin observaciones',0,1,DATEADD(DAY,-8,GETDATE())),
   (25,6,8,'Cirugia','Pendiente','Autorizo la cirugia propuesta y declaro haber recibido la informacion de riesgos.',NULL,NULL,'[SEED94] Pendiente de firma del cliente',0,1,DATEADD(DAY,-4,GETDATE())),
   (28,NULL,9,'Eutanasia','Pendiente','Autorizo la eutanasia humanitaria bajo criterio medico.',NULL,NULL,'[SEED94] Pendiente de aprobacion del refugio',0,1,DATEADD(DAY,-2,GETDATE()));

/* ---------- 6. SUSTANCIAS CONTROLADAS (6) ----------
   OJO: no hay un item controlado en la lista; se usa itm_Id 7 como placeholder.
   Cambiar por el item real si existe. */
IF NOT EXISTS (SELECT 1 FROM Medico.tbSustanciaMovimiento WHERE scm_Notas LIKE '[[]SEED94]%')
  INSERT INTO Medico.tbSustanciaMovimiento
   (itm_Id,scm_Tipo,scm_Cantidad,masc_Id,scm_TestigoId,scm_Lote,scm_DeaSchedule,scm_Notas,scm_Fecha,
    scm_EsEliminado,scm_UsuarioCrea,scm_FechaCrea)
  VALUES
   (7,'Recibido',50.00,NULL,NULL,'LT-2026-001','CII','[SEED94] Ingreso de lote',DATEADD(DAY,-60,GETDATE()),0,1,DATEADD(DAY,-60,GETDATE())),
   (7,'Administrado',2.50,23,5,'LT-2026-001','CII','[SEED94] Sedacion para procedimiento',DATEADD(DAY,-35,GETDATE()),0,1,DATEADD(DAY,-35,GETDATE())),
   (7,'Administrado',3.00,26,5,'LT-2026-001','CII','[SEED94] Anestesia cirugia menor',DATEADD(DAY,-30,GETDATE()),0,1,DATEADD(DAY,-30,GETDATE())),
   (7,'Administrado',2.00,21,8,'LT-2026-001','CII','[SEED94] Sedacion previa a curacion',DATEADD(DAY,-20,GETDATE()),0,1,DATEADD(DAY,-20,GETDATE())),
   (7,'Desechado',1.50,NULL,5,'LT-2026-001','CII','[SEED94] Sobrante descartado con testigo',DATEADD(DAY,-19,GETDATE()),0,1,DATEADD(DAY,-19,GETDATE())),
   (7,'Recibido',30.00,NULL,NULL,'LT-2026-002','CII','[SEED94] Reposicion de stock',DATEADD(DAY,-10,GETDATE()),0,1,DATEADD(DAY,-10,GETDATE()));

/* ---------- 7. LABORATORIO (+6) con detalle ---------- */
IF NOT EXISTS (SELECT 1 FROM Medico.tbLaboratorio WHERE lab_Observaciones LIKE '[[]SEED94]%')
BEGIN
  DECLARE @lab INT;

  -- Hemograma mascota 22
  INSERT INTO Medico.tbLaboratorio (masc_Id,cita_Id,lab_Examen,lab_Tipo,lab_Estado,lab_FechaSolicitud,lab_FechaResultado,lab_Veterinario,lab_Observaciones,lab_EsEliminado,lab_UsuarioCrea,lab_FechaCrea)
  VALUES (22,1,'Hemograma completo','Sangre','Completado',DATEADD(DAY,-40,GETDATE()),DATEADD(DAY,-39,GETDATE()),5,'[SEED94] Valores normales',0,1,DATEADD(DAY,-40,GETDATE()));
  SET @lab = SCOPE_IDENTITY();
  INSERT INTO Medico.tbLaboratorioDetalle (lab_Id,labd_Parametro,labd_Valor,labd_Unidad,labd_RangoMin,labd_RangoMax,labd_FueraRango,labd_EsEliminado,labd_UsuarioCrea,labd_FechaCrea)
  VALUES (@lab,'Hematocrito','42','%',37,55,0,0,1,GETDATE()),
         (@lab,'Leucocitos','11.5','x10^3/uL',6,17,0,0,1,GETDATE()),
         (@lab,'Plaquetas','280','x10^3/uL',200,500,0,0,1,GETDATE());

  -- Quimica sanguinea mascota 23 (con valor fuera de rango)
  INSERT INTO Medico.tbLaboratorio (masc_Id,cita_Id,lab_Examen,lab_Tipo,lab_Estado,lab_FechaSolicitud,lab_FechaResultado,lab_Veterinario,lab_Observaciones,lab_EsEliminado,lab_UsuarioCrea,lab_FechaCrea)
  VALUES (23,2,'Quimica sanguinea','Sangre','Completado',DATEADD(DAY,-35,GETDATE()),DATEADD(DAY,-34,GETDATE()),5,'[SEED94] ALT ligeramente elevada',0,1,DATEADD(DAY,-35,GETDATE()));
  SET @lab = SCOPE_IDENTITY();
  INSERT INTO Medico.tbLaboratorioDetalle (lab_Id,labd_Parametro,labd_Valor,labd_Unidad,labd_RangoMin,labd_RangoMax,labd_FueraRango,labd_EsEliminado,labd_UsuarioCrea,labd_FechaCrea)
  VALUES (@lab,'Glucosa','95','mg/dL',70,120,0,0,1,GETDATE()),
         (@lab,'ALT','135','U/L',10,100,1,0,1,GETDATE()),
         (@lab,'Creatinina','1.1','mg/dL',0.5,1.5,0,0,1,GETDATE());

  -- Coprologico mascota 26
  INSERT INTO Medico.tbLaboratorio (masc_Id,cita_Id,lab_Examen,lab_Tipo,lab_Estado,lab_FechaSolicitud,lab_FechaResultado,lab_Veterinario,lab_Observaciones,lab_EsEliminado,lab_UsuarioCrea,lab_FechaCrea)
  VALUES (26,3,'Coprologico','Heces','Completado',DATEADD(DAY,-30,GETDATE()),DATEADD(DAY,-29,GETDATE()),5,'[SEED94] Sin parasitos',0,1,DATEADD(DAY,-30,GETDATE()));
  SET @lab = SCOPE_IDENTITY();
  INSERT INTO Medico.tbLaboratorioDetalle (lab_Id,labd_Parametro,labd_Valor,labd_Unidad,labd_RangoMin,labd_RangoMax,labd_FueraRango,labd_EsEliminado,labd_UsuarioCrea,labd_FechaCrea)
  VALUES (@lab,'Parasitos','Negativo',NULL,NULL,NULL,0,0,1,GETDATE()),
         (@lab,'Sangre oculta','Negativo',NULL,NULL,NULL,0,0,1,GETDATE());

  -- Coprologico mascota 21 (positivo)
  INSERT INTO Medico.tbLaboratorio (masc_Id,cita_Id,lab_Examen,lab_Tipo,lab_Estado,lab_FechaSolicitud,lab_FechaResultado,lab_Veterinario,lab_Observaciones,lab_EsEliminado,lab_UsuarioCrea,lab_FechaCrea)
  VALUES (21,5,'Coprologico','Heces','Completado',DATEADD(DAY,-20,GETDATE()),DATEADD(DAY,-19,GETDATE()),5,'[SEED94] Giardia positivo',0,1,DATEADD(DAY,-20,GETDATE()));
  SET @lab = SCOPE_IDENTITY();
  INSERT INTO Medico.tbLaboratorioDetalle (lab_Id,labd_Parametro,labd_Valor,labd_Unidad,labd_RangoMin,labd_RangoMax,labd_FueraRango,labd_EsEliminado,labd_UsuarioCrea,labd_FechaCrea)
  VALUES (@lab,'Giardia','Positivo',NULL,NULL,NULL,1,0,1,GETDATE()),
         (@lab,'Consistencia','Blanda',NULL,NULL,NULL,0,0,1,GETDATE());

  -- Hemograma mascota 24 (en proceso, sin resultado)
  INSERT INTO Medico.tbLaboratorio (masc_Id,cita_Id,lab_Examen,lab_Tipo,lab_Estado,lab_FechaSolicitud,lab_FechaResultado,lab_Veterinario,lab_Observaciones,lab_EsEliminado,lab_UsuarioCrea,lab_FechaCrea)
  VALUES (24,7,'Hemograma completo','Sangre','En proceso',DATEADD(DAY,-8,GETDATE()),NULL,5,'[SEED94] Muestra enviada',0,1,DATEADD(DAY,-8,GETDATE()));

  -- Quimica sanguinea mascota 25 (pendiente)
  INSERT INTO Medico.tbLaboratorio (masc_Id,cita_Id,lab_Examen,lab_Tipo,lab_Estado,lab_FechaSolicitud,lab_FechaResultado,lab_Veterinario,lab_Observaciones,lab_EsEliminado,lab_UsuarioCrea,lab_FechaCrea)
  VALUES (25,8,'Quimica sanguinea','Sangre','Pendiente',DATEADD(DAY,-4,GETDATE()),NULL,5,'[SEED94] Pendiente de toma de muestra',0,1,DATEADD(DAY,-4,GETDATE()));
END

COMMIT;
PRINT 'Seed MEDICO (94a) completado.';
