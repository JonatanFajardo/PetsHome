-- =============================================
-- Stored Procedures para Recepciones de Mercancía
-- =============================================
-- NOTA: Los procedimientos PR_Inventario_RecepcionesDetalles_ByRecepcion,
-- PR_Inventario_RecepcionesMercancia_Delete, PR_Inventario_RecepcionesMercancia_Find,
-- PR_Inventario_RecepcionesDetalles_Insert, PR_Inventario_RecepcionesDetalles_Update,
-- y PR_Inventario_RecepcionesMercancia_Update ya existen en la base de datos.
--
-- A continuación se muestran los procedimientos faltantes que necesitan ser creados:
-- =============================================

-- =============================================
-- Procedimiento: Listado de Recepciones de Mercancía
-- =============================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesMercancia_List]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        rm.recep_Id,
        rm.recep_Descripcion,
        rm.recep_Fecha,
        rm.refg_Id,
        r.refg_Nombre,
        rm.recep_TipoRecepcion,
        rm.recep_NumeroDocumento
    FROM
        [Inventario].[tbRecepcionesMercancia] rm
        INNER JOIN [Refugio].[tbRefugios] r ON rm.refg_Id = r.refg_Id
    WHERE
        rm.recep_EsEliminado = 0
    ORDER BY
        rm.recep_Fecha DESC, rm.recep_Id DESC;
END
GO

-- =============================================
-- Procedimiento: Detalle de Recepción de Mercancía
-- =============================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesMercancia_Detail]
    @recep_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        rm.recep_Id,
        rm.recep_Descripcion,
        rm.recep_Fecha,
        rm.refg_Id,
        r.refg_Nombre,
        rm.recep_TipoRecepcion,
        rm.recep_OrigenId,
        p.proc_Descripcion,
        rm.recep_NumeroDocumento
    FROM
        [Inventario].[tbRecepcionesMercancia] rm
        INNER JOIN [Refugio].[tbRefugios] r ON rm.refg_Id = r.refg_Id
        LEFT JOIN [General].[tbProcedencias] p ON rm.recep_OrigenId = p.proc_Id
    WHERE
        rm.recep_Id = @recep_Id
        AND rm.recep_EsEliminado = 0;
END
GO

-- =============================================
-- Procedimiento: Insertar Recepción de Mercancía
-- =============================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesMercancia_Insert]
    @recep_Descripcion VARCHAR(500),
    @recep_Fecha DATETIME,
    @refg_Id INT,
    @recep_TipoRecepcion VARCHAR(50),
    @recep_OrigenId INT = NULL,
    @recep_NumeroDocumento VARCHAR(50) = NULL,
    @recep_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [Inventario].[tbRecepcionesMercancia]
        (
            recep_Descripcion,
            recep_Fecha,
            refg_Id,
            recep_EsEliminado,
            recep_UsuarioCrea,
            recep_FechaCrea,
            recep_TipoRecepcion,
            recep_OrigenId,
            recep_NumeroDocumento
        )
        VALUES
        (
            @recep_Descripcion,
            @recep_Fecha,
            @refg_Id,
            0, -- recep_EsEliminado
            @recep_UsuarioCrea,
            GETDATE(),
            @recep_TipoRecepcion,
            @recep_OrigenId,
            @recep_NumeroDocumento
        );

        COMMIT TRANSACTION;
        SELECT 1 AS Result; -- Éxito
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 0 AS Result; -- Error
    END CATCH
END
GO

-- =============================================
-- Procedimiento: Listado de Detalles por Recepción
-- =============================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesDetalles_List]
    @recep_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        rd.recdet_Id,
        rd.recep_Id,
        rd.itm_Id,
        i.itm_Descripcion,
        rd.recdet_Cantidad,
        rd.recdet_PrecioUnitario,
        rd.recdet_FechaVencimiento,
        rd.recdet_NumeroLote
    FROM
        [Inventario].[tbRecepcionesDetalles] rd
        INNER JOIN [Inventario].[tbItems] i ON rd.itm_Id = i.itm_Id
    WHERE
        rd.recep_Id = @recep_Id
        AND rd.recdet_EsEliminado = 0
    ORDER BY
        rd.recdet_Id;
END
GO

-- =============================================
-- Procedimiento: Buscar Detalle de Recepción por ID
-- =============================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesDetalles_Find]
    @recdet_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        recdet_Id,
        recep_Id,
        itm_Id,
        recdet_Cantidad,
        recdet_PrecioUnitario,
        recdet_FechaVencimiento,
        recdet_NumeroLote
    FROM
        [Inventario].[tbRecepcionesDetalles]
    WHERE
        recdet_Id = @recdet_Id
        AND recdet_EsEliminado = 0;
END
GO

-- =============================================
-- Procedimiento: Eliminar Detalle de Recepción
-- =============================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesDetalles_Delete]
    @recdet_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE [Inventario].[tbRecepcionesDetalles]
        SET
            recdet_EsEliminado = 1,
            recdet_UsuarioModifica = 1, -- Cambiar según usuario en sesión
            recdet_FechaModifica = GETDATE()
        WHERE
            recdet_Id = @recdet_Id;

        COMMIT TRANSACTION;
        SELECT 1 AS Result; -- Éxito
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SELECT 0 AS Result; -- Error
    END CATCH
END
GO

-- =============================================
-- Notas de implementación:
-- =============================================
-- 1. Los procedimientos existentes deben seguir la misma estructura
-- 2. Verificar que las tablas tbRefugios y tbProcedencias existan
-- 3. Ajustar el campo recep_UsuarioModifica según el sistema de autenticación
-- 4. Los procedimientos de Insert y Update deben manejar transacciones
-- 5. El procedimiento Delete implementa eliminación lógica (recep_EsEliminado = 1)
