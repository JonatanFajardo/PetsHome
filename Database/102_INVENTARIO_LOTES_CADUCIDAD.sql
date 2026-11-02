-- =====================================================================
-- 102_INVENTARIO_LOTES_CADUCIDAD.sql
-- Inventario por LOTES con CADUCIDAD + consumo FEFO + alertas de vencimiento.
--
-- DISENO ADITIVO (NO rompe el motor de stock actual):
--   - Inventario.tbExistencias.exi_Cantidad SIGUE siendo el total autoritativo.
--   - tbLotes es una capa al lado; se mantiene sincronizada: sum(lotes) = exi_Cantidad.
--   - Entrada (recepcion) ya sube exi_Cantidad via TR_RecepcionesDetalles_ActualizarExistencias;
--     se agrega un trigger APARTE que ademas crea el lote (no toca exi para no duplicar).
--   - Salida ya baja exi_Cantidad via TR_SalidasDetalles_ActualizarExistencias; se AMPLIA ese
--     trigger para que ademas reparta la salida entre lotes FEFO (vence primero sale primero).
--     Si un item no tiene lotes (datos viejos), NO falla: cae al comportamiento actual.
--
-- Idempotente: re-ejecutable. Tablas con IF NOT EXISTS, SPs/triggers con CREATE OR ALTER,
-- backfill sin duplicar (una fila de lote por detalle de recepcion).
-- Texto sin acentos por convencion.
-- =====================================================================
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- =====================================================================
-- 1. TABLA Inventario.tbLotes
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('Inventario.tbLotes') AND type = 'U')
BEGIN
    CREATE TABLE [Inventario].[tbLotes]
    (
        lot_Id                 INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tbLotes PRIMARY KEY,
        itm_Id                 INT           NOT NULL,
        refg_Id                INT           NOT NULL,
        lot_Numero             NVARCHAR(60)  NULL,
        lot_Vencimiento        DATE          NULL,
        lot_CantidadInicial    INT           NOT NULL CONSTRAINT DF_tbLotes_CantInicial DEFAULT (0),
        lot_CantidadActual     INT           NOT NULL CONSTRAINT DF_tbLotes_CantActual  DEFAULT (0)
                                             CONSTRAINT CK_tbLotes_CantActual CHECK (lot_CantidadActual >= 0),
        lot_RecepcionDetalleId INT           NULL,
        lot_EsEliminado        BIT           NOT NULL CONSTRAINT DF_tbLotes_EsEliminado DEFAULT (0),
        lot_UsuarioCrea        INT           NOT NULL CONSTRAINT DF_tbLotes_UsuarioCrea DEFAULT (1),
        lot_FechaCrea          DATETIME      NOT NULL CONSTRAINT DF_tbLotes_FechaCrea   DEFAULT (GETDATE()),
        lot_UsuarioModifica    INT           NULL,
        lot_FechaModifica      DATETIME      NULL
    );
END
GO

-- FK a items
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tbLotes_Items')
    AND EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('Inventario.tbItems'))
BEGIN
    ALTER TABLE [Inventario].[tbLotes]
        ADD CONSTRAINT FK_tbLotes_Items FOREIGN KEY (itm_Id) REFERENCES [Inventario].[tbItems](itm_Id);
END
GO

-- FK a refugios
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tbLotes_Refugios')
    AND EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('Refugio.tbRefugios'))
BEGIN
    ALTER TABLE [Inventario].[tbLotes]
        ADD CONSTRAINT FK_tbLotes_Refugios FOREIGN KEY (refg_Id) REFERENCES [Refugio].[tbRefugios](refg_Id);
END
GO

-- Indice FEFO: por item/refugio ordenado por vencimiento
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tbLotes_FEFO' AND object_id = OBJECT_ID('Inventario.tbLotes'))
BEGIN
    CREATE INDEX IX_tbLotes_FEFO
        ON [Inventario].[tbLotes] (itm_Id, refg_Id, lot_Vencimiento)
        INCLUDE (lot_CantidadActual)
        WHERE lot_EsEliminado = 0;
END
GO

