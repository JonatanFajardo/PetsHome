-- 83_FIX_FACTURAR_VISITA_SERVICIOS.sql
-- Corrige FacturarVisita: mapea el cargo a su linea de factura segun cargo_TipoOrigen
-- (Servicio -> serv_Id / Producto -> itm_Id). Antes estaba fijo en 'Producto' y los
-- servicios (itm_Id NULL) violaban el CHECK CK_Veterinaria_tbFacturaDetalle_Ref.
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
-- =============================================
-- PASO 3: Facturar/resolver UNA visita (cita) segÃºn disposiciÃ³n
-- =============================================
ALTER   PROCEDURE [Veterinaria].[PR_Veterinaria_CargoDispensacion_FacturarVisita]
    @cita_Id      INT,
    @disposicion  NVARCHAR(20),              -- 'Pagado' | 'PorCobrar' | 'SinCargo' (sin tildes)
    @vencimiento  DATE = NULL,               -- requerido si 'PorCobrar'
    @motivo       NVARCHAR(1000) = NULL,     -- requerido (>=5) si 'SinCargo'
    @usuario      INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        -- Validaciones de entrada ------------------------------------------------
        IF @cita_Id IS NULL OR @cita_Id <= 0
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -2 AS CodeStatus, 'Visita (cita) invÃ¡lida.' AS MessageStatus
            RETURN
        END
        IF @disposicion NOT IN ('Pagado','PorCobrar','SinCargo')
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -2 AS CodeStatus, 'DisposiciÃ³n invÃ¡lida (Pagado/PorCobrar/SinCargo).' AS MessageStatus
            RETURN
        END
        IF @disposicion = 'PorCobrar' AND @vencimiento IS NULL
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -3 AS CodeStatus, 'Por cobrar requiere fecha de vencimiento.' AS MessageStatus
            RETURN
        END
        IF @disposicion = 'SinCargo' AND (@motivo IS NULL OR LEN(@motivo) < 5)
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -4 AS CodeStatus, 'Sin cargo requiere un motivo de al menos 5 caracteres.' AS MessageStatus
            RETURN
        END

        -- Debe haber cargos pendientes en ESA visita -----------------------------
        IF NOT EXISTS (SELECT 1 FROM [Veterinaria].[tbCargoDispensacion]
                       WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0)
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -1 AS CodeStatus, 'La visita no tiene cargos pendientes.' AS MessageStatus
            RETURN
        END

        -- â”€â”€ SinCargo: condonar los cargos de la visita (sin factura) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        IF @disposicion = 'SinCargo'
        BEGIN
            UPDATE [Veterinaria].[tbCargoDispensacion]
            SET cargo_Estado = 'Condonado',
                cargo_ResueltoPor = @usuario, cargo_FechaResuelto = GETDATE(),
                cargo_MotivoResolucion = @motivo,
                cargo_UsuarioModifica = @usuario, cargo_FechaModifica = GETDATE()
            WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0

            SELECT CAST(NULL AS INT) AS fact_Id, 1 AS CodeStatus, 'Cargos de la visita condonados (sin cargo).' AS MessageStatus
            RETURN
        END

        -- â”€â”€ Pagado / PorCobrar: crear UNA factura con los cargos de la visita â”€â”€â”€
        DECLARE @cli_Id INT, @masc_Id INT
        SELECT TOP 1 @cli_Id = cli_Id, @masc_Id = masc_Id
        FROM [Veterinaria].[tbCargoDispensacion]
        WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0

        DECLARE @subtotal DECIMAL(18,2) =
            (SELECT SUM(cargo_Cantidad * cargo_PrecioUnitSnap)
             FROM [Veterinaria].[tbCargoDispensacion]
             WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0);

        -- ISV 15% (Honduras). Misma convenciÃ³n que scripts 43 y 66.
        DECLARE @impuesto DECIMAL(18,2) = ROUND(@subtotal * 15.00 / 100.0, 2);
        DECLARE @total    DECIMAL(18,2) = @subtotal + @impuesto;
        DECLARE @estado   VARCHAR(20)   = CASE WHEN @disposicion = 'Pagado' THEN 'Pagada' ELSE 'PorCobrar' END;
        DECLARE @venc     DATE          = CASE WHEN @disposicion = 'PorCobrar' THEN @vencimiento ELSE NULL END;

        BEGIN TRAN

        -- 1. Encabezado (factura de la visita, con su mascota de origen)
        INSERT INTO [Veterinaria].[tbFacturas]
            (fact_Numero, cli_Id, masc_Id, refg_Id, fact_Fecha,
             fact_Subtotal, fact_Impuesto, fact_Total, fact_Estado, fact_Vencimiento,
             fact_EsEliminado, fact_UsuarioCrea, fact_FechaCrea)
        VALUES
            (NULL, @cli_Id, @masc_Id, NULL, GETDATE(),
             @subtotal, @impuesto, @total, @estado, @venc,
             0, @usuario, GETDATE())

        DECLARE @fact_Id INT = CAST(SCOPE_IDENTITY() AS INT)

        -- 2. Una lÃ­nea por cargo pendiente de la visita + marcar Facturado
        DECLARE @cargo_Id INT, @itm_Id INT, @cant INT, @desc NVARCHAR(500), @precio DECIMAL(18,2), @fdet_Id INT
        DECLARE @tipoOrigen NVARCHAR(20), @origenId INT

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT cargo_Id, itm_Id, cargo_Cantidad, cargo_DescripcionSnap, cargo_PrecioUnitSnap,
                   cargo_TipoOrigen, cargo_OrigenId
            FROM [Veterinaria].[tbCargoDispensacion]
            WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0

        OPEN cur
        FETCH NEXT FROM cur INTO @cargo_Id, @itm_Id, @cant, @desc, @precio, @tipoOrigen, @origenId
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Mapea el cargo a su linea de factura: Servicio (vacuna/lab/procedimiento)
            -- usa serv_Id; Producto (medicamento) usa itm_Id. La regla CK exige exactamente uno.
            IF @tipoOrigen = 'Servicio'
                INSERT INTO [Veterinaria].[tbFacturaDetalle]
                    (fact_Id, fdet_Tipo, serv_Id, itm_Id, fdet_Descripcion,
                     fdet_Cantidad, fdet_PrecioUnit, fdet_Subtotal)
                VALUES
                    (@fact_Id, 'Servicio', @origenId, NULL, @desc,
                     @cant, @precio, @cant * @precio)
            ELSE
                INSERT INTO [Veterinaria].[tbFacturaDetalle]
                    (fact_Id, fdet_Tipo, serv_Id, itm_Id, fdet_Descripcion,
                     fdet_Cantidad, fdet_PrecioUnit, fdet_Subtotal)
                VALUES
                    (@fact_Id, 'Producto', NULL, @itm_Id, @desc,
                     @cant, @precio, @cant * @precio)

            SET @fdet_Id = CAST(SCOPE_IDENTITY() AS INT)

            UPDATE [Veterinaria].[tbCargoDispensacion]
            SET cargo_Estado = 'Facturado',
                fact_Id = @fact_Id, fdet_Id = @fdet_Id,
                cargo_ResueltoPor = @usuario, cargo_FechaResuelto = GETDATE(),
                cargo_MotivoResolucion = NULL,
                cargo_UsuarioModifica = @usuario, cargo_FechaModifica = GETDATE()
            WHERE cargo_Id = @cargo_Id

            FETCH NEXT FROM cur INTO @cargo_Id, @itm_Id, @cant, @desc, @precio, @tipoOrigen, @origenId
        END
        CLOSE cur
        DEALLOCATE cur

        COMMIT

        SELECT @fact_Id AS fact_Id, 1 AS CodeStatus, 'Factura de la visita creada.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK
        SELECT CAST(NULL AS INT) AS fact_Id, -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END


