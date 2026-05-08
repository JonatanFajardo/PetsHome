-- Item 7: agregar estado/fecha a la tabla puente, SPs de voluntarios por evento

ALTER TABLE [Refugio].[tbEventos_tbVoluntarios]
    ADD evevol_Estado VARCHAR(20) NOT NULL
            CONSTRAINT DF_evevol_Estado DEFAULT 'Pendiente',
        evevol_FechaConfirmacion DATETIME NULL;
GO

CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_EventoVoluntarios_List]
    @eve_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ev.evevol_Id,
        ev.eve_Id,
        ev.vol_Id,
        LTRIM(CONCAT(p.per_PrimerNombre, ' ',
              ISNULL(p.per_SegundoNombre + ' ', ''),
              p.per_ApellidoPaterno, ' ',
              ISNULL(p.per_ApellidoMaterno, ''))) AS vol_NombreCompleto,
        p.per_Telefono,
        ev.evevol_Estado,
        ev.evevol_FechaConfirmacion
    FROM [Refugio].[tbEventos_tbVoluntarios] ev
    INNER JOIN [Refugio].[tbVoluntarios] v  ON ev.vol_Id = v.vol_Id
    INNER JOIN [General].[tbPersonas]    p  ON v.per_Id  = p.per_Id
    WHERE ev.eve_Id = @eve_Id
    ORDER BY ev.evevol_Estado, vol_NombreCompleto;
END
GO

CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_EventoVoluntarios_CambiarEstado]
    @evevol_Id INT,
    @evevol_Estado VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [Refugio].[tbEventos_tbVoluntarios]
    SET evevol_Estado            = @evevol_Estado,
        evevol_FechaConfirmacion = CASE WHEN @evevol_Estado = 'Confirmado' THEN GETDATE() ELSE NULL END
    WHERE evevol_Id = @evevol_Id;
    SELECT @@ROWCOUNT AS Resultado;
END
GO

CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Voluntarios_Disponibles]
    @eve_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        v.vol_Id,
        LTRIM(CONCAT(p.per_PrimerNombre, ' ',
              ISNULL(p.per_SegundoNombre + ' ', ''),
              p.per_ApellidoPaterno, ' ',
              ISNULL(p.per_ApellidoMaterno, ''))) AS vol_NombreCompleto,
        p.per_Telefono
    FROM [Refugio].[tbVoluntarios] v
    INNER JOIN [General].[tbPersonas] p ON v.per_Id = p.per_Id
    WHERE p.per_EsEliminado = 0
      AND v.vol_Id NOT IN (
          SELECT vol_Id FROM [Refugio].[tbEventos_tbVoluntarios] WHERE eve_Id = @eve_Id
      )
    ORDER BY vol_NombreCompleto;
END
GO

CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_EventoVoluntarios_Asignar]
    @eve_Id INT,
    @vol_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (
        SELECT 1 FROM [Refugio].[tbEventos_tbVoluntarios]
        WHERE eve_Id = @eve_Id AND vol_Id = @vol_Id
    )
    BEGIN
        INSERT INTO [Refugio].[tbEventos_tbVoluntarios] (eve_Id, vol_Id, evevol_Estado)
        VALUES (@eve_Id, @vol_Id, 'Pendiente');
        SELECT 1 AS Resultado;
    END
    ELSE
        SELECT 0 AS Resultado;
END
GO