-- Unique para que el backfill no duplique lotes de un mismo detalle de recepcion
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_tbLotes_RecepcionDetalle' AND object_id = OBJECT_ID('Inventario.tbLotes'))
BEGIN
    CREATE UNIQUE INDEX UQ_tbLotes_RecepcionDetalle
        ON [Inventario].[tbLotes] (lot_RecepcionDetalleId)
        WHERE lot_RecepcionDetalleId IS NOT NULL;
END
GO

-- =====================================================================
-- 2. SP Inventario.PR_Inventario_Lote_RegistrarEntrada
--    Entrada DIRECTA (fuera de recepciones): crea/incrementa lote y SUBE exi_Cantidad.
--    Nota: el flujo de recepciones NO usa este SP (ahi el trigger ya mueve exi); este
--    SP es para entradas manuales/programaticas y mantiene exi = sum(lotes) por si mismo.
-- =====================================================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Lote_RegistrarEntrada]
    @itm_Id          INT,
    @refg_Id         INT,
    @lot_Numero      NVARCHAR(60) = NULL,
    @lot_Vencimiento DATE         = NULL,
    @cantidad        INT,
    @recepDetId      INT          = NULL,
    @usuario         INT          = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @cantidad IS NULL OR @cantidad <= 0
        BEGIN
            SELECT -2 AS CodeStatus, 'La cantidad debe ser mayor que cero.' AS MessageStatus;
            RETURN;
        END

        BEGIN TRANSACTION;

        DECLARE @lotId INT = NULL;

        -- 1) Si viene de un detalle de recepcion y ya existe su lote, incrementar ese
        IF @recepDetId IS NOT NULL
            SELECT @lotId = lot_Id FROM [Inventario].[tbLotes]
            WHERE lot_RecepcionDetalleId = @recepDetId AND lot_EsEliminado = 0;

        -- 2) Si no, buscar un lote abierto que coincida (mismo numero y vencimiento)
        IF @lotId IS NULL
            SELECT TOP 1 @lotId = lot_Id FROM [Inventario].[tbLotes]
            WHERE itm_Id = @itm_Id AND refg_Id = @refg_Id AND lot_EsEliminado = 0
              AND lot_RecepcionDetalleId IS NULL
              AND ISNULL(lot_Numero,'') = ISNULL(@lot_Numero,'')
              AND ((lot_Vencimiento IS NULL AND @lot_Vencimiento IS NULL) OR lot_Vencimiento = @lot_Vencimiento)
            ORDER BY lot_Id;

        IF @lotId IS NOT NULL
            UPDATE [Inventario].[tbLotes]
            SET lot_CantidadInicial = lot_CantidadInicial + @cantidad,
                lot_CantidadActual  = lot_CantidadActual  + @cantidad,
                lot_UsuarioModifica = @usuario,
                lot_FechaModifica   = GETDATE()
            WHERE lot_Id = @lotId;
        ELSE
            INSERT INTO [Inventario].[tbLotes]
                (itm_Id, refg_Id, lot_Numero, lot_Vencimiento, lot_CantidadInicial, lot_CantidadActual,
                 lot_RecepcionDetalleId, lot_UsuarioCrea, lot_FechaCrea)
            VALUES
                (@itm_Id, @refg_Id, @lot_Numero, @lot_Vencimiento, @cantidad, @cantidad,
                 @recepDetId, @usuario, GETDATE());

        -- Upsert del total autoritativo en existencias
        IF EXISTS (SELECT 1 FROM [Inventario].[tbExistencias] WHERE itm_Id = @itm_Id AND refg_Id = @refg_Id)
            UPDATE [Inventario].[tbExistencias]
            SET exi_Cantidad = exi_Cantidad + @cantidad,
                exi_UltimaActualizacion = GETDATE()
            WHERE itm_Id = @itm_Id AND refg_Id = @refg_Id;
        ELSE
            INSERT INTO [Inventario].[tbExistencias] (itm_Id, refg_Id, exi_Cantidad, exi_UltimaActualizacion)
            VALUES (@itm_Id, @refg_Id, @cantidad, GETDATE());

        COMMIT TRANSACTION;
        SELECT 1 AS CodeStatus, 'Entrada de lote registrada correctamente.' AS MessageStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus;
    END CATCH
END
GO

