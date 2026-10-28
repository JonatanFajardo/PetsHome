/*
=============================================
SCRIPT: Modulo LISTA DE ESPERA POR DEMANDA (waitlist)
AUTOR:  Sistema PetsHome
FECHA:  2026-10-27
DETALLE: Tabla Medico.tbListaEspera + SPs de lectura/escritura.
         Es DISTINTO de la "sala de espera del dia" (Medico.tbCitaMedica
         con estado 'En espera' + tablero Kanban). Aquello es la cola de
         HOY (pacientes que YA llegaron). ESTO es demanda SIN cupo todavia:
         "aviseme si se libera un espacio". Nadie ha llegado; quieren que
         les avisen cuando se libere una cita.

         Flujo (feed, no gate): la lista de espera ALIMENTA/sugiere a quien
         llamar cuando se cancela/libera una cita; nunca bloquea el agendado
         normal.

         Estados (ALMACENADOS sin tildes; la UI los muestra con tilde):
            'Esperando' -> 'Agendada' | 'Cancelada'
         Prioridad: 'Normal' | 'Alta' | 'Urgente'

         Aditivo: no reemplaza nada. El cli_Id (dueño) se resuelve de la
         mascota si no se envia. Convive con tbCitaMedica intacto.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE PETSHOMEDB
GO

-- =====================================================
-- TABLA: Medico.tbListaEspera
-- =====================================================
IF OBJECT_ID('Medico.tbListaEspera', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbListaEspera]
    (
        lesp_Id              INT IDENTITY(1,1) NOT NULL,
        masc_Id              INT           NOT NULL,                 -- mascota que espera cupo
        cli_Id               INT           NULL,                    -- dueño/cliente (se resuelve de la mascota)
        tipoCon_Id           INT           NULL,                    -- tipo de consulta deseado (NULL = cualquiera)
        lesp_FechaDeseada    DATE          NULL,                    -- "para cuando" lo quieren (NULL = lo antes posible)
        lesp_Prioridad       NVARCHAR(20)  NOT NULL DEFAULT 'Normal', -- Normal|Alta|Urgente
        lesp_Estado          NVARCHAR(20)  NOT NULL DEFAULT 'Esperando', -- Esperando|Agendada|Cancelada (sin tildes)
        lesp_Notas           NVARCHAR(1000) NULL,
        lesp_EsEliminado     BIT           NOT NULL DEFAULT 0,
        lesp_UsuarioCrea     INT           NOT NULL,
        lesp_FechaCrea       DATETIME      NOT NULL DEFAULT GETDATE(),
        lesp_UsuarioModifica INT           NULL,
        lesp_FechaModifica   DATETIME      NULL,
        CONSTRAINT PK_tbListaEspera PRIMARY KEY (lesp_Id),
        CONSTRAINT FK_tbListaEspera_Mascota FOREIGN KEY (masc_Id)          REFERENCES [Refugio].[tbMascotas](masc_Id),
        CONSTRAINT FK_tbListaEspera_Cliente FOREIGN KEY (cli_Id)           REFERENCES [Veterinaria].[tbClientes](cli_Id),
        CONSTRAINT FK_tbListaEspera_Tipo    FOREIGN KEY (tipoCon_Id)       REFERENCES [Medico].[tbTiposConsulta](tipoCon_Id),
        CONSTRAINT FK_tbListaEspera_UsuCrea FOREIGN KEY (lesp_UsuarioCrea) REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT CK_tbListaEspera_Prioridad CHECK (lesp_Prioridad IN ('Normal','Alta','Urgente')),
        CONSTRAINT CK_tbListaEspera_Estado    CHECK (lesp_Estado    IN ('Esperando','Agendada','Cancelada'))
    )
    PRINT 'Tabla Medico.tbListaEspera creada.'
END
ELSE
    PRINT 'La tabla Medico.tbListaEspera ya existia, se omite.'
GO

-- Indice filtrado: acelera la cola FIFO de los que aun esperan.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tbListaEspera_Esperando')
BEGIN
    CREATE NONCLUSTERED INDEX IX_tbListaEspera_Esperando
        ON [Medico].[tbListaEspera] (lesp_FechaCrea)
        INCLUDE (masc_Id, tipoCon_Id, lesp_FechaDeseada, lesp_Prioridad)
        WHERE lesp_Estado = 'Esperando' AND lesp_EsEliminado = 0
    PRINT 'Indice IX_tbListaEspera_Esperando creado.'
END
GO

-- =====================================================
-- SP: PR_Medico_ListaEspera_List
--   Lista la lista de espera. Orden: primero los que aun esperan,
--   luego por prioridad (Urgente > Alta > Normal) y FIFO por antiguedad.
--   Filtros opcionales por mascota, tipo de consulta y estado.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ListaEspera_List]
    @masc_Id    INT = NULL,
    @tipoCon_Id INT = NULL,
    @estado     NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(
            ORDER BY
                CASE lesp.lesp_Estado WHEN 'Esperando' THEN 0 ELSE 1 END,
                CASE lesp.lesp_Prioridad WHEN 'Urgente' THEN 0 WHEN 'Alta' THEN 1 ELSE 2 END,
                lesp.lesp_FechaCrea ASC
        ) AS Fila,
        lesp.lesp_Id,
        lesp.masc_Id,
        masc.masc_Nombre            AS Mascota,
        lesp.cli_Id,
        CASE WHEN lesp.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(cli.cli_Nombres + ' ' + ISNULL(cli.cli_Apellidos, '')))
             ELSE 'Interno (refugio)' END AS Responsable,
        CASE WHEN lesp.cli_Id IS NOT NULL THEN cli.cli_Telefono ELSE NULL END AS Telefono,
        lesp.tipoCon_Id,
        ISNULL(tc.tipoCon_Descripcion, 'Cualquiera') AS TipoConsulta,
        lesp.lesp_FechaDeseada,
        lesp.lesp_Prioridad,
        lesp.lesp_Estado,
        lesp.lesp_Notas,
        lesp.lesp_FechaCrea,
        DATEDIFF(DAY, lesp.lesp_FechaCrea, GETDATE()) AS DiasEsperando
    FROM [Medico].[tbListaEspera] AS lesp
    INNER JOIN [Refugio].[tbMascotas]      AS masc ON lesp.masc_Id = masc.masc_Id
    LEFT  JOIN [Veterinaria].[tbClientes]   AS cli  ON lesp.cli_Id = cli.cli_Id
    LEFT  JOIN [Medico].[tbTiposConsulta]   AS tc   ON lesp.tipoCon_Id = tc.tipoCon_Id
    WHERE lesp.lesp_EsEliminado = 0
      AND (@masc_Id    IS NULL OR lesp.masc_Id    = @masc_Id)
      AND (@tipoCon_Id IS NULL OR lesp.tipoCon_Id = @tipoCon_Id)
      AND (@estado     IS NULL OR lesp.lesp_Estado = @estado)
    ORDER BY
        CASE lesp.lesp_Estado WHEN 'Esperando' THEN 0 ELSE 1 END,
        CASE lesp.lesp_Prioridad WHEN 'Urgente' THEN 0 WHEN 'Alta' THEN 1 ELSE 2 END,
        lesp.lesp_FechaCrea ASC
END
GO

-- =====================================================
-- SP: PR_Medico_ListaEspera_Find (pre-llenar el form en edicion)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ListaEspera_Find]
    @lesp_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        lesp.lesp_Id, lesp.masc_Id, lesp.cli_Id, lesp.tipoCon_Id,
        lesp.lesp_FechaDeseada, lesp.lesp_Prioridad, lesp.lesp_Estado, lesp.lesp_Notas
    FROM [Medico].[tbListaEspera] AS lesp
    WHERE lesp.lesp_Id = @lesp_Id AND lesp.lesp_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_ListaEspera_Detail (vista detalle)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ListaEspera_Detail]
    @lesp_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        lesp.lesp_Id, lesp.masc_Id, masc.masc_Nombre AS Mascota,
        lesp.cli_Id,
        CASE WHEN lesp.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(cli.cli_Nombres + ' ' + ISNULL(cli.cli_Apellidos, '')))
             ELSE 'Interno (refugio)' END AS Responsable,
        CASE WHEN lesp.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(ISNULL(cli.cli_Telefono, '') +
                  CASE WHEN cli.cli_Correo IS NOT NULL THEN ' · ' + cli.cli_Correo ELSE '' END))
             ELSE NULL END AS ResponsableContacto,
        lesp.tipoCon_Id,
        ISNULL(tc.tipoCon_Descripcion, 'Cualquiera') AS TipoConsulta,
        lesp.lesp_FechaDeseada, lesp.lesp_Prioridad, lesp.lesp_Estado, lesp.lesp_Notas,
        DATEDIFF(DAY, lesp.lesp_FechaCrea, GETDATE()) AS DiasEsperando,
        usuCrea.usu_Nombre AS UsuarioCreacion, lesp.lesp_FechaCrea,
        usuMod.usu_Nombre  AS UsuarioModificacion, lesp.lesp_FechaModifica
    FROM [Medico].[tbListaEspera] AS lesp
    INNER JOIN [Refugio].[tbMascotas]     AS masc    ON lesp.masc_Id = masc.masc_Id
    LEFT  JOIN [Veterinaria].[tbClientes]  AS cli     ON lesp.cli_Id = cli.cli_Id
    LEFT  JOIN [Medico].[tbTiposConsulta]  AS tc      ON lesp.tipoCon_Id = tc.tipoCon_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]   AS usuCrea ON lesp.lesp_UsuarioCrea = usuCrea.usu_Id
    LEFT  JOIN [Seguridad].[tbUsuarios]   AS usuMod  ON lesp.lesp_UsuarioModifica = usuMod.usu_Id
    WHERE lesp.lesp_Id = @lesp_Id AND lesp.lesp_EsEliminado = 0
END
GO

-- =====================================================
-- SP: PR_Medico_ListaEspera_Insert (retorna id + CodeStatus)
--   El cli_Id (dueño) se resuelve de la mascota si no se envia.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ListaEspera_Insert]
    @masc_Id          INT,
    @cli_Id           INT = NULL,
    @tipoCon_Id       INT = NULL,
    @lesp_FechaDeseada DATE = NULL,
    @lesp_Prioridad   NVARCHAR(20) = 'Normal',
    @lesp_Notas       NVARCHAR(1000) = NULL,
    @lesp_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF @masc_Id IS NULL OR NOT EXISTS (SELECT 1 FROM [Refugio].[tbMascotas] WHERE masc_Id = @masc_Id)
        BEGIN
            SELECT -1 AS CodeStatus, 'La mascota indicada no existe.' AS MessageStatus, 0 AS lesp_Id
            RETURN
        END

        SET @lesp_Prioridad = ISNULL(NULLIF(LTRIM(RTRIM(@lesp_Prioridad)), ''), 'Normal')
        IF @lesp_Prioridad NOT IN ('Normal','Alta','Urgente')
        BEGIN
            SELECT -2 AS CodeStatus, 'Prioridad no valida.' AS MessageStatus, 0 AS lesp_Id
            RETURN
        END

        IF @tipoCon_Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposConsulta] WHERE tipoCon_Id = @tipoCon_Id)
        BEGIN
            SELECT -3 AS CodeStatus, 'El tipo de consulta indicado no existe.' AS MessageStatus, 0 AS lesp_Id
            RETURN
        END

        -- Evitar duplicado activo: misma mascota ya esperando
        IF EXISTS (SELECT 1 FROM [Medico].[tbListaEspera]
                   WHERE masc_Id = @masc_Id AND lesp_Estado = 'Esperando' AND lesp_EsEliminado = 0)
        BEGIN
            SELECT -4 AS CodeStatus, 'Esta mascota ya esta en la lista de espera.' AS MessageStatus, 0 AS lesp_Id
            RETURN
        END

        -- Resolver el dueño desde la mascota si no vino explicito
        DECLARE @cliResolved INT = ISNULL(@cli_Id, (SELECT cli_Id FROM [Refugio].[tbMascotas] WHERE masc_Id = @masc_Id))

        INSERT INTO [Medico].[tbListaEspera]
            (masc_Id, cli_Id, tipoCon_Id, lesp_FechaDeseada, lesp_Prioridad, lesp_Estado,
             lesp_Notas, lesp_UsuarioCrea, lesp_FechaCrea)
        VALUES
            (@masc_Id, @cliResolved, @tipoCon_Id, @lesp_FechaDeseada, @lesp_Prioridad, 'Esperando',
             @lesp_Notas, @lesp_UsuarioCrea, GETDATE())

        SELECT 1 AS CodeStatus, 'Anotado en la lista de espera.' AS MessageStatus,
               CAST(SCOPE_IDENTITY() AS INT) AS lesp_Id
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus, 0 AS lesp_Id
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_ListaEspera_Update
--   Edita tipo/fecha deseada/prioridad/notas SOLO mientras sigue 'Esperando'
--   (una vez agendada o cancelada, el registro es historico/inmutable).
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ListaEspera_Update]
    @lesp_Id             INT,
    @tipoCon_Id          INT = NULL,
    @lesp_FechaDeseada   DATE = NULL,
    @lesp_Prioridad      NVARCHAR(20) = 'Normal',
    @lesp_Notas          NVARCHAR(1000) = NULL,
    @lesp_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @estadoActual NVARCHAR(20) = (SELECT lesp_Estado FROM [Medico].[tbListaEspera]
                                              WHERE lesp_Id = @lesp_Id AND lesp_EsEliminado = 0)
        IF @estadoActual IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, 'El registro de lista de espera no existe.' AS MessageStatus
            RETURN
        END
        IF @estadoActual <> 'Esperando'
        BEGIN
            SELECT -2 AS CodeStatus, 'Solo se puede editar mientras sigue esperando.' AS MessageStatus
            RETURN
        END

        SET @lesp_Prioridad = ISNULL(NULLIF(LTRIM(RTRIM(@lesp_Prioridad)), ''), 'Normal')
        IF @lesp_Prioridad NOT IN ('Normal','Alta','Urgente')
        BEGIN
            SELECT -3 AS CodeStatus, 'Prioridad no valida.' AS MessageStatus
            RETURN
        END

        UPDATE [Medico].[tbListaEspera]
        SET tipoCon_Id        = @tipoCon_Id,
            lesp_FechaDeseada = @lesp_FechaDeseada,
            lesp_Prioridad    = @lesp_Prioridad,
            lesp_Notas        = @lesp_Notas,
            lesp_UsuarioModifica = @lesp_UsuarioModifica,
            lesp_FechaModifica = GETDATE()
        WHERE lesp_Id = @lesp_Id AND lesp_EsEliminado = 0

        SELECT 1 AS CodeStatus, 'Lista de espera actualizada.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_ListaEspera_CambiarEstado
--   Esperando -> Agendada (ya se le dio cupo) o -> Cancelada (ya no quiere).
--   Estados terminales: no se revierten.
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ListaEspera_CambiarEstado]
    @lesp_Id INT,
    @estado  NVARCHAR(20),
    @usuario INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF @estado NOT IN ('Agendada','Cancelada')
        BEGIN
            SELECT -1 AS CodeStatus, 'Estado destino no valido.' AS MessageStatus
            RETURN
        END

        DECLARE @estadoActual NVARCHAR(20) = (SELECT lesp_Estado FROM [Medico].[tbListaEspera]
                                              WHERE lesp_Id = @lesp_Id AND lesp_EsEliminado = 0)
        IF @estadoActual IS NULL
        BEGIN
            SELECT -2 AS CodeStatus, 'El registro de lista de espera no existe.' AS MessageStatus
            RETURN
        END
        IF @estadoActual <> 'Esperando'
        BEGIN
            SELECT -3 AS CodeStatus, 'Solo un registro que sigue esperando puede agendarse o cancelarse.' AS MessageStatus
            RETURN
        END

        UPDATE [Medico].[tbListaEspera]
        SET lesp_Estado = @estado,
            lesp_UsuarioModifica = @usuario,
            lesp_FechaModifica = GETDATE()
        WHERE lesp_Id = @lesp_Id AND lesp_EsEliminado = 0

        SELECT 1 AS CodeStatus,
               CASE WHEN @estado = 'Agendada' THEN 'Marcado como agendado.' ELSE 'Salio de la lista de espera.' END AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =====================================================
-- SP: PR_Medico_ListaEspera_SugerenciasParaHueco
--   Cuando se libera/cancela una cita, ¿a quien llamar?
--   Devuelve los 'Esperando' que encajan con el hueco (fecha + tipo),
--   FIFO el mas antiguo primero, respetando prioridad.
--   Reglas de encaje (analogo a matchWaitlist de OpenVPM):
--     - tipo: el registro no pide tipo (NULL) o coincide con el del hueco
--             (o el hueco no especifica tipo).
--     - fecha: el registro no pide fecha (NULL) o el hueco cae en/antes de
--             su fecha deseada (sirve para su "lo quiero para tal fecha").
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ListaEspera_SugerenciasParaHueco]
    @fecha      DATE = NULL,
    @tipoCon_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(
            ORDER BY
                CASE lesp.lesp_Prioridad WHEN 'Urgente' THEN 0 WHEN 'Alta' THEN 1 ELSE 2 END,
                lesp.lesp_FechaCrea ASC
        ) AS Fila,
        lesp.lesp_Id,
        lesp.masc_Id,
        masc.masc_Nombre            AS Mascota,
        lesp.cli_Id,
        CASE WHEN lesp.cli_Id IS NOT NULL
             THEN LTRIM(RTRIM(cli.cli_Nombres + ' ' + ISNULL(cli.cli_Apellidos, '')))
             ELSE 'Interno (refugio)' END AS Responsable,
        CASE WHEN lesp.cli_Id IS NOT NULL THEN cli.cli_Telefono ELSE NULL END AS Telefono,
        lesp.tipoCon_Id,
        ISNULL(tc.tipoCon_Descripcion, 'Cualquiera') AS TipoConsulta,
        lesp.lesp_FechaDeseada,
        lesp.lesp_Prioridad,
        lesp.lesp_FechaCrea,
        DATEDIFF(DAY, lesp.lesp_FechaCrea, GETDATE()) AS DiasEsperando
    FROM [Medico].[tbListaEspera] AS lesp
    INNER JOIN [Refugio].[tbMascotas]      AS masc ON lesp.masc_Id = masc.masc_Id
    LEFT  JOIN [Veterinaria].[tbClientes]   AS cli  ON lesp.cli_Id = cli.cli_Id
    LEFT  JOIN [Medico].[tbTiposConsulta]   AS tc   ON lesp.tipoCon_Id = tc.tipoCon_Id
    WHERE lesp.lesp_EsEliminado = 0
      AND lesp.lesp_Estado = 'Esperando'
      AND (@tipoCon_Id IS NULL OR lesp.tipoCon_Id IS NULL OR lesp.tipoCon_Id = @tipoCon_Id)
      AND (@fecha IS NULL OR lesp.lesp_FechaDeseada IS NULL OR @fecha <= lesp.lesp_FechaDeseada)
    ORDER BY
        CASE lesp.lesp_Prioridad WHEN 'Urgente' THEN 0 WHEN 'Alta' THEN 1 ELSE 2 END,
        lesp.lesp_FechaCrea ASC
END
GO

-- =====================================================
-- SP: PR_Medico_ListaEspera_Delete (soft delete / quitar de la lista)
-- =====================================================
CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ListaEspera_Delete]
    @lesp_Id INT,
    @usuario INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbListaEspera]
        SET lesp_EsEliminado = 1, lesp_UsuarioModifica = @usuario, lesp_FechaModifica = GETDATE()
        WHERE lesp_Id = @lesp_Id

        SELECT 1 AS CodeStatus, 'Registro eliminado de la lista de espera.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

PRINT 'Modulo LISTA DE ESPERA: tabla y SPs creados correctamente.'
GO
