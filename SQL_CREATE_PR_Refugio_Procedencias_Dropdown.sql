-- =============================================
-- Autor: Sistema PetsHome
-- Fecha de Creación: 2025
-- Descripción: Stored Procedure para obtener el dropdown de Procedencias
-- =============================================

USE [PETSHOMEDB]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Refugio].[PR_Refugio_Procedencias_Dropdown]') AND type in (N'P', N'PC'))
DROP PROCEDURE [Refugio].[PR_Refugio_Procedencias_Dropdown]
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Procedencias_Dropdown]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        proc_Id,
        proc_Descripcion
    FROM
        Refugio.tbProcedencias
    WHERE
        proc_EsEliminado = 0
    ORDER BY
        proc_Descripcion ASC;
END
GO