-- =====================================================================
-- 3. SP Inventario.PR_Inventario_Lotes_Backfill
--    Genera lotes a partir de los detalles de recepcion existentes (recdet_*),
--    sin duplicar (una fila por recdet_Id). Luego CUADRA por (item,refugio) para que
--    sum(lotes) = exi_Cantidad: el sobrante que no tiene lote conocido queda en una
--    fila "sin lote" (lot_Numero NULL); si hubo consumo previo no rastreado, descuenta
--    de los lotes que vencen primero. Idempotente.
-- =====================================================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Lotes_Backfill]
    @usuario INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 3.1 Crear lotes faltantes a partir de recdet (idempotente via lot_RecepcionDetalleId)
        INSERT INTO [Inventario].[tbLotes]
            (itm_Id, refg_Id, lot_Numero, lot_Vencimiento, lot_CantidadInicial, lot_CantidadActual,
             lot_RecepcionDetalleId, lot_UsuarioCrea, lot_FechaCrea)
        SELECT rd.itm_Id, r.refg_Id, rd.recdet_NumeroLote, rd.recdet_FechaVencimiento,
               rd.recdet_Cantidad, rd.recdet_Cantidad, rd.recdet_Id, @usuario, GETDATE()
        FROM [Inventario].[tbRecepcionesDetalles] rd
        INNER JOIN [Inventario].[tbRecepcionesMercancia] r ON rd.recep_Id = r.recep_Id
        WHERE ISNULL(rd.recdet_EsEliminado, 0) = 0
          AND rd.recdet_Cantidad > 0
          AND NOT EXISTS (SELECT 1 FROM [Inventario].[tbLotes] l WHERE l.lot_RecepcionDetalleId = rd.recdet_Id);

        -- 3.2 Cuadrar cada (item,refugio) presente en existencias
        DECLARE @itm INT, @refg INT, @exi INT, @sumRecdet INT, @remanente INT;

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT itm_Id, refg_Id, exi_Cantidad FROM [Inventario].[tbExistencias];
        OPEN cur;
        FETCH NEXT FROM cur INTO @itm, @refg, @exi;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Suma de lotes con origen conocido (de recepcion)
            SELECT @sumRecdet = ISNULL(SUM(lot_CantidadActual), 0)
            FROM [Inventario].[tbLotes]
            WHERE itm_Id = @itm AND refg_Id = @refg AND lot_EsEliminado = 0
              AND lot_RecepcionDetalleId IS NOT NULL;

            SET @remanente = @exi - @sumRecdet;

            IF @remanente >= 0
            BEGIN
                -- Existencias >= lotes conocidos: el resto va a una fila "sin lote" (una por item/refugio)
                DECLARE @sinLoteId INT = NULL;
                SELECT TOP 1 @sinLoteId = lot_Id FROM [Inventario].[tbLotes]
                WHERE itm_Id = @itm AND refg_Id = @refg AND lot_EsEliminado = 0
                  AND lot_RecepcionDetalleId IS NULL AND lot_Numero IS NULL AND lot_Vencimiento IS NULL
                ORDER BY lot_Id;

                IF @remanente > 0
                BEGIN
                    IF @sinLoteId IS NULL
                        INSERT INTO [Inventario].[tbLotes]
                            (itm_Id, refg_Id, lot_Numero, lot_Vencimiento, lot_CantidadInicial, lot_CantidadActual,
                             lot_RecepcionDetalleId, lot_UsuarioCrea, lot_FechaCrea)
                        VALUES (@itm, @refg, NULL, NULL, @remanente, @remanente, NULL, @usuario, GETDATE());
                    ELSE
                        UPDATE [Inventario].[tbLotes]
                        SET lot_CantidadInicial = CASE WHEN lot_CantidadInicial < @remanente THEN @remanente ELSE lot_CantidadInicial END,
                            lot_CantidadActual  = @remanente,
                            lot_UsuarioModifica = @usuario, lot_FechaModifica = GETDATE()
                        WHERE lot_Id = @sinLoteId;
                END
                ELSE IF @sinLoteId IS NOT NULL
                    UPDATE [Inventario].[tbLotes]
                    SET lot_CantidadActual = 0, lot_UsuarioModifica = @usuario, lot_FechaModifica = GETDATE()
                    WHERE lot_Id = @sinLoteId;
            END
            ELSE
            BEGIN
                -- Existencias < lotes conocidos: hubo consumo previo no rastreado.
                -- Descontar FEFO (vence primero) de los lotes de recepcion.
                DECLARE @faltante INT = -@remanente;
                DECLARE @lId INT, @disp INT, @tomar INT;
                DECLARE curL CURSOR LOCAL FAST_FORWARD FOR
                    SELECT lot_Id, lot_CantidadActual FROM [Inventario].[tbLotes]
                    WHERE itm_Id = @itm AND refg_Id = @refg AND lot_EsEliminado = 0
                      AND lot_RecepcionDetalleId IS NOT NULL AND lot_CantidadActual > 0
                    ORDER BY CASE WHEN lot_Vencimiento IS NULL THEN 1 ELSE 0 END, lot_Vencimiento ASC, lot_Id ASC;
                OPEN curL;
                FETCH NEXT FROM curL INTO @lId, @disp;
                WHILE @@FETCH_STATUS = 0 AND @faltante > 0
                BEGIN
                    SET @tomar = CASE WHEN @disp <= @faltante THEN @disp ELSE @faltante END;
                    UPDATE [Inventario].[tbLotes]
                    SET lot_CantidadActual = lot_CantidadActual - @tomar,
                        lot_UsuarioModifica = @usuario, lot_FechaModifica = GETDATE()
                    WHERE lot_Id = @lId;
                    SET @faltante = @faltante - @tomar;
                    FETCH NEXT FROM curL INTO @lId, @disp;
                END
                CLOSE curL; DEALLOCATE curL;
            END

            FETCH NEXT FROM cur INTO @itm, @refg, @exi;
        END
        CLOSE cur; DEALLOCATE cur;

        COMMIT TRANSACTION;
        SELECT 1 AS CodeStatus, 'Backfill de lotes completado.' AS MessageStatus;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        -- Cerrar cursores si quedaron abiertos
        IF CURSOR_STATUS('local','curL') >= 0 BEGIN CLOSE curL; DEALLOCATE curL; END
        IF CURSOR_STATUS('local','cur')  >= 0 BEGIN CLOSE cur;  DEALLOCATE cur;  END
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus;
    END CATCH
