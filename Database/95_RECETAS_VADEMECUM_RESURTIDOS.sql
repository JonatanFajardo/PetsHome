/*
=============================================
SCRIPT: 95 - Vademecum (catalogo de farmacos de referencia para la
              calculadora de dosis por peso) + Resurtidos de recetas
AUTOR: Sistema PetsHome
FECHA: 2026-10-30
Idempotente. Replica el formulario de dosing de OpenVPM
(apps/web/lib/dosing/formulary.ts) adaptado a espanol SIN acentos.
=============================================
*/
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

USE PETSHOMEDB
GO

/* =========================================================
   PARTE A.1 - Tabla Medico.tbVademecum
   ========================================================= */
IF OBJECT_ID('Medico.tbVademecum', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbVademecum]
    (
        vade_Id                     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tbVademecum PRIMARY KEY,
        vade_Farmaco                NVARCHAR(120) NOT NULL,
        vade_Categoria              NVARCHAR(60)  NOT NULL,
        vade_PresentacionesTabletaMg NVARCHAR(100) NULL,  -- CSV mg, ej '25,75,100'
        vade_ConcentracionesMgMl    NVARCHAR(100) NULL,    -- CSV mg/mL, ej '10'
        vade_Advertencias           NVARCHAR(500) NULL,
        vade_EsActivo               BIT NOT NULL CONSTRAINT DF_tbVademecum_EsActivo DEFAULT (1),
        vade_EsEliminado            BIT NOT NULL CONSTRAINT DF_tbVademecum_EsEliminado DEFAULT (0),
        vade_UsuarioCrea            INT NOT NULL,
        vade_FechaCrea              DATETIME NOT NULL CONSTRAINT DF_tbVademecum_FechaCrea DEFAULT (GETDATE()),
        vade_UsuarioModifica        INT NULL,
        vade_FechaModifica          DATETIME NULL
    )
END
GO

/* =========================================================
   PARTE A.2 - Tabla Medico.tbVademecumDosis
   ========================================================= */
IF OBJECT_ID('Medico.tbVademecumDosis', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbVademecumDosis]
    (
        vdos_Id             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tbVademecumDosis PRIMARY KEY,
        vade_Id             INT NOT NULL CONSTRAINT FK_tbVademecumDosis_Vademecum
                                FOREIGN KEY REFERENCES [Medico].[tbVademecum](vade_Id),
        vdos_Especie        NVARCHAR(20) NOT NULL,   -- 'Canino' | 'Felino'
        vdos_MgKgMin        DECIMAL(10,3) NOT NULL,
        vdos_MgKgMax        DECIMAL(10,3) NOT NULL,
        vdos_Via            NVARCHAR(20) NOT NULL,
        vdos_Frecuencia     NVARCHAR(30) NOT NULL,
        vdos_MaxDosisUnicaMg DECIMAL(10,2) NULL,
        vdos_Notas          NVARCHAR(300) NULL,
        vdos_EsEliminado    BIT NOT NULL CONSTRAINT DF_tbVademecumDosis_EsEliminado DEFAULT (0),
        vdos_UsuarioCrea    INT NOT NULL,
        vdos_FechaCrea      DATETIME NOT NULL CONSTRAINT DF_tbVademecumDosis_FechaCrea DEFAULT (GETDATE()),
        vdos_UsuarioModifica INT NULL,
        vdos_FechaModifica  DATETIME NULL
    )
END
GO

/* =========================================================
   PARTE A.3 - Siembra idempotente del formulario (OpenVPM)
   ========================================================= */
DECLARE @id INT;

-- Maropitant (Cerenia)
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecum WHERE vade_Farmaco = N'Maropitant (Cerenia)')
    INSERT INTO Medico.tbVademecum (vade_Farmaco, vade_Categoria, vade_PresentacionesTabletaMg, vade_ConcentracionesMgMl, vade_Advertencias, vade_UsuarioCrea)
    VALUES (N'Maropitant (Cerenia)', N'Antiemetico', N'16,24,60,160', N'10', NULL, 1);
