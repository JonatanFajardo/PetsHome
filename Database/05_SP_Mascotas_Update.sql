/****************************************************************************************
    ACTUALIZACIÓN: Refugio.tbMascotas - Agregar campo masc_IngresoId
    BASE DE DATOS: [PETSHOMEDB]
    DESCRIPCIÓN: Actualiza los SPs existentes para incluir el campo masc_IngresoId
****************************************************************************************/
USE [PETSHOMEDB];
GO

----------------------------------------------------------------------------------------
-- 1. Actualizar PR_Refugio_Mascotas_List
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Refugio].[PR_Refugio_Mascotas_List]') AND type in (N'P'))
BEGIN
    -- Verificar si el SP necesita actualización
    DECLARE @spDefinition NVARCHAR(MAX);
    SELECT @spDefinition = OBJECT_DEFINITION(OBJECT_ID('[Refugio].[PR_Refugio_Mascotas_List]'));

    IF CHARINDEX('masc_IngresoId', @spDefinition) = 0
    BEGIN
        PRINT '⚠ PR_Refugio_Mascotas_List necesita actualización manual';
        PRINT '  Agregar: masc_IngresoId en el SELECT';
    END
    ELSE
    BEGIN
        PRINT '✓ PR_Refugio_Mascotas_List ya incluye masc_IngresoId';
    END
END
ELSE
BEGIN
    PRINT '⚠ PR_Refugio_Mascotas_List no existe - debe crearse manualmente';
END
GO

----------------------------------------------------------------------------------------
-- 2. Actualizar PR_Refugio_Mascotas_Find
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Refugio].[PR_Refugio_Mascotas_Find]') AND type in (N'P'))
BEGIN
    DECLARE @spDefinition NVARCHAR(MAX);
    SELECT @spDefinition = OBJECT_DEFINITION(OBJECT_ID('[Refugio].[PR_Refugio_Mascotas_Find]'));

    IF CHARINDEX('masc_IngresoId', @spDefinition) = 0
    BEGIN
        PRINT '⚠ PR_Refugio_Mascotas_Find necesita actualización manual';
        PRINT '  Agregar: masc_IngresoId en el SELECT';
    END
    ELSE
    BEGIN
        PRINT '✓ PR_Refugio_Mascotas_Find ya incluye masc_IngresoId';
    END
END
ELSE
BEGIN
    PRINT '⚠ PR_Refugio_Mascotas_Find no existe - debe crearse manualmente';
END
GO

----------------------------------------------------------------------------------------
-- 3. Actualizar PR_Refugio_Mascotas_Detail
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Refugio].[PR_Refugio_Mascotas_Detail]') AND type in (N'P'))
BEGIN
    DECLARE @spDefinition NVARCHAR(MAX);
    SELECT @spDefinition = OBJECT_DEFINITION(OBJECT_ID('[Refugio].[PR_Refugio_Mascotas_Detail]'));

    IF CHARINDEX('masc_IngresoId', @spDefinition) = 0
    BEGIN
        PRINT '⚠ PR_Refugio_Mascotas_Detail necesita actualización manual';
        PRINT '  Agregar: masc_IngresoId y LEFT JOIN con tbIngresos';
    END
    ELSE
    BEGIN
        PRINT '✓ PR_Refugio_Mascotas_Detail ya incluye masc_IngresoId';
    END
END
ELSE
BEGIN
    PRINT '⚠ PR_Refugio_Mascotas_Detail no existe - debe crearse manualmente';
END
GO

----------------------------------------------------------------------------------------
-- 4. Actualizar PR_Refugio_Mascotas_Insert
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Refugio].[PR_Refugio_Mascotas_Insert]') AND type in (N'P'))
BEGIN
    DECLARE @spDefinition NVARCHAR(MAX);
    SELECT @spDefinition = OBJECT_DEFINITION(OBJECT_ID('[Refugio].[PR_Refugio_Mascotas_Insert]'));

    IF CHARINDEX('masc_IngresoId', @spDefinition) = 0
    BEGIN
        PRINT '⚠ PR_Refugio_Mascotas_Insert necesita actualización manual';
        PRINT '  Agregar: @masc_IngresoId INT (parámetro)';
        PRINT '  Agregar: masc_IngresoId en INSERT INTO y VALUES';
    END
    ELSE
    BEGIN
        PRINT '✓ PR_Refugio_Mascotas_Insert ya incluye masc_IngresoId';
    END
END
ELSE
BEGIN
    PRINT '⚠ PR_Refugio_Mascotas_Insert no existe - debe crearse manualmente';
END
GO

----------------------------------------------------------------------------------------
-- 5. Actualizar PR_Refugio_Mascotas_Update
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Refugio].[PR_Refugio_Mascotas_Update]') AND type in (N'P'))
BEGIN
    DECLARE @spDefinition NVARCHAR(MAX);
    SELECT @spDefinition = OBJECT_DEFINITION(OBJECT_ID('[Refugio].[PR_Refugio_Mascotas_Update]'));

    IF CHARINDEX('masc_IngresoId', @spDefinition) = 0
    BEGIN
        PRINT '⚠ PR_Refugio_Mascotas_Update necesita actualización manual';
        PRINT '  Agregar: @masc_IngresoId INT (parámetro)';
        PRINT '  Agregar: masc_IngresoId = @masc_IngresoId en UPDATE SET';
    END
    ELSE
    BEGIN
        PRINT '✓ PR_Refugio_Mascotas_Update ya incluye masc_IngresoId';
    END
END
ELSE
BEGIN
    PRINT '⚠ PR_Refugio_Mascotas_Update no existe - debe crearse manualmente';
END
GO

PRINT '';
PRINT '══════════════════════════════════════════════════════════════════════';
PRINT '  NOTA: Este script solo verifica los SPs existentes.';
PRINT '  Si aparecen advertencias, debes actualizar los SPs manualmente';
PRINT '  siguiendo el patrón documentado en CLAUDE.md';
PRINT '══════════════════════════════════════════════════════════════════════';
GO
