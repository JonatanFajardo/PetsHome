/*
=============================================
SCRIPT 81: Ligar cargos demo a una VISITA real (feature/cobro-por-visita)
AUTOR: Sistema PetsHome
FECHA: 2026-10-28
DESCRIPCIÓN:
  El seed (script 79) insertó los cargos de dispensación con cita_Id NULL, por lo que
  /Cobros no podía agruparlos por visita. Este script asigna a cada cargo pendiente
  sin visita la cita real de SU mascota (la más reciente), para que /Cobros muestre
  grupos por visita. Idempotente: solo toca cargos con cita_Id NULL que tengan una
  cita disponible. NO reabre ni altera cargos ya facturados/condonados.
  Requiere scripts 65, 79, 80.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

UPDATE c
SET c.cita_Id = x.cita_Id,
    c.cargo_UsuarioModifica = 1,
    c.cargo_FechaModifica = GETDATE()
FROM [Veterinaria].[tbCargoDispensacion] c
CROSS APPLY (
    SELECT TOP 1 ci.cita_Id
    FROM [Medico].[tbCitaMedica] ci
    WHERE ci.masc_Id = c.masc_Id AND ci.cita_EsEliminado = 0
    ORDER BY ci.cita_FechaConsulta DESC, ci.cita_Id DESC
) x
WHERE c.cita_Id IS NULL
  AND c.cargo_EsEliminado = 0
  AND c.cargo_Estado = 'Pendiente';

PRINT '✅ Script 81: cargos pendientes sin visita ligados a la cita real de su mascota';
GO

-- Verificación rápida (no transaccional): cómo quedaron agrupados
SELECT c.cita_Id, c.masc_Id, m.masc_Nombre, COUNT(*) AS Cargos,
       SUM(c.cargo_Cantidad * c.cargo_PrecioUnitSnap) AS Subtotal
FROM [Veterinaria].[tbCargoDispensacion] c
LEFT JOIN [Refugio].[tbMascotas] m ON c.masc_Id = m.masc_Id
WHERE c.cargo_EsEliminado = 0 AND c.cargo_Estado = 'Pendiente'
GROUP BY c.cita_Id, c.masc_Id, m.masc_Nombre
ORDER BY c.cita_Id;
GO