SET @id = (SELECT vade_Id FROM Medico.tbVademecum WHERE vade_Farmaco = N'Maropitant (Cerenia)');
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Canino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Canino', 1, 2, N'PO', N'q24h', NULL, N'1 mg/kg SC o 2 mg/kg PO', 1);
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Felino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Felino', 1, 1, N'SC', N'q24h', NULL, NULL, 1);

-- Carprofeno (Rimadyl)  (solo canino; no extrapolar a gatos)
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecum WHERE vade_Farmaco = N'Carprofeno (Rimadyl)')
    INSERT INTO Medico.tbVademecum (vade_Farmaco, vade_Categoria, vade_PresentacionesTabletaMg, vade_ConcentracionesMgMl, vade_Advertencias, vade_UsuarioCrea)
    VALUES (N'Carprofeno (Rimadyl)', N'AINE', N'25,75,100', NULL, N'No recomendado en gatos. Usar con precaucion en pacientes con enfermedad renal, hepatica o gastrointestinal.', 1);
SET @id = (SELECT vade_Id FROM Medico.tbVademecum WHERE vade_Farmaco = N'Carprofeno (Rimadyl)');
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Canino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Canino', 2.2, 4.4, N'PO', N'q12-24h', NULL, N'4.4 mg/kg/dia total, dividido q12h o una vez al dia', 1);

-- Meloxicam (Metacam)
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecum WHERE vade_Farmaco = N'Meloxicam (Metacam)')
    INSERT INTO Medico.tbVademecum (vade_Farmaco, vade_Categoria, vade_PresentacionesTabletaMg, vade_ConcentracionesMgMl, vade_Advertencias, vade_UsuarioCrea)
    VALUES (N'Meloxicam (Metacam)', N'AINE', NULL, N'1.5,5', N'El uso repetido de AINE en gatos conlleva riesgo renal significativo. Confirmar hidratacion y valores renales.', 1);
SET @id = (SELECT vade_Id FROM Medico.tbVademecum WHERE vade_Farmaco = N'Meloxicam (Metacam)');
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Canino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Canino', 0.1, 0.2, N'PO', N'q24h', NULL, N'0.2 mg/kg dosis de carga dia 1, luego 0.1 mg/kg', 1);
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Felino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Felino', 0.05, 0.1, N'PO', N'q24h', 0.3, N'Uso de dosis unica; la dosificacion repetida en gatos es de alto riesgo', 1);

-- Gabapentina
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecum WHERE vade_Farmaco = N'Gabapentina')
    INSERT INTO Medico.tbVademecum (vade_Farmaco, vade_Categoria, vade_PresentacionesTabletaMg, vade_ConcentracionesMgMl, vade_Advertencias, vade_UsuarioCrea)
    VALUES (N'Gabapentina', N'Analgesico / Ansiolitico', N'100,300,400', N'50', NULL, 1);
SET @id = (SELECT vade_Id FROM Medico.tbVademecum WHERE vade_Farmaco = N'Gabapentina');
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Canino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Canino', 5, 10, N'PO', N'q8-12h', NULL, NULL, 1);
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Felino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Felino', 5, 10, N'PO', N'q12h', NULL, N'Dosis unicas mas altas (ej. 50-100 mg/gato) usadas situacionalmente para ansiedad por transporte', 1);

-- Metronidazol
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecum WHERE vade_Farmaco = N'Metronidazol')
    INSERT INTO Medico.tbVademecum (vade_Farmaco, vade_Categoria, vade_PresentacionesTabletaMg, vade_ConcentracionesMgMl, vade_Advertencias, vade_UsuarioCrea)
    VALUES (N'Metronidazol', N'Antibiotico / Antiprotozoario', N'250,500', NULL, N'Dosis altas acumuladas tienen riesgo de neurotoxicidad. Evitar tratamientos prolongados a dosis altas.', 1);
SET @id = (SELECT vade_Id FROM Medico.tbVademecum WHERE vade_Farmaco = N'Metronidazol');
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Canino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Canino', 10, 15, N'PO', N'q12h', NULL, NULL, 1);
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Felino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Felino', 10, 15, N'PO', N'q12h', NULL, NULL, 1);

