-- Actualiza el SP de lista de mascotas para incluir campos extra
-- necesarios para la landing page pública.
-- Ejecutar en: PETSHOMEDB

CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Mascotas_List]
AS
BEGIN
    SELECT      mascotas.masc_Id,
                ROW_NUMBER() OVER(ORDER BY ISNULL(mascotas.masc_FechaModifica, mascotas.masc_FechaCrea) DESC) AS masc_Fila,
                mascotas.masc_Imagen,
                mascotas.masc_Nombre,
                raza.raza_Descripcion,
                albergue.refg_Nombre,
                mascotas.masc_Edad,
                mascotas.masc_Sexo,
                ISNULL(talla.tall_Descripcion, '') AS tall_Descripcion,
                ISNULL(mascotas.masc_Color, '')    AS masc_Color,
                ISNULL(mascotas.masc_Historia, '') AS masc_Historia,
                ISNULL(mascotas.masc_EsAdoptado,  0) AS masc_EsAdoptado,
                ISNULL(mascotas.masc_EsReservado, 0) AS masc_EsReservado
    FROM        [Refugio].[tbMascotas] AS mascotas
    INNER JOIN  [Refugio].[tbRazas]        AS raza        ON mascotas.raza_Id  = raza.raza_Id
    INNER JOIN  [Refugio].[tbRefugios]     AS albergue    ON mascotas.refg_Id  = albergue.refg_Id
    INNER JOIN  [Refugio].[tbProcedencias] AS procedencia ON mascotas.proc_Id  = procedencia.proc_Id
    LEFT  JOIN  [Refugio].[tbTallas]       AS talla       ON mascotas.tall_Id  = talla.tall_Id
    WHERE       mascotas.masc_EsEliminado != 1
    ORDER BY    ISNULL(mascotas.masc_FechaModifica, mascotas.masc_FechaCrea) DESC
END
GO