END
GO

-- =====================================================================
-- 4. TRIGGER de ENTRADA -> LOTE (aparte, NO toca exi para no duplicar)
--    El trigger existente TR_RecepcionesDetalles_ActualizarExistencias ya sube exi.
--    Este solo crea el lote de cada detalle de recepcion recien insertado.
-- =====================================================================
CREATE OR ALTER TRIGGER [Inventario].[TR_RecepcionesDetalles_ActualizarLotes]
ON [Inventario].[tbRecepcionesDetalles]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [Inventario].[tbLotes]
        (itm_Id, refg_Id, lot_Numero, lot_Vencimiento, lot_CantidadInicial, lot_CantidadActual,
         lot_RecepcionDetalleId, lot_UsuarioCrea, lot_FechaCrea)
    SELECT i.itm_Id, r.refg_Id, i.recdet_NumeroLote, i.recdet_FechaVencimiento,
           i.recdet_Cantidad, i.recdet_Cantidad, i.recdet_Id, i.recdet_UsuarioCrea, GETDATE()
    FROM inserted i
    INNER JOIN [Inventario].[tbRecepcionesMercancia] r ON i.recep_Id = r.recep_Id
    WHERE ISNULL(i.recdet_EsEliminado, 0) = 0
      AND i.recdet_Cantidad > 0
      AND NOT EXISTS (SELECT 1 FROM [Inventario].[tbLotes] l WHERE l.lot_RecepcionDetalleId = i.recdet_Id);
END
GO

