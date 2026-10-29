-- 85_FIX_UNIQUE_RECETA_NULL.sql
-- Corrige UQ_CargoDispensacion_Receta: el indice unico (filtrado por EsEliminado=0)
-- bloqueaba tener mas de UN cargo con receta_Id = NULL. Como los SERVICIOS
-- (vacuna/lab/procedimiento) se encolan con receta_Id = NULL, solo se podia
-- registrar un servicio en todo el sistema. Se recrea excluyendo los NULL de la
-- unicidad: receta_Id sigue siendo unico para medicamentos, y los servicios
-- (receta_Id NULL) ya no chocan entre si.
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_CargoDispensacion_Receta'
           AND object_id = OBJECT_ID('Veterinaria.tbCargoDispensacion'))
    DROP INDEX UQ_CargoDispensacion_Receta ON Veterinaria.tbCargoDispensacion;
GO
CREATE UNIQUE INDEX UQ_CargoDispensacion_Receta
    ON Veterinaria.tbCargoDispensacion (receta_Id)
    WHERE cargo_EsEliminado = 0 AND receta_Id IS NOT NULL;
GO
