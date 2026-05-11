-- Script para actualizar el procedimiento almacenado PR_Refugio_Adopciones_List
-- Este script agrega las columnas masc_Nombre, per_Nombre y adop_FechaCrea al resultado

USE PETSHOMEDB
GO

-- Eliminar el procedimiento si existe
IF OBJECT_ID('[Refugio].[PR_Refugio_Adopciones_List]', 'P') IS NOT NULL
    DROP PROCEDURE [Refugio].[PR_Refugio_Adopciones_List]
GO

-- Crear el procedimiento almacenado actualizado
CREATE PROCEDURE [Refugio].[PR_Refugio_Adopciones_List]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.adop_Id,
        a.sol_Id,
        a.adop_Estado,
        m.masc_Nombre,
        p.per_Nombre,
        a.adop_FechaCrea
    FROM
        Refugio.tbAdopciones a
    INNER JOIN
        Refugio.tbSolicitudes s ON a.sol_Id = s.sol_Id
    INNER JOIN
        Refugio.tbMascotas m ON s.masc_Id = m.masc_Id
    INNER JOIN
        General.tbPersonas p ON s.per_Id = p.per_Id
    WHERE
        a.adop_Estado = 1
    ORDER BY
        a.adop_FechaCrea DESC
END
GO

PRINT 'Procedimiento almacenado PR_Refugio_Adopciones_List actualizado correctamente'
GO