-- =====================================================================
-- 5. TRIGGER de SALIDA -> consumo FEFO (AMPLIA el trigger existente)
--    Conserva TODO lo que ya hacia (validar stock, bajar exi_Cantidad, registrar movimiento)
--    y ADEMAS reparte la cantidad saliente entre lotes FEFO. Si no hay lotes, no falla.
-- =====================================================================
CREATE OR ALTER TRIGGER [Inventario].[TR_SalidasDetalles_ActualizarExistencias]
ON [Inventario].[tbSalidasDetalles]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar stock disponible
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN [Inventario].[tbSalidas] s ON i.sal_Id = s.sal_Id
        LEFT JOIN [Inventario].[tbExistencias] e ON i.itm_Id = e.itm_Id AND s.refg_Id = e.refg_Id
        WHERE ISNULL(e.exi_Cantidad, 0) < i.saldet_Cantidad
    )
    BEGIN
        RAISERROR('Stock insuficiente para realizar la salida', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Actualizar existencias (restar) -- comportamiento original intacto
    UPDATE e
    SET e.exi_Cantidad = e.exi_Cantidad - i.saldet_Cantidad,
        e.exi_UltimaActualizacion = GETDATE()
    FROM [Inventario].[tbExistencias] e
    INNER JOIN inserted i ON e.itm_Id = i.itm_Id
    INNER JOIN [Inventario].[tbSalidas] s ON i.sal_Id = s.sal_Id AND e.refg_Id = s.refg_Id;

    -- Registrar movimientos -- comportamiento original intacto
    INSERT INTO [Inventario].[tbMovimientos]
    (mov_TipoMovimiento, mov_DocumentoId, itm_Id, refg_Id,
     mov_Cantidad, mov_SaldoAnterior, mov_SaldoActual, mov_UsuarioCrea)
    SELECT
        'S',
        i.sal_Id,
        i.itm_Id,
        s.refg_Id,
        i.saldet_Cantidad,
        e.exi_Cantidad + i.saldet_Cantidad,
        e.exi_Cantidad,
        i.saldet_UsuarioCrea
    FROM inserted i
    INNER JOIN [Inventario].[tbSalidas] s ON i.sal_Id = s.sal_Id
    INNER JOIN [Inventario].[tbExistencias] e ON i.itm_Id = e.itm_Id AND s.refg_Id = e.refg_Id;

    -- NUEVO: repartir la salida entre lotes FEFO (vence primero sale primero)
    DECLARE @itm INT, @refg INT, @cant INT;
    DECLARE curS CURSOR LOCAL FAST_FORWARD FOR
        SELECT i.itm_Id, s.refg_Id, i.saldet_Cantidad
        FROM inserted i
        INNER JOIN [Inventario].[tbSalidas] s ON i.sal_Id = s.sal_Id;
    OPEN curS;
    FETCH NEXT FROM curS INTO @itm, @refg, @cant;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Solo si el item tiene lotes (si no, se queda con el comportamiento actual)
        IF EXISTS (SELECT 1 FROM [Inventario].[tbLotes]
                   WHERE itm_Id = @itm AND refg_Id = @refg AND lot_EsEliminado = 0 AND lot_CantidadActual > 0)
        BEGIN
            DECLARE @restante INT = @cant, @lotId INT, @disp INT, @tomar INT;
            DECLARE curL CURSOR LOCAL FAST_FORWARD FOR
                SELECT lot_Id, lot_CantidadActual FROM [Inventario].[tbLotes]
                WHERE itm_Id = @itm AND refg_Id = @refg AND lot_EsEliminado = 0 AND lot_CantidadActual > 0
                ORDER BY CASE WHEN lot_Vencimiento IS NULL THEN 1 ELSE 0 END, lot_Vencimiento ASC, lot_Id ASC;
            OPEN curL;
            FETCH NEXT FROM curL INTO @lotId, @disp;
            WHILE @@FETCH_STATUS = 0 AND @restante > 0
            BEGIN
                SET @tomar = CASE WHEN @disp <= @restante THEN @disp ELSE @restante END;
                UPDATE [Inventario].[tbLotes]
                SET lot_CantidadActual = lot_CantidadActual - @tomar,
                    lot_FechaModifica = GETDATE()
                WHERE lot_Id = @lotId;
                SET @restante = @restante - @tomar;
                FETCH NEXT FROM curL INTO @lotId, @disp;
            END
            CLOSE curL; DEALLOCATE curL;
        END
        FETCH NEXT FROM curS INTO @itm, @refg, @cant;
    END
    CLOSE curS; DEALLOCATE curS;
END
GO

-- =====================================================================
-- 6. SP Inventario.PR_Inventario_Lotes_PorVencer  (alertas de caducidad)
--    Lotes con existencia (>0) vencidos o por vencer en @dias dias.
-- =====================================================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Lotes_PorVencer]
    @dias INT = 30
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        l.lot_Id,
        l.itm_Id,
        i.itm_Codigo,
        i.itm_Descripcion,
        c.cat_Descripcion,
        l.refg_Id,
        rf.refg_Nombre,
        l.lot_Numero,
        l.lot_Vencimiento,
        l.lot_CantidadActual,
        DATEDIFF(DAY, CAST(GETDATE() AS DATE), l.lot_Vencimiento) AS DiasRestantes
    FROM [Inventario].[tbLotes] l
    INNER JOIN [Inventario].[tbItems] i ON l.itm_Id = i.itm_Id
    LEFT JOIN [Inventario].[tbCategorias] c ON i.cat_Id = c.cat_Id
    LEFT JOIN [Refugio].[tbRefugios] rf ON l.refg_Id = rf.refg_Id
    WHERE l.lot_EsEliminado = 0
      AND l.lot_CantidadActual > 0
      AND l.lot_Vencimiento IS NOT NULL
      AND l.lot_Vencimiento <= DATEADD(DAY, @dias, CAST(GETDATE() AS DATE))
    ORDER BY l.lot_Vencimiento ASC;