-- Amoxicilina / Clavulanato (Clavamox)
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecum WHERE vade_Farmaco = N'Amoxicilina / Clavulanato (Clavamox)')
    INSERT INTO Medico.tbVademecum (vade_Farmaco, vade_Categoria, vade_PresentacionesTabletaMg, vade_ConcentracionesMgMl, vade_Advertencias, vade_UsuarioCrea)
    VALUES (N'Amoxicilina / Clavulanato (Clavamox)', N'Antibiotico', N'62.5,125,250,375', NULL, NULL, 1);
SET @id = (SELECT vade_Id FROM Medico.tbVademecum WHERE vade_Farmaco = N'Amoxicilina / Clavulanato (Clavamox)');
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Canino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Canino', 12.5, 25, N'PO', N'q12h', NULL, NULL, 1);
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Felino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Felino', 12.5, 25, N'PO', N'q12h', NULL, NULL, 1);

-- Famotidina
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecum WHERE vade_Farmaco = N'Famotidina')
    INSERT INTO Medico.tbVademecum (vade_Farmaco, vade_Categoria, vade_PresentacionesTabletaMg, vade_ConcentracionesMgMl, vade_Advertencias, vade_UsuarioCrea)
    VALUES (N'Famotidina', N'Antagonista H2', N'10,20,40', NULL, NULL, 1);
SET @id = (SELECT vade_Id FROM Medico.tbVademecum WHERE vade_Farmaco = N'Famotidina');
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Canino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Canino', 0.5, 1, N'PO', N'q12-24h', NULL, NULL, 1);
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Felino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Felino', 0.5, 1, N'PO', N'q12-24h', NULL, NULL, 1);

-- Oclacitinib (Apoquel)  (solo canino)
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecum WHERE vade_Farmaco = N'Oclacitinib (Apoquel)')
    INSERT INTO Medico.tbVademecum (vade_Farmaco, vade_Categoria, vade_PresentacionesTabletaMg, vade_ConcentracionesMgMl, vade_Advertencias, vade_UsuarioCrea)
    VALUES (N'Oclacitinib (Apoquel)', N'Antipruriginoso', N'3.6,5.4,16', NULL, N'Indicado para perros de 12 meses o mas. No para gatos.', 1);
SET @id = (SELECT vade_Id FROM Medico.tbVademecum WHERE vade_Farmaco = N'Oclacitinib (Apoquel)');
IF NOT EXISTS (SELECT 1 FROM Medico.tbVademecumDosis WHERE vade_Id = @id AND vdos_Especie = N'Canino')
    INSERT INTO Medico.tbVademecumDosis (vade_Id, vdos_Especie, vdos_MgKgMin, vdos_MgKgMax, vdos_Via, vdos_Frecuencia, vdos_MaxDosisUnicaMg, vdos_Notas, vdos_UsuarioCrea)
    VALUES (@id, N'Canino', 0.4, 0.6, N'PO', N'q12h', NULL, N'q12h hasta 14 dias, luego q24h mantenimiento', 1);
GO

/* =========================================================
   PARTE A.4 - SP lista plana del vademecum
   ========================================================= */
IF OBJECT_ID('Medico.PR_Medico_Vademecum_List', 'P') IS NOT NULL
    DROP PROCEDURE [Medico].[PR_Medico_Vademecum_List]
GO
CREATE PROCEDURE [Medico].[PR_Medico_Vademecum_List]
    @especie   NVARCHAR(20) = NULL,
    @categoria NVARCHAR(60) = NULL
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        v.vade_Id,
        v.vade_Farmaco,
        v.vade_Categoria,
        d.vdos_Especie,
        d.vdos_MgKgMin,
        d.vdos_MgKgMax,
        d.vdos_Via,
        d.vdos_Frecuencia,
        d.vdos_MaxDosisUnicaMg,
        v.vade_PresentacionesTabletaMg,
        v.vade_ConcentracionesMgMl,
        v.vade_Advertencias,
        d.vdos_Notas
    FROM [Medico].[tbVademecum] AS v
    INNER JOIN [Medico].[tbVademecumDosis] AS d
        ON d.vade_Id = v.vade_Id AND d.vdos_EsEliminado = 0
    WHERE v.vade_EsEliminado = 0
      AND v.vade_EsActivo = 1
      AND (@especie   IS NULL OR @especie   = N'' OR d.vdos_Especie = @especie)
      AND (@categoria IS NULL OR @categoria = N'' OR v.vade_Categoria = @categoria)
    ORDER BY v.vade_Farmaco, d.vdos_Especie
