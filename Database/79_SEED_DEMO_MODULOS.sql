/*
=============================================
SCRIPT 79: Datos DEMO para las pantallas nuevas (v1.1–v1.3)
AUTOR: Sistema PetsHome
FECHA: 2026-10-28
DESCRIPCIÓN:
  Puebla con datos de ejemplo: banderas rojas (alergias/padecimientos), cobros
  pendientes, tablero del día, agenda, laboratorio, sustancias controladas,
  consentimientos y lista de espera. Idempotente (guardas NOT EXISTS).
  Usuario demo = 1 (admin). Veterinario demo = 5. Cliente demo = 1.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

DECLARE @hoy DATETIME = GETDATE();

-- Asignar una mascota (Luna=2) al cliente 1 para que lo "de cliente" tenga sentido
IF EXISTS (SELECT 1 FROM Refugio.tbMascotas WHERE masc_Id=2 AND cli_Id IS NULL)
    UPDATE Refugio.tbMascotas SET cli_Id=1 WHERE masc_Id=2;

-- ============================================================
-- 1) BANDERAS ROJAS: alergias + padecimientos
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Medico.tbMascotaAlergia)
BEGIN
    INSERT INTO Medico.tbMascotaAlergia (masc_Id, alrg_Sustancia, alrg_Reaccion, alrg_Gravedad, alrg_EsEliminado, alrg_UsuarioCrea, alrg_FechaCrea) VALUES
    (1, N'Penicilina', N'Urticaria y edema facial', N'Alta', 0, 1, @hoy),
    (1, N'Polen',      N'Estornudos y prurito',     N'Media',0, 1, @hoy),
    (3, N'Sulfas',     N'Reaccion cutanea',         N'Alta', 0, 1, @hoy),
    (2, N'Procaina',   N'Leve enrojecimiento',      N'Leve', 0, 1, @hoy);
    PRINT 'OK alergias';
END

IF NOT EXISTS (SELECT 1 FROM Medico.tbMascotaPadecimiento)
BEGIN
    INSERT INTO Medico.tbMascotaPadecimiento (masc_Id, pade_Nombre, pade_Estado, pade_Desde, pade_Hasta, pade_EsEliminado, pade_UsuarioCrea, pade_FechaCrea) VALUES
    (1, N'Insuficiencia renal cronica', N'Cronico', '2024-03-01', NULL, 0, 1, @hoy),
    (1, N'Otitis recurrente',           N'Vigente', '2026-08-15', NULL, 0, 1, @hoy),
    (2, N'Dermatitis alergica',         N'Vigente', '2026-09-10', NULL, 0, 1, @hoy),
    (3, N'Fractura tibia (recuperada)', N'Superado','2025-11-01', '2026-01-20', 0, 1, @hoy);
    PRINT 'OK padecimientos';
END

-- ============================================================
-- 2) COBROS PENDIENTES (cola dispensar->cobrar) — cliente 1
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Veterinaria.tbCargoDispensacion)
BEGIN
    -- El cobro es POR VISITA: cada cargo se ata a la cita real de la mascota 2 (Luna).
    DECLARE @citaDemo INT = (SELECT TOP 1 cita_Id FROM Medico.tbCitaMedica
                             WHERE masc_Id = 2 AND cita_EsEliminado = 0 ORDER BY cita_Id);
    INSERT INTO Veterinaria.tbCargoDispensacion
        (receta_Id, masc_Id, cli_Id, cita_Id, itm_Id, cargo_Cantidad, cargo_DescripcionSnap, cargo_PrecioUnitSnap, cargo_Estado, cargo_EsEliminado, cargo_UsuarioCrea, cargo_FechaCrea) VALUES
    (1, 2, 1, @citaDemo, 1, 2, N'Concentrado Premium Adulto 15kg', 850.00, N'Pendiente', 0, 1, @hoy),
    (2, 2, 1, @citaDemo, 2, 1, N'Concentrado Cachorro 8kg',        620.00, N'Pendiente', 0, 1, @hoy),
    (3, 2, 1, @citaDemo, 3, 3, N'Alimento Humedo Latas x12',       480.00, N'Pendiente', 0, 1, @hoy);
    PRINT 'OK cobros';
END

-- ============================================================
-- 3) TABLERO DEL DÍA + 4) AGENDA (citas de hoy + del mes, con veterinario)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Medico.tbCitaMedica WHERE cita_MotivoConsulta LIKE '%DEMO%')
BEGIN
    -- Citas de HOY para el tablero Kanban (3 estados) + visibles en agenda
    INSERT INTO Medico.tbCitaMedica (masc_Id, cita_FechaConsulta, tipoCon_Id, cita_MotivoConsulta, cita_Estado, cita_Prioridad, cita_HoraLlegada, cita_Veterinario, cita_Recepcionista, cita_EsEliminado, cita_UsuarioCrea, cita_FechaCrea) VALUES
    (1, @hoy, 2, N'Vomito y decaimiento [DEMO]',     N'En espera',   N'Urgente', DATEADD(MINUTE,-25,@hoy), 5, 1, 0, 1, @hoy),
    (3, @hoy, 1, N'Chequeo general [DEMO]',           N'En espera',   N'Normal',  DATEADD(MINUTE,-15,@hoy), 5, 1, 0, 1, @hoy),
    (2, @hoy, 3, N'Control dermatitis [DEMO]',        N'En atencion', N'Normal',  DATEADD(MINUTE,-55,@hoy), 5, 1, 0, 1, @hoy),
    (4, @hoy, 6, N'Vacunacion anual [DEMO]',          N'Finalizada',  N'Normal',  DATEADD(MINUTE,-120,@hoy),5, 1, 0, 1, @hoy);

    -- Citas del MES (agenda) con y sin veterinario asignado
    INSERT INTO Medico.tbCitaMedica (masc_Id, cita_FechaConsulta, tipoCon_Id, cita_MotivoConsulta, cita_Estado, cita_Prioridad, cita_Veterinario, cita_EsEliminado, cita_UsuarioCrea, cita_FechaCrea) VALUES
    (5, DATEADD(DAY, 2, @hoy),  1, N'Consulta general [DEMO]',   N'En espera', N'Normal', 5,    0, 1, @hoy),
    (6, DATEADD(DAY, 3, @hoy),  7, N'Chequeo de rutina [DEMO]',  N'En espera', N'Normal', 5,    0, 1, @hoy),
    (1, DATEADD(DAY, -3, @hoy), 4, N'Pre-quirurgico [DEMO]',     N'Finalizada',N'Alta',   5,    0, 1, @hoy),
    (3, DATEADD(DAY, 4, @hoy),  2, N'Emergencia [DEMO]',         N'En espera', N'Urgente',NULL, 0, 1, @hoy),
    (4, DATEADD(DAY, 1, @hoy),  6, N'Vacunacion [DEMO]',         N'En espera', N'Normal', NULL, 0, 1, @hoy);
    PRINT 'OK citas (tablero + agenda)';
END

-- ============================================================
-- 5) LABORATORIO (examenes + detalle con rangos)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Medico.tbLaboratorio)
BEGIN
    INSERT INTO Medico.tbLaboratorio (masc_Id, cita_Id, lab_Examen, lab_Tipo, lab_Estado, lab_FechaSolicitud, lab_FechaResultado, lab_Veterinario, lab_Observaciones, lab_EsEliminado, lab_UsuarioCrea, lab_FechaCrea) VALUES
    (1, NULL, N'Hemograma completo', N'Sangre', N'Completado', DATEADD(DAY,-2,@hoy), DATEADD(DAY,-1,@hoy), 5, N'Leucocitosis leve', 0, 1, @hoy),
    (3, NULL, N'Urianalisis',        N'Orina',  N'Pendiente',  @hoy,                 NULL,                  5, NULL,                  0, 1, @hoy),
    (2, NULL, N'Quimica sanguinea',  N'Sangre', N'Revisado',   DATEADD(DAY,-5,@hoy), DATEADD(DAY,-4,@hoy), 5, N'Valores renales elevados', 0, 1, @hoy);

    DECLARE @lab1 INT = (SELECT lab_Id FROM Medico.tbLaboratorio WHERE lab_Examen=N'Hemograma completo' AND masc_Id=1);
    DECLARE @lab3 INT = (SELECT lab_Id FROM Medico.tbLaboratorio WHERE lab_Examen=N'Quimica sanguinea' AND masc_Id=2);
    INSERT INTO Medico.tbLaboratorioDetalle (lab_Id, labd_Parametro, labd_Valor, labd_Unidad, labd_RangoMin, labd_RangoMax, labd_FueraRango, labd_EsEliminado, labd_UsuarioCrea, labd_FechaCrea) VALUES
    (@lab1, N'Hematocrito',  N'42',    N'%',      37, 55, 0, 0, 1, @hoy),
    (@lab1, N'Leucocitos',   N'18500', N'/uL', 6000,17000, 1, 0, 1, @hoy),
    (@lab1, N'Plaquetas',    N'280000',N'/uL',200000,500000,0,0, 1, @hoy),
    (@lab3, N'Creatinina',   N'3.1',   N'mg/dL', 0.5, 1.8, 1, 0, 1, @hoy),
    (@lab3, N'BUN',          N'45',    N'mg/dL',  7,  27, 1, 0, 1, @hoy);
    PRINT 'OK laboratorio';
END

-- ============================================================
-- 6) SUSTANCIAS CONTROLADAS (movimientos; saldo = 100-30-10+5 = 65)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Medico.tbSustanciaMovimiento)
BEGIN
    INSERT INTO Medico.tbSustanciaMovimiento (itm_Id, scm_Tipo, scm_Cantidad, masc_Id, scm_TestigoId, scm_Lote, scm_DeaSchedule, scm_Notas, scm_Fecha, scm_EsEliminado, scm_UsuarioCrea, scm_FechaCrea) VALUES
    (1, N'Recibido',     100, NULL, NULL, N'L-2026-001', N'III', N'Ingreso de inventario controlado', DATEADD(DAY,-10,@hoy), 0, 1, @hoy),
    (1, N'Administrado',  30, 1,    NULL, N'L-2026-001', N'III', N'Sedacion para procedimiento',        DATEADD(DAY,-3,@hoy),  0, 1, @hoy),
    (1, N'Desechado',     10, NULL, 1,    N'L-2026-001', N'III', N'Vial vencido, desecho con testigo',  DATEADD(DAY,-2,@hoy),  0, 1, @hoy),
    (1, N'Devuelto',       5, NULL, NULL, N'L-2026-001', N'III', N'Sobrante devuelto a farmacia',       DATEADD(DAY,-1,@hoy),  0, 1, @hoy),
    (2, N'Recibido',      50, NULL, NULL, N'L-2026-055', N'IV',  N'Ingreso de inventario controlado',   DATEADD(DAY,-7,@hoy),  0, 1, @hoy);
    PRINT 'OK sustancias controladas';
END

-- ============================================================
-- 7) CONSENTIMIENTOS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Medico.tbConsentimiento)
BEGIN
    INSERT INTO Medico.tbConsentimiento (masc_Id, cli_Id, cita_Id, cons_Tipo, cons_Estado, cons_Texto, cons_FirmanteNombre, cons_FechaFirma, cons_Notas, cons_EsEliminado, cons_UsuarioCrea, cons_FechaCrea) VALUES
    (2, 1, NULL, N'Cirugia',   N'Firmado',  N'Autorizo el procedimiento quirurgico descrito y declaro haber sido informado de los riesgos.', N'Maria Lopez', DATEADD(DAY,-1,@hoy), NULL, 0, 1, @hoy),
    (1, NULL, NULL, N'Anestesia', N'Pendiente', N'Autorizo la administracion de anestesia general para el procedimiento programado.', NULL, NULL, NULL, 0, 1, @hoy),
    (3, NULL, NULL, N'General',  N'Firmado',  N'Autorizo la atencion medica general de mi mascota en esta clinica.', N'Carlos Ruiz', DATEADD(DAY,-4,@hoy), NULL, 0, 1, @hoy),
    (2, 1, NULL, N'Eutanasia', N'Rechazado', N'Consentimiento de eutanasia humanitaria.', NULL, NULL, N'El dueno decidio continuar el tratamiento', 0, 1, @hoy);
    PRINT 'OK consentimientos';
END

-- ============================================================
-- 8) LISTA DE ESPERA (waitlist)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Medico.tbListaEspera)
BEGIN
    INSERT INTO Medico.tbListaEspera (masc_Id, cli_Id, tipoCon_Id, lesp_FechaDeseada, lesp_Prioridad, lesp_Estado, lesp_Notas, lesp_EsEliminado, lesp_UsuarioCrea, lesp_FechaCrea) VALUES
    (1, NULL, 2,    NULL,                     N'Urgente', N'Esperando', N'Avisar en cuanto se libere cupo', 0, 1, @hoy),
    (4, NULL, 1,    DATEADD(DAY,3,@hoy),      N'Normal',  N'Esperando', N'Prefiere por la manana',          0, 1, @hoy),
    (5, NULL, NULL, NULL,                     N'Alta',    N'Esperando', N'Cualquier tipo de consulta',      0, 1, @hoy),
    (2, 1,    7,    DATEADD(DAY,5,@hoy),       N'Normal', N'Esperando', N'Chequeo de rutina',               0, 1, @hoy);
    PRINT 'OK lista de espera';
END

PRINT '=========================================='
PRINT 'SEED DEMO completado.'
PRINT '=========================================='
GO
