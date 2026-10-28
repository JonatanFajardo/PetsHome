/*
=============================================
SCRIPT: Modulo SUSTANCIAS CONTROLADAS (bitacora regulatoria)
AUTOR:  Sistema PetsHome
FECHA:  2026-10-27
DETALLE: Tabla Medico.tbSustanciaMovimiento (prefijo scm_) + SPs.
         Registra movimientos de farmacos controlados:
            Recibido | Administrado | Desechado | Devuelto
         El SALDO por farmaco se calcula (no se guarda):
            SALDO = SUM(Recibido + Devuelto) - SUM(Administrado + Desechado)
         Reglas: Administrado exige mascota; Desechado exige testigo;
                 cantidad > 0; el saldo no puede quedar negativo al
                 Administrar o Desechar.
         Aditivo: referencia Inventario.tbItems(itm_Id) para reusar el
         catalogo de productos; NO altera tbItems.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE PETSHOMEDB
GO

-- =====================================================
-- TABLA: Medico.tbSustanciaMovimiento
-- =====================================================
IF OBJECT_ID('Medico.tbSustanciaMovimiento', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbSustanciaMovimiento]
    (
        scm_Id              INT IDENTITY(1,1) NOT NULL,
        itm_Id              INT NOT NULL,                      -- farmaco controlado (FK tbItems)
        scm_Tipo            NVARCHAR(20)  NOT NULL,            -- Recibido|Administrado|Desechado|Devuelto (sin tildes)
        scm_Cantidad        DECIMAL(18,3) NOT NULL,
        masc_Id             INT           NULL,                -- obligatorio si Administrado
        scm_TestigoId       INT           NULL,                -- obligatorio si Desechado (FK tbUsuarios)
        scm_Lote            NVARCHAR(64)  NULL,
        scm_DeaSchedule     NVARCHAR(10)  NULL,                -- II|III|IV|V
        scm_Notas           NVARCHAR(1000) NULL,
        scm_Fecha           DATETIME      NOT NULL DEFAULT GETDATE(),
        scm_EsEliminado     BIT           NOT NULL DEFAULT 0,
        scm_UsuarioCrea     INT           NOT NULL,
        scm_FechaCrea       DATETIME      NOT NULL DEFAULT GETDATE(),
        scm_UsuarioModifica INT           NULL,
        scm_FechaModifica   DATETIME      NULL,
        CONSTRAINT PK_tbSustanciaMovimiento PRIMARY KEY (scm_Id),
        CONSTRAINT FK_tbSustanciaMovimiento_Item    FOREIGN KEY (itm_Id)          REFERENCES [Inventario].[tbItems](itm_Id),
        CONSTRAINT FK_tbSustanciaMovimiento_Mascota FOREIGN KEY (masc_Id)         REFERENCES [Refugio].[tbMascotas](masc_Id),
        CONSTRAINT FK_tbSustanciaMovimiento_Testigo FOREIGN KEY (scm_TestigoId)   REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT FK_tbSustanciaMovimiento_UsuCrea FOREIGN KEY (scm_UsuarioCrea) REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT CK_tbSustanciaMovimiento_Tipo CHECK (scm_Tipo IN ('Recibido','Administrado','Desechado','Devuelto')),
        CONSTRAINT CK_tbSustanciaMovimiento_Cantidad CHECK (scm_Cantidad > 0)
    )
END
GO

-- Indice para calcular saldo por farmaco rapido
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tbSustanciaMovimiento_Item' AND object_id = OBJECT_ID('Medico.tbSustanciaMovimiento'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_tbSustanciaMovimiento_Item
        ON [Medico].[tbSustanciaMovimiento] (itm_Id, scm_EsEliminado)
        INCLUDE (scm_Tipo, scm_Cantidad)
END
GO

-- =====================================================
-- SP: PR_Medico_SustanciaMovimiento_List
--   Lista movimientos (todos o por farmaco) con joins a
--   item / mascota / testigo / usuario que registra.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_SustanciaMovimiento_List]
    @itm_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(ORDER BY scm.scm_Fecha DESC, scm.scm_Id DESC) AS Fila,
        scm.scm_Id,
        scm.itm_Id,
        itm.itm_Codigo,
        itm.itm_Descripcion                 AS Farmaco,
        scm.scm_Tipo,
        scm.scm_Cantidad,
        scm.masc_Id,
        masc.masc_Nombre                    AS Mascota,
        scm.scm_TestigoId,
        testigo.Usu_Nombre                  AS Testigo,
        scm.scm_Lote,
        scm.scm_DeaSchedule,
        scm.scm_Notas,
        scm.scm_Fecha,
        scm.scm_UsuarioCrea,
        usuCrea.Usu_Nombre                  AS RegistradoPor
    FROM [Medico].[tbSustanciaMovimiento] AS scm
    INNER JOIN [Inventario].[tbItems]    AS itm     ON scm.itm_Id = itm.itm_Id
    LEFT  JOIN [Refugio].[tbMascotas]    AS masc    ON scm.masc_Id = masc.masc_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]  AS testigo ON scm.scm_TestigoId = testigo.usu_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]  AS usuCrea ON scm.scm_UsuarioCrea = usuCrea.usu_Id
    WHERE scm.scm_EsEliminado = 0
      AND (@itm_Id IS NULL OR scm.itm_Id = @itm_Id)
    ORDER BY scm.scm_Fecha DESC, scm.scm_Id DESC
END
GO

-- =====================================================
-- SP: PR_Medico_SustanciaMovimiento_Detail (una fila)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_SustanciaMovimiento_Detail]
    @scm_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        scm.scm_Id, scm.itm_Id, itm.itm_Codigo, itm.itm_Descripcion AS Farmaco,
        scm.scm_Tipo, scm.scm_Cantidad, scm.masc_Id, masc.masc_Nombre AS Mascota,
        scm.scm_TestigoId, testigo.Usu_Nombre AS Testigo,
        scm.scm_Lote, scm.scm_DeaSchedule, scm.scm_Notas, scm.scm_Fecha,
        usuCrea.Usu_Nombre AS RegistradoPor, scm.scm_FechaCrea
    FROM [Medico].[tbSustanciaMovimiento] AS scm
    INNER JOIN [Inventario].[tbItems]    AS itm     ON scm.itm_Id = itm.itm_Id
    LEFT  JOIN [Refugio].[tbMascotas]    AS masc    ON scm.masc_Id = masc.masc_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]  AS testigo ON scm.scm_TestigoId = testigo.usu_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]  AS usuCrea ON scm.scm_UsuarioCrea = usuCrea.usu_Id
    WHERE scm.scm_Id = @scm_Id AND scm.scm_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_SustanciaSaldo_List (resumen de saldos)
--   Un renglon por farmaco con totales y saldo calculado.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_SustanciaSaldo_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(ORDER BY itm.itm_Descripcion ASC) AS Fila,
        scm.itm_Id,
        itm.itm_Codigo,
        itm.itm_Descripcion AS Farmaco,
        SUM(CASE WHEN scm.scm_Tipo = 'Recibido'     THEN scm.scm_Cantidad ELSE 0 END) AS TotalRecibido,
        SUM(CASE WHEN scm.scm_Tipo = 'Devuelto'     THEN scm.scm_Cantidad ELSE 0 END) AS TotalDevuelto,
        SUM(CASE WHEN scm.scm_Tipo = 'Administrado' THEN scm.scm_Cantidad ELSE 0 END) AS TotalAdministrado,
        SUM(CASE WHEN scm.scm_Tipo = 'Desechado'    THEN scm.scm_Cantidad ELSE 0 END) AS TotalDesechado,
        SUM(CASE WHEN scm.scm_Tipo IN ('Recibido','Devuelto')     THEN scm.scm_Cantidad ELSE 0 END)
          - SUM(CASE WHEN scm.scm_Tipo IN ('Administrado','Desechado') THEN scm.scm_Cantidad ELSE 0 END) AS Saldo,
        MAX(scm.scm_DeaSchedule) AS DeaSchedule,
        COUNT(1)                 AS TotalMovimientos
    FROM [Medico].[tbSustanciaMovimiento] AS scm
    INNER JOIN [Inventario].[tbItems] AS itm ON scm.itm_Id = itm.itm_Id
    WHERE scm.scm_EsEliminado = 0
    GROUP BY scm.itm_Id, itm.itm_Codigo, itm.itm_Descripcion
    ORDER BY itm.itm_Descripcion ASC
END
GO

-- =====================================================
-- SP: PR_Medico_SustanciaMovimiento_RegistrarMovimiento
--   Valida testigo / paciente / cantidad / saldo y registra.
--   Retorna { CodeStatus, MessageStatus, scm_Id }.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_SustanciaMovimiento_RegistrarMovimiento]
    @itm_Id          INT,
    @scm_Tipo        NVARCHAR(20),
    @scm_Cantidad    DECIMAL(18,3),
    @masc_Id         INT = NULL,
    @scm_TestigoId   INT = NULL,
    @scm_Lote        NVARCHAR(64) = NULL,
    @scm_DeaSchedule NVARCHAR(10) = NULL,
    @scm_Notas       NVARCHAR(1000) = NULL,
    @scm_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        -- Tipo valido
        IF @scm_Tipo NOT IN ('Recibido','Administrado','Desechado','Devuelto')
        BEGIN
            SELECT -1 AS CodeStatus, 'Tipo de movimiento no valido.' AS MessageStatus, 0 AS scm_Id
            RETURN
        END

        -- Cantidad positiva
        IF @scm_Cantidad IS NULL OR @scm_Cantidad <= 0
        BEGIN
            SELECT -2 AS CodeStatus, 'La cantidad debe ser mayor que cero.' AS MessageStatus, 0 AS scm_Id
            RETURN
        END

        -- Farmaco existe
        IF NOT EXISTS (SELECT 1 FROM [Inventario].[tbItems] WHERE itm_Id = @itm_Id AND itm_EsEliminado = 0)
        BEGIN
            SELECT -3 AS CodeStatus, 'El farmaco indicado no existe.' AS MessageStatus, 0 AS scm_Id
            RETURN
        END

        -- Administrado exige paciente
        IF @scm_Tipo = 'Administrado' AND @masc_Id IS NULL
        BEGIN
            SELECT -4 AS CodeStatus, 'Debe indicar el paciente al administrar una sustancia controlada.' AS MessageStatus, 0 AS scm_Id
            RETURN
        END

        -- Desechado exige testigo
        IF @scm_Tipo = 'Desechado' AND @scm_TestigoId IS NULL
        BEGIN
            SELECT -5 AS CodeStatus, 'El desecho de una sustancia controlada requiere un testigo.' AS MessageStatus, 0 AS scm_Id
            RETURN
        END

        -- Paciente existe (si se indico)
        IF @masc_Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Refugio].[tbMascotas] WHERE masc_Id = @masc_Id)
        BEGIN
            SELECT -6 AS CodeStatus, 'El paciente indicado no existe.' AS MessageStatus, 0 AS scm_Id
            RETURN
        END

        -- Testigo existe (si se indico)
        IF @scm_TestigoId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE usu_Id = @scm_TestigoId AND Usu_EsEliminado = 0)
        BEGIN
            SELECT -7 AS CodeStatus, 'El testigo indicado no existe.' AS MessageStatus, 0 AS scm_Id
            RETURN
        END

        -- Validar saldo cuando el movimiento consume (Administrado / Desechado)
        IF @scm_Tipo IN ('Administrado','Desechado')
        BEGIN
            DECLARE @saldo DECIMAL(18,3) =
            (
                SELECT
                    SUM(CASE WHEN scm_Tipo IN ('Recibido','Devuelto')     THEN scm_Cantidad ELSE 0 END)
                  - SUM(CASE WHEN scm_Tipo IN ('Administrado','Desechado') THEN scm_Cantidad ELSE 0 END)
                FROM [Medico].[tbSustanciaMovimiento]
                WHERE itm_Id = @itm_Id AND scm_EsEliminado = 0
            )
            SET @saldo = ISNULL(@saldo, 0)

            IF @scm_Cantidad > @saldo
            BEGIN
                SELECT -8 AS CodeStatus,
                       'Saldo insuficiente del farmaco: disponible ' + CONVERT(NVARCHAR(30), @saldo) + '.' AS MessageStatus,
                       0 AS scm_Id
                RETURN
            END
        END

        INSERT INTO [Medico].[tbSustanciaMovimiento]
            (itm_Id, scm_Tipo, scm_Cantidad, masc_Id, scm_TestigoId, scm_Lote,
             scm_DeaSchedule, scm_Notas, scm_Fecha, scm_UsuarioCrea, scm_FechaCrea)
        VALUES
            (@itm_Id, @scm_Tipo, @scm_Cantidad, @masc_Id,
             @scm_TestigoId, @scm_Lote, @scm_DeaSchedule, @scm_Notas,
             GETDATE(), @scm_UsuarioCrea, GETDATE())

        SELECT 1 AS CodeStatus, 'Movimiento registrado correctamente.' AS MessageStatus,
               CAST(SCOPE_IDENTITY() AS INT) AS scm_Id
    END TRY
    BEGIN CATCH
        SELECT -99 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus, 0 AS scm_Id
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_SustanciaMovimiento_Delete (soft delete)
--   Un movimiento mal registrado se anula (no se borra fisico).
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_SustanciaMovimiento_Delete]
    @scm_Id  INT,
    @usuario INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbSustanciaMovimiento]
        SET scm_EsEliminado = 1, scm_UsuarioModifica = @usuario, scm_FechaModifica = GETDATE()
        WHERE scm_Id = @scm_Id AND scm_EsEliminado = 0

        SELECT 1 AS CodeStatus, 'Movimiento anulado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -99 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

PRINT 'Modulo SUSTANCIAS CONTROLADAS: tabla y SPs creados correctamente.'
GO
