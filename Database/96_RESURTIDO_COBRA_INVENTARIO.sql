/*
=============================================
SCRIPT: 96 - El RESURTIDO de una receta descuenta inventario y encola el cargo
AUTOR: Sistema PetsHome
FECHA: 2026-10-30
Idempotente. GO entre lotes. Textos SIN acentos.

Replica la ENTREGA ORIGINAL de medicamentos (ConsultaService): al resurtir una
receta ligada a un item del inventario, se crea una SALIDA (baja stock, los
triggers de tbSalidasDetalles validan existencia y descuentan) y, si el paciente
es de un cliente externo, se encola el CARGO por cobrar (Pendiente).

CANDADOS verificados en la BD viva:
  - UQ_CargoDispensacion_Receta : UNIQUE FILTRADO (receta_Id IS NOT NULL) -> admite
    multiples NULL. El cargo del resurtido se inserta con receta_Id = NULL para no
    chocar con el cargo de la entrega original (que usa el mismo receta_Id).
  - UQ_saldet_Receta            : UNIQUE FILTRADO (saldet_RecetaId IS NOT NULL) -> la
    salida del resurtido usa saldet_RecetaId = NULL; la traza va en observaciones.
  - cargo_TipoOrigen            : NO tiene CHECK constraint (solo DEFAULT 'Medicamento').
    Por eso usamos cargo_TipoOrigen = 'Resurtido' SIN tocar ningun CHECK (mas limpio
    y distingue el cargo del resurtido del de la entrega original). cargo_OrigenId
    apunta al rsrt_Id del resurtido.
=============================================
*/
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

USE PETSHOMEDB
GO

/* =========================================================
   PARTE 1 - Trazabilidad en la bitacora de resurtidos
   (que salida y que cargo genero cada resurtido)
   ========================================================= */
IF COL_LENGTH('Medico.tbRecetaResurtido', 'rsrt_SalidaId') IS NULL
    ALTER TABLE [Medico].[tbRecetaResurtido] ADD rsrt_SalidaId INT NULL
GO
IF COL_LENGTH('Medico.tbRecetaResurtido', 'rsrt_CargoId') IS NULL
    ALTER TABLE [Medico].[tbRecetaResurtido] ADD rsrt_CargoId INT NULL
GO

/* =========================================================
   PARTE 2 - Recrear el SP de resurtido: contar + descontar + cobrar
   Todo en una transaccion (XACT_ABORT ON). Si algo falla (incluido el
   trigger de stock), se revierte TODO: nunca se incrementa usados.
   NO cambia la firma (@receta_Id, @usuario) ni el shape de salida
   (CodeStatus, MessageStatus, receta_ResurtidosUsados, receta_ResurtidosAutorizados).
   ========================================================= */
IF OBJECT_ID('Medico.PR_Medico_Recetas_Resurtir', 'P') IS NOT NULL
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Resurtir]
GO
CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Resurtir]
    @receta_Id INT,
    @usuario   INT