END
GO

-- =====================================================================
-- 7. SP Inventario.PR_Inventario_Lotes_PorItem  (existencias por lote de un item)
-- =====================================================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Lotes_PorItem]
    @itm_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        l.lot_Id,
        l.refg_Id,
        rf.refg_Nombre,
        l.lot_Numero,
        l.lot_Vencimiento,
        l.lot_CantidadActual,
        CASE WHEN l.lot_Vencimiento IS NULL THEN NULL
             ELSE DATEDIFF(DAY, CAST(GETDATE() AS DATE), l.lot_Vencimiento) END AS DiasRestantes
    FROM [Inventario].[tbLotes] l
    LEFT JOIN [Refugio].[tbRefugios] rf ON l.refg_Id = rf.refg_Id
    WHERE l.lot_EsEliminado = 0
      AND l.itm_Id = @itm_Id
      AND l.lot_CantidadActual > 0
    ORDER BY CASE WHEN l.lot_Vencimiento IS NULL THEN 1 ELSE 0 END, l.lot_Vencimiento ASC, l.lot_Id ASC;
END
GO

-- =====================================================================
-- 8. REPOINT de PR_Inventario_Items_PorVencer a la capa de LOTES
--    Mantiene EXACTAMENTE las mismas columnas de salida (contrato de la UI "Por vencer"
--    en Item/Index intacto), pero ahora refleja la cantidad REAL restante por lote
--    e incluye vencidos. Agrega @dias (default 30).
-- =====================================================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Items_PorVencer]
    @dias INT = 30
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        l.itm_Id,
        i.itm_Codigo,
        i.itm_Descripcion,
        c.cat_Descripcion,
        l.lot_Numero          AS recdet_NumeroLote,
        l.lot_CantidadActual  AS recdet_Cantidad,
        l.lot_Vencimiento     AS recdet_FechaVencimiento,
        DATEDIFF(DAY, CAST(GETDATE() AS DATE), l.lot_Vencimiento) AS DiasRestantes
    FROM [Inventario].[tbLotes] l
    INNER JOIN [Inventario].[tbItems] i ON l.itm_Id = i.itm_Id
    LEFT JOIN [Inventario].[tbCategorias] c ON i.cat_Id = c.cat_Id
    WHERE l.lot_EsEliminado = 0
      AND l.lot_CantidadActual > 0
      AND l.lot_Vencimiento IS NOT NULL
      AND l.lot_Vencimiento <= DATEADD(DAY, @dias, CAST(GETDATE() AS DATE))
    ORDER BY l.lot_Vencimiento ASC;
END
GO

-- =====================================================================
-- 9. EJECUTAR BACKFILL (idempotente)
-- =====================================================================
EXEC [Inventario].[PR_Inventario_Lotes_Backfill] @usuario = 1;
GO