END
GO

/* =========================================================
   PARTE B.5 - Columnas de resurtido en tbRecetas
   ========================================================= */
IF COL_LENGTH('Medico.tbRecetas', 'receta_ResurtidosAutorizados') IS NULL
    ALTER TABLE [Medico].[tbRecetas]
        ADD receta_ResurtidosAutorizados INT NOT NULL CONSTRAINT DF_tbRecetas_ResurtidosAutorizados DEFAULT (0)
GO
IF COL_LENGTH('Medico.tbRecetas', 'receta_ResurtidosUsados') IS NULL
    ALTER TABLE [Medico].[tbRecetas]
        ADD receta_ResurtidosUsados INT NOT NULL CONSTRAINT DF_tbRecetas_ResurtidosUsados DEFAULT (0)
GO

/* =========================================================
   PARTE B.6 - Bitacora de resurtidos
   ========================================================= */
IF OBJECT_ID('Medico.tbRecetaResurtido', 'U') IS NULL
BEGIN
    CREATE TABLE [Medico].[tbRecetaResurtido]
    (
        rsrt_Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tbRecetaResurtido PRIMARY KEY,
        receta_Id        INT NOT NULL CONSTRAINT FK_tbRecetaResurtido_Receta
                             FOREIGN KEY REFERENCES [Medico].[tbRecetas](receta_Id),
        rsrt_Fecha       DATETIME NOT NULL CONSTRAINT DF_tbRecetaResurtido_Fecha DEFAULT (GETDATE()),
        rsrt_Usuario     INT NOT NULL,
        rsrt_EsEliminado BIT NOT NULL CONSTRAINT DF_tbRecetaResurtido_EsEliminado DEFAULT (0)
    )
END
GO

/* =========================================================
   PARTE B.7 - SP registrar resurtido
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
    BEGIN TRY
        DECLARE @auth INT, @usad INT;

        SELECT @auth = receta_ResurtidosAutorizados,
               @usad = receta_ResurtidosUsados
        FROM [Medico].[tbRecetas]
        WHERE receta_Id = @receta_Id AND receta_EsEliminado = 0;

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

        BEGIN TRANSACTION
            UPDATE [Medico].[tbRecetas]
            SET receta_ResurtidosUsados = receta_ResurtidosUsados + 1
            WHERE receta_Id = @receta_Id;

            INSERT INTO [Medico].[tbRecetaResurtido] (receta_Id, rsrt_Usuario)
            VALUES (@receta_Id, @usuario);
        COMMIT TRANSACTION

        SELECT 1 AS CodeStatus,
               N'Resurtido registrado.' AS MessageStatus,
               (@usad + 1) AS receta_ResurtidosUsados,
               @auth AS receta_ResurtidosAutorizados
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus,
               0 AS receta_ResurtidosUsados, 0 AS receta_ResurtidosAutorizados
    END CATCH
END
GO

/* =========================================================
   PARTE B.8 - Recrear SP List / Detail / Find / Insert / Update
   para exponer resurtidos (y especie/peso en Find)
   ========================================================= */

-- List: agrega columnas de resurtido
IF OBJECT_ID('Medico.PR_Medico_Recetas_List', 'P') IS NOT NULL
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_List]
GO
CREATE PROCEDURE [Medico].[PR_Medico_Recetas_List]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ROW_NUMBER() OVER(ORDER BY rec.receta_FechaInicio DESC) AS Fila,
        rec.receta_Id,
        rec.cita_Id,
        rec.masc_Id,
        masc.masc_Nombre AS Mascota,
        rec.receta_Medicamento,
        rec.tipoMed_Id,
        tipoMed.tipoMed_Descripcion AS TipoMedicamento,
        rec.viaAdmin_Id,
        viaAdmin.viaAdmin_Descripcion AS ViaAdministracion,
        rec.receta_Dosis,
        rec.receta_Frecuencia,
        rec.receta_Duracion,
        rec.receta_FechaInicio,
        rec.receta_FechaFin,
        rec.receta_Estado,
        rec.receta_ResurtidosAutorizados,
        rec.receta_ResurtidosUsados
    FROM [Medico].[tbRecetas] AS rec
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON rec.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposMedicamento] AS tipoMed
        ON rec.tipoMed_Id = tipoMed.tipoMed_Id
    LEFT JOIN [Medico].[tbViasAdministracion] AS viaAdmin
        ON rec.viaAdmin_Id = viaAdmin.viaAdmin_Id
    WHERE rec.receta_EsEliminado = 0
    ORDER BY rec.receta_FechaInicio DESC