AS
BEGIN
    SET NOCOUNT ON
    SET XACT_ABORT ON

    DECLARE @auth INT, @usad INT,
            @masc_Id INT, @cita_Id INT, @itm_Id INT, @cant INT,
            @cli_Id INT, @mascRefg INT, @mascNombre NVARCHAR(200),
            @refg_Id INT, @disp INT,
            @salId INT, @cargoId INT, @rsrtId INT,
            @precioSnap DECIMAL(18,2), @descSnap NVARCHAR(500),
            @tipoSalida CHAR(1), @nuevoUsad INT, @huboCargo BIT = 0;

    -- a. La receta existe y tiene resurtidos disponibles
    SELECT @auth       = receta_ResurtidosAutorizados,
           @usad       = receta_ResurtidosUsados,
           @masc_Id    = masc_Id,
           @cita_Id    = cita_Id,
           @itm_Id     = itm_Id,
           @cant       = receta_Cantidad
    FROM [Medico].[tbRecetas]
    WHERE receta_Id = @receta_Id AND receta_EsEliminado = 0

    IF @auth IS NULL
    BEGIN
        SELECT -1 AS CodeStatus,
               N'La receta no fue encontrada.' AS MessageStatus,
               0 AS receta_ResurtidosUsados,
               0 AS receta_ResurtidosAutorizados
        RETURN
    END

    IF @usad >= @auth
    BEGIN
        SELECT -1 AS CodeStatus,
               N'No quedan resurtidos autorizados.' AS MessageStatus,
               @usad AS receta_ResurtidosUsados,
               @auth AS receta_ResurtidosAutorizados
        RETURN
    END

    -- Cantidad a entregar: al menos 1 (receta_Cantidad puede venir NULL/0)
    SET @cant = ISNULL(NULLIF(@cant, 0), 1)
    SET @nuevoUsad = @usad + 1

    -- b. Cliente/refugio/nombre de la mascota
    SELECT @mascRefg   = refg_Id,
           @cli_Id     = cli_Id,
           @mascNombre = masc_Nombre
    FROM [Refugio].[tbMascotas]
    WHERE masc_Id = @masc_Id

    BEGIN TRY
        BEGIN TRANSACTION

        -- c. Entrega del inventario SOLO si la receta salio de un item real.
        IF @itm_Id IS NOT NULL
        BEGIN
            -- Resolver el refugio: primero el de la mascota; si no, el del vet.
            SET @refg_Id = @mascRefg
            IF @refg_Id IS NULL OR @refg_Id = 0
                SELECT @refg_Id =
                    (SELECT TOP (1) e.refg_Id
                     FROM [Seguridad].[tbUsuarios] u
                     JOIN [Refugio].[tbEmpleados] e ON e.Emp_Id = u.Emp_Id
                     WHERE u.usu_Id = @usuario AND e.refg_Id IS NOT NULL)

            IF @refg_Id IS NULL OR @refg_Id = 0
            BEGIN
                ROLLBACK TRANSACTION
                SELECT -2 AS CodeStatus,
                       N'No se pudo resolver el refugio para descontar el inventario.' AS MessageStatus,
                       @usad AS receta_ResurtidosUsados,
                       @auth AS receta_ResurtidosAutorizados
                RETURN
            END

            -- Mensaje amable antes del trigger: existencia suficiente
            SET @disp = ISNULL(
                (SELECT TOP (1) exi_Cantidad FROM [Inventario].[tbExistencias]
                 WHERE itm_Id = @itm_Id AND refg_Id = @refg_Id), 0)

            IF @disp < @cant
            BEGIN
                ROLLBACK TRANSACTION
                SELECT -3 AS CodeStatus,
                       N'Stock insuficiente para resurtir (disponible ' + CAST(@disp AS NVARCHAR(10))
                           + N', requerido ' + CAST(@cant AS NVARCHAR(10)) + N').' AS MessageStatus,
                       @usad AS receta_ResurtidosUsados,
                       @auth AS receta_ResurtidosAutorizados
                RETURN
            END

            -- Header de salida. Cliente externo => Venta ('V'); interno => Uso ('U').
            SET @tipoSalida = CASE WHEN @cli_Id IS NOT NULL THEN 'V' ELSE 'U' END
            INSERT INTO [Inventario].[tbSalidas]
                (sal_Descripcion, sal_TipoSalida, refg_Id, sal_Fecha,
                 sal_EsEliminado, sal_UsuarioCrea, sal_FechaCrea)
            VALUES
                (N'Resurtido receta #' + CAST(@receta_Id AS NVARCHAR(10)) + N' - ' + ISNULL(@mascNombre, N'Mascota'),
                 @tipoSalida, @refg_Id, GETDATE(), 0, @usuario, GETDATE())
            SET @salId = CAST(SCOPE_IDENTITY() AS INT)

            -- Snapshot de precio/descripcion del item (para salida y cargo)
            SELECT @precioSnap = itm_Precio, @descSnap = itm_Descripcion
            FROM [Inventario].[tbItems] WHERE itm_Id = @itm_Id

            -- Detalle de salida. saldet_RecetaId = NULL (esquiva UQ_saldet_Receta); la
            -- traza va en observaciones. El trigger valida existencia y descuenta stock.
            INSERT INTO [Inventario].[tbSalidasDetalles]
                (sal_Id, itm_Id, saldet_Cantidad, saldet_Observaciones,
                 saldet_RecetaId, saldet_PrecioUnitario, saldet_DescripcionItem,
                 saldet_EsEliminado, saldet_UsuarioCrea, saldet_FechaCrea)
            VALUES
                (@salId, @itm_Id, @cant,
                 N'Resurtido receta #' + CAST(@receta_Id AS NVARCHAR(10)),
                 NULL, @precioSnap, @descSnap, 0, @usuario, GETDATE())

            -- Cargo por cobrar SOLO para cliente externo (feed, no gate): el interno
            -- descuenta inventario pero no se cobra.
            IF @cli_Id IS NOT NULL
            BEGIN
                INSERT INTO [Veterinaria].[tbCargoDispensacion]
                    (receta_Id, masc_Id, cli_Id, cita_Id, itm_Id, cargo_Cantidad,
                     cargo_DescripcionSnap, cargo_PrecioUnitSnap, cargo_Estado,
                     cargo_TipoOrigen, cargo_OrigenId,
                     cargo_EsEliminado, cargo_UsuarioCrea, cargo_FechaCrea)
                VALUES
                    (NULL, @masc_Id, @cli_Id, @cita_Id, @itm_Id, @cant,
                     @descSnap, @precioSnap, N'Pendiente',
                     N'Resurtido', NULL,   -- cargo_OrigenId se liga al rsrt_Id abajo
                     0, @usuario, GETDATE())
                SET @cargoId = CAST(SCOPE_IDENTITY() AS INT)
                SET @huboCargo = 1
            END
        END

        -- d. Contar el resurtido + bitacora (con la traza de salida/cargo)
        UPDATE [Medico].[tbRecetas]
        SET receta_ResurtidosUsados = receta_ResurtidosUsados + 1
        WHERE receta_Id = @receta_Id

        INSERT INTO [Medico].[tbRecetaResurtido]
            (receta_Id, rsrt_Usuario, rsrt_SalidaId, rsrt_CargoId)
        VALUES
            (@receta_Id, @usuario, @salId, @cargoId)
        SET @rsrtId = CAST(SCOPE_IDENTITY() AS INT)

        -- Ligar el cargo a este resurtido (origen trazable)
        IF @cargoId IS NOT NULL
            UPDATE [Veterinaria].[tbCargoDispensacion]
            SET cargo_OrigenId = @rsrtId
            WHERE cargo_Id = @cargoId

        COMMIT TRANSACTION

        SELECT 1 AS CodeStatus,
               CASE WHEN @huboCargo = 1 THEN N'Resurtido entregado y cobrado.'
                    WHEN @salId IS NOT NULL THEN N'Resurtido entregado del inventario.'
                    ELSE N'Resurtido registrado.' END AS MessageStatus,
               @nuevoUsad AS receta_ResurtidosUsados,
               @auth AS receta_ResurtidosAutorizados
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus,
               ERROR_MESSAGE() AS MessageStatus,
               @usad AS receta_ResurtidosUsados,
               @auth AS receta_ResurtidosAutorizados
    END CATCH
END
GO

PRINT '=========================================='
PRINT 'Script 96: Resurtido descuenta inventario y encola cargo aplicado.'
PRINT '=========================================='
GO
