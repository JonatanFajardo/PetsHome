-- ============================================================
-- FIX: Agregar sol_Estado a tbSolicitudes
-- y actualizar SP List para incluir campos del Kanban
-- Ejecutar en: PETSHOMEDB
-- ============================================================

-- 1. Agregar columna sol_Estado
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'Refugio'
      AND TABLE_NAME   = 'tbSolicitudes'
      AND COLUMN_NAME  = 'sol_Estado'
)
BEGIN
    ALTER TABLE [Refugio].[tbSolicitudes]
    ADD sol_Estado VARCHAR(20) NOT NULL DEFAULT 'Pendiente';
    PRINT 'Columna sol_Estado agregada correctamente.';
END
ELSE
    PRINT 'Columna sol_Estado ya existe.';

GO

-- 2. SP CambiarEstado
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Solicitudes_CambiarEstado]
    @sol_Id            INT,
    @sol_Estado        VARCHAR(20),
    @sol_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [Refugio].[tbSolicitudes]
    SET    sol_Estado          = @sol_Estado,
           sol_UsuarioModifica = @sol_UsuarioModifica,
           sol_FechaModifica   = GETDATE()
    WHERE  sol_Id        = @sol_Id
      AND  sol_EsEliminado = 0;

    SELECT CAST(@@ROWCOUNT AS BIT) AS Success,
           NULL AS Message;
END
GO

-- 3. SP List actualizado con campos del Kanban
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Solicitudes_List]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.sol_Id,
        s.sol_Identidad,
        s.sol_Nombres,
        m.masc_Nombre,
        s.sol_Correo,
        ISNULL(s.sol_Estado, 'Pendiente')               AS sol_Estado,
        CONVERT(VARCHAR(10), s.sol_Fecha, 103)           AS sol_Fecha,
        ISNULL(r.raza_Descripcion, '')                   AS raza_Descripcion,
        ISNULL(r.raza_TipoAnimal,  'Otro')               AS raza_TipoAnimal
    FROM  [Refugio].[tbSolicitudes]  s
    INNER JOIN [Refugio].[tbMascotas] m ON s.masc_Id = m.masc_Id
    LEFT  JOIN [Refugio].[tbRazas]    r ON m.raza_Id  = r.raza_Id
    WHERE  s.sol_EsEliminado = 0
    ORDER  BY s.sol_FechaCrea DESC;
END
GO