END
GO

-- Detail: agrega columnas de resurtido
IF OBJECT_ID('Medico.PR_Medico_Recetas_Detail', 'P') IS NOT NULL
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Detail]
GO
CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Detail]
    @receta_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        rec.receta_Id,
        rec.cita_Id,
        rec.masc_Id,
        masc.masc_Nombre AS Mascota,
        rec.receta_Medicamento,
        rec.tipoMed_Id,
        tipoMed.tipoMed_Descripcion AS TipoMedicamento,
        rec.viaAdmin_Id,
        viaAdmin.viaAdmin_Descripcion AS ViaAdministracion,
        rec.receta_Dosis,
        rec.receta_Frecuencia,
        rec.receta_Duracion,
        rec.receta_Instrucciones,
        rec.receta_FechaInicio,
        rec.receta_FechaFin,
        rec.receta_Estado,
        rec.receta_ResurtidosAutorizados,
        rec.receta_ResurtidosUsados,
        usuarioCrea.usu_Nombre AS UsuarioCreacion,
        rec.receta_FechaCrea,
        usuarioModifica.usu_Nombre AS UsuarioModificacion,
        rec.receta_FechaModifica
    FROM [Medico].[tbRecetas] AS rec
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON rec.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposMedicamento] AS tipoMed
        ON rec.tipoMed_Id = tipoMed.tipoMed_Id
    LEFT JOIN [Medico].[tbViasAdministracion] AS viaAdmin
        ON rec.viaAdmin_Id = viaAdmin.viaAdmin_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON rec.receta_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON rec.receta_UsuarioModifica = usuarioModifica.usu_Id
    WHERE rec.receta_Id = @receta_Id
      AND rec.receta_EsEliminado = 0
END
GO

-- Find: agrega resurtidos + especie (raza_TipoAnimal) y peso de la mascota
IF OBJECT_ID('Medico.PR_Medico_Recetas_Find', 'P') IS NOT NULL
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Find]
GO
CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Find]
    @receta_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        rec.receta_Id,
        rec.cita_Id,
        rec.masc_Id,
        masc.masc_Nombre,
        masc.masc_Peso,
        raza.raza_TipoAnimal,
        rec.receta_Medicamento,
        rec.tipoMed_Id,
        rec.viaAdmin_Id,
        rec.receta_Dosis,
        rec.receta_Frecuencia,
        rec.receta_Duracion,
        rec.receta_Instrucciones,
        rec.receta_FechaInicio,
        rec.receta_FechaFin,
        rec.receta_Estado,
        rec.receta_ResurtidosAutorizados,
        rec.receta_ResurtidosUsados,
        rec.receta_UsuarioCrea,
        usuarioCrea.usu_Nombre AS usuarioCrea,
        rec.receta_FechaCrea,
        rec.receta_UsuarioModifica,
        usuarioModifica.usu_Nombre AS usuarioModifica,
        rec.receta_FechaModifica
    FROM [Medico].[tbRecetas] AS rec
    LEFT JOIN [Refugio].[tbMascotas] AS masc
        ON rec.masc_Id = masc.masc_Id
    LEFT JOIN [Refugio].[tbRazas] AS raza
        ON masc.raza_Id = raza.raza_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON rec.receta_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON rec.receta_UsuarioModifica = usuarioModifica.usu_Id
    WHERE rec.receta_Id = @receta_Id
      AND rec.receta_EsEliminado = 0
END
GO

-- Insert: agrega parametro @receta_ResurtidosAutorizados
IF OBJECT_ID('Medico.PR_Medico_Recetas_Insert', 'P') IS NOT NULL
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Insert]
GO
CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Insert]
    @cita_Id INT,
    @masc_Id INT,
    @receta_Medicamento NVARCHAR(200),
    @tipoMed_Id INT = NULL,
    @viaAdmin_Id INT = NULL,
    @receta_Dosis NVARCHAR(100) = NULL,
    @receta_Frecuencia NVARCHAR(100) = NULL,
    @receta_Duracion NVARCHAR(50) = NULL,
    @receta_Instrucciones NVARCHAR(500) = NULL,
    @receta_FechaInicio DATE = NULL,
    @receta_FechaFin DATE = NULL,
    @receta_Estado NVARCHAR(20) = 'Activa',
    @itm_Id INT = NULL,
    @receta_ResurtidosAutorizados INT = 0,
    @receta_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [Medico].[tbRecetas]
    (
        cita_Id, masc_Id, receta_Medicamento, tipoMed_Id, viaAdmin_Id,
        receta_Dosis, receta_Frecuencia, receta_Duracion, receta_Instrucciones,
        receta_FechaInicio, receta_FechaFin, receta_Estado, itm_Id,
        receta_ResurtidosAutorizados, receta_UsuarioCrea, receta_FechaCrea
    )
    VALUES
    (
        @cita_Id, @masc_Id, @receta_Medicamento, @tipoMed_Id, @viaAdmin_Id,
        @receta_Dosis, @receta_Frecuencia, @receta_Duracion, @receta_Instrucciones,
        @receta_FechaInicio, @receta_FechaFin, @receta_Estado, @itm_Id,
        ISNULL(@receta_ResurtidosAutorizados, 0), @receta_UsuarioCrea, GETDATE()
    )

    SELECT SCOPE_IDENTITY() AS receta_Id
END
GO

-- Update: agrega parametro @receta_ResurtidosAutorizados (conserva patron transaccional)
IF OBJECT_ID('Medico.PR_Medico_Recetas_Update', 'P') IS NOT NULL
    DROP PROCEDURE [Medico].[PR_Medico_Recetas_Update]
GO
CREATE PROCEDURE [Medico].[PR_Medico_Recetas_Update]
    @receta_Id              INT,
    @cita_Id                INT,
    @masc_Id                INT,
    @receta_Medicamento     NVARCHAR(200),
    @tipoMed_Id             INT           = NULL,
    @viaAdmin_Id            INT           = NULL,
    @receta_Dosis           NVARCHAR(100) = NULL,
    @receta_Frecuencia      NVARCHAR(100) = NULL,
    @receta_Duracion        NVARCHAR(100) = NULL,
    @receta_Instrucciones   NVARCHAR(500) = NULL,
    @receta_FechaInicio     DATE          = NULL,
    @receta_FechaFin        DATE          = NULL,
    @receta_Estado          NVARCHAR(50)  = NULL,
    @receta_ResurtidosAutorizados INT     = NULL,
    @receta_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Medico.tbRecetas WHERE receta_Id = @receta_Id AND receta_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'La receta no fue encontrada.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE Medico.tbRecetas
            SET cita_Id                = @cita_Id,
                masc_Id                = @masc_Id,
                receta_Medicamento     = @receta_Medicamento,
                tipoMed_Id             = @tipoMed_Id,
                viaAdmin_Id            = @viaAdmin_Id,
                receta_Dosis           = @receta_Dosis,
                receta_Frecuencia      = @receta_Frecuencia,
                receta_Duracion        = @receta_Duracion,
                receta_Instrucciones   = @receta_Instrucciones,
                receta_FechaInicio     = @receta_FechaInicio,
                receta_FechaFin        = @receta_FechaFin,
                receta_Estado          = @receta_Estado,
                receta_ResurtidosAutorizados = ISNULL(@receta_ResurtidosAutorizados, receta_ResurtidosAutorizados),
                receta_UsuarioModifica = @receta_UsuarioModifica,
                receta_FechaModifica   = GETDATE()
            WHERE receta_Id = @receta_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Receta actualizada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

PRINT '=========================================='
PRINT 'Script 95: Vademecum + Resurtidos aplicado.'
PRINT '=========================================='
GO
