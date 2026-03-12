-- ============================================================
-- Update_All_List_OrderBy.sql
-- Fecha: 2026-02-22
--
-- Aplica el patrón ORDER BY ISNULL(FechaModifica, FechaCrea) DESC
-- a todos los procedimientos _List del sistema PetsHome.
--
-- INSTRUCCIONES: Ejecutar este script en PETSHOMEDB
-- ============================================================

CREATE OR ALTER PROCEDURE [General].[PR_General_Departamentos_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(depto_FechaModifica, depto_FechaCrea) DESC) AS Fila,
            depto_Id,
            depto_Codigo,
            depto_Descripcion,
            [depto_Capital],
            [depto_Poblacion],
            [depto_AreaKm2]
    FROM    [General].[tbDepartamentos]
    WHERE   depto_EsEliminado != 1
    ORDER BY ISNULL(depto_FechaModifica, depto_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [General].[PR_General_Municipios_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(mpio_FechaModifica, mpio_FechaCrea) DESC) AS Fila,
            [mpio_Id],
            [mpio_Codigo],
            [mpio_Descripcion]
    FROM    [General].[tbMunicipios]
    WHERE   [mpio_EsEliminado] = 0
    ORDER BY ISNULL(mpio_FechaModifica, mpio_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Categorias_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(cat_FechaModifica, cat_FechaCrea) DESC) AS Fila,
            cat_Id,
            cat_Descripcion,
            CASE WHEN cat_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS cat_EsActivo
    FROM [Inventario].[tbCategorias]
    WHERE cat_EsEliminado != 1
    ORDER BY ISNULL(cat_FechaModifica, cat_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Entradas_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(entradas.ent_FechaModifica, entradas.ent_FechaCrea) DESC) AS Fila,
            entradas.ent_Id,
            ent_Descripcion,
            refugios.refg_Nombre,
            ent_Fecha
    FROM    [Inventario].[tbEntradas] AS entradas
    INNER JOIN [Refugio].[tbRefugios] AS refugios
    ON      entradas.refg_Id = refugios.refg_Id
    WHERE   ent_EsEliminado != 1
    ORDER BY ISNULL(entradas.ent_FechaModifica, entradas.ent_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_EntradasDetalles_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(entradasDetalles.entdet_FechaModifica, entradasDetalles.entdet_FechaCrea) DESC) AS Fila,
            entdet_Id,
            entradas.ent_Descripcion,
            items.itm_Descripcion,
            entdet_Cantidad
    FROM    [Inventario].[tbEntradasDetalles] AS entradasDetalles
    INNER JOIN [Inventario].[tbEntradas] AS entradas
    ON      entradasDetalles.ent_Id = entradas.ent_Id
    INNER JOIN [Inventario].[tbItems] AS items
    ON      entradasDetalles.itm_Id = items.itm_Id
    WHERE   entdet_EsEliminado != 1
    ORDER BY ISNULL(entradasDetalles.entdet_FechaModifica, entradasDetalles.entdet_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Items_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(items.itm_FechaModifica, items.itm_FechaCrea) DESC) AS Fila,
            itm_Id,
            itm_Codigo,
            itm_Descripcion,
            categorias.cat_Descripcion,
            itm_Precio
    FROM [Inventario].[tbItems] AS items
    INNER JOIN  [Inventario].[tbCategorias] AS categorias
    ON      items.cat_Id = categorias.cat_Id
    WHERE itm_EsEliminado != 1
    ORDER BY ISNULL(items.itm_FechaModifica, items.itm_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesDetalles_List]
    @recep_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ROW_NUMBER() OVER(ORDER BY ISNULL(rd.recdet_FechaModifica, rd.recdet_FechaCrea) DESC) AS Fila,
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
    ORDER BY ISNULL(rd.recdet_FechaModifica, rd.recdet_FechaCrea) DESC;
END
GO


CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesMercancia_List]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ROW_NUMBER() OVER(ORDER BY ISNULL(rm.recep_FechaModifica, rm.recep_FechaCrea) DESC) AS Fila,
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
    ORDER BY ISNULL(rm.recep_FechaModifica, rm.recep_FechaCrea) DESC;
END
GO


CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Salidas_List]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ROW_NUMBER() OVER(ORDER BY ISNULL(s.sal_FechaModifica, s.sal_FechaCrea) DESC) AS Fila,
        s.[sal_Id],
        s.[sal_Descripcion],
        s.[sal_TipoSalida],
        s.[refg_Id],
        r.[refg_Nombre],
        s.[sal_Fecha]
    FROM [Inventario].[tbSalidas] s
    INNER JOIN [Refugio].[tbRefugios] r ON s.refg_Id = r.refg_Id
    WHERE s.sal_EsEliminado = 0
    ORDER BY ISNULL(s.sal_FechaModifica, s.sal_FechaCrea) DESC;
END
GO


CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_CitaMedica_List]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ROW_NUMBER() OVER(ORDER BY ISNULL(cita.cita_FechaModifica, cita.cita_FechaCrea) DESC) AS Fila,
        cita.cita_Id,
        cita.masc_Id,
        masc.masc_Nombre,
        cita.cita_FechaConsulta,
        cita.tipoCon_Id,
        tipoCon.tipoCon_Descripcion AS TipoConsulta,
        cita.grav_Id,
        grav.grav_Descripcion AS Gravedad,
        cita.cita_MotivoConsulta,
        cita.cita_Diagnostico,
        cita.cita_Peso,
        cita.cita_Temperatura,
        cita.cita_FrecuenciaCardiaca,
        cita.cita_FrecuenciaRespiratoria,
        cita.cita_ProximaCita
    FROM [Medico].[tbCitaMedica] AS cita
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON cita.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposConsulta] AS tipoCon
        ON cita.tipoCon_Id = tipoCon.tipoCon_Id
    LEFT JOIN [Medico].[tbGravedades] AS grav
        ON cita.grav_Id = grav.grav_Id
    WHERE cita.cita_EsEliminado = 0
    ORDER BY ISNULL(cita.cita_FechaModifica, cita.cita_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Gravedades_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(grav_FechaModifica, grav_FechaCrea) DESC) AS Fila,
            grav_Id,
            grav_Descripcion,
            CASE WHEN grav_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS grav_EsActivo
    FROM [Medico].[tbGravedades]
    WHERE grav_EsEliminado != 1
    ORDER BY ISNULL(grav_FechaModifica, grav_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Recetas_List]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ROW_NUMBER() OVER(ORDER BY ISNULL(rec.receta_FechaModifica, rec.receta_FechaCrea) DESC) AS Fila,
        rec.receta_Id,
        rec.cita_Id,
        rec.masc_Id,
        masc.masc_Nombre,
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
        rec.receta_Estado
    FROM [Medico].[tbRecetas] AS rec
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON rec.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposMedicamento] AS tipoMed
        ON rec.tipoMed_Id = tipoMed.tipoMed_Id
    LEFT JOIN [Medico].[tbViasAdministracion] AS viaAdmin
        ON rec.viaAdmin_Id = viaAdmin.viaAdmin_Id
    WHERE rec.receta_EsEliminado = 0
    ORDER BY ISNULL(rec.receta_FechaModifica, rec.receta_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_TiposConsulta_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(tipoCon_FechaModifica, tipoCon_FechaCrea) DESC) AS Fila,
            tipoCon_Id,
            tipoCon_Descripcion,
            CASE WHEN tipoCon_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS tipoCon_EsActivo
    FROM [Medico].[tbTiposConsulta]
    WHERE tipoCon_EsEliminado != 1
    ORDER BY ISNULL(tipoCon_FechaModifica, tipoCon_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(tipoEst_FechaModifica, tipoEst_FechaCrea) DESC) AS Fila,
            tipoEst_Id,
            tipoEst_Descripcion,
            tipoEst_Sexo,
            CASE WHEN tipoEst_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS tipoEst_EsActivo
    FROM [Medico].[tbTiposEsterilizacion]
    WHERE tipoEst_EsEliminado != 1
    ORDER BY ISNULL(tipoEst_FechaModifica, tipoEst_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_TiposMedicamento_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(tipoMed_FechaModifica, tipoMed_FechaCrea) DESC) AS Fila,
            tipoMed_Id,
            tipoMed_Descripcion,
            CASE WHEN tipoMed_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS tipoMed_EsActivo
    FROM [Medico].[tbTiposMedicamento]
    WHERE tipoMed_EsEliminado != 1
    ORDER BY ISNULL(tipoMed_FechaModifica, tipoMed_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_TiposParasito_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(tipoPar_FechaModifica, tipoPar_FechaCrea) DESC) AS Fila,
            tipoPar_Id,
            tipoPar_Descripcion,
            tipoPar_Categoria,
            CASE WHEN tipoPar_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS tipoPar_EsActivo
    FROM [Medico].[tbTiposParasito]
    WHERE tipoPar_EsEliminado != 1
    ORDER BY ISNULL(tipoPar_FechaModifica, tipoPar_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_Tratamientos_List]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ROW_NUMBER() OVER(ORDER BY ISNULL(trat.trat_FechaModifica, trat.trat_FechaCrea) DESC) AS Fila,
        trat.trat_Id,
        trat.masc_Id,
        masc.masc_Nombre AS Mascota,
        trat.tipoPar_Id,
        tipoPar.tipoPar_Descripcion AS TipoParasito,
        tipoPar.tipoPar_Categoria AS CategoriaParasito,
        trat.trat_ParasitoDetectado,
        trat.trat_Medicamento,
        trat.tipoMed_Id,
        tipoMed.tipoMed_Descripcion AS TipoMedicamento,
        trat.viaAdmin_Id,
        viaAdmin.viaAdmin_Descripcion AS ViaAdministracion,
        trat.trat_FechaAplicacion,
        trat.trat_ProximaDosis,
        trat.trat_Estado
    FROM [Medico].[tbTratamientos] AS trat
    INNER JOIN [Refugio].[tbMascotas] AS masc
        ON trat.masc_Id = masc.masc_Id
    LEFT JOIN [Medico].[tbTiposParasito] AS tipoPar
        ON trat.tipoPar_Id = tipoPar.tipoPar_Id
    LEFT JOIN [Medico].[tbTiposMedicamento] AS tipoMed
        ON trat.tipoMed_Id = tipoMed.tipoMed_Id
    LEFT JOIN [Medico].[tbViasAdministracion] AS viaAdmin
        ON trat.viaAdmin_Id = viaAdmin.viaAdmin_Id
    WHERE trat.trat_EsEliminado = 0
    ORDER BY ISNULL(trat.trat_FechaModifica, trat.trat_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Medico].[PR_Medico_ViasAdministracion_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(viaAdmin_FechaModifica, viaAdmin_FechaCrea) DESC) AS Fila,
            viaAdmin_Id,
            viaAdmin_Descripcion,
            CASE WHEN viaAdmin_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS viaAdmin_EsActivo
    FROM [Medico].[tbViasAdministracion]
    WHERE viaAdmin_EsEliminado != 1
    ORDER BY ISNULL(viaAdmin_FechaModifica, viaAdmin_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_CitaMedica_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(citaMedica.medic_FechaModifica, citaMedica.medic_FechaCrea) DESC) AS Fila,
            citaMedica.medic_Id,
            mascota.masc_Nombre,
            comportamientos.com_Descripcion,
            citaMedica.medic_FechaConsulta,
            citaMedica.medic_TipoConsulta,
            citaMedica.medic_MotivoConsulta,
            citaMedica.medic_Diagnostico,
            citaMedica.medic_Peso,
            citaMedica.medic_Temperatura,
            citaMedica.medic_FrecuenciaCardiaca,
            citaMedica.medic_FrecuenciaRespiratoria,
            vacunas.vac_Descripcion,
            citaMedica.medic_MedicamentosRecetados,
            citaMedica.medic_Dosificacion,
            citaMedica.medic_ProcedimientosRealizados,
            citaMedica.medic_ResultadosExamenes,
            citaMedica.medic_ProximaCita,
            citaMedica.medic_MotivoProximaCita,
            citaMedica.medic_FechaCrea,
            citaMedica.medic_FechaModifica
    FROM    [Refugio].[tbCitaMedica] AS citaMedica
    INNER JOIN  Refugio.tbMascotas AS mascota
    ON          citaMedica.masc_Id = mascota.masc_Id
    INNER JOIN  Refugio.tbComportamientos AS comportamientos
    ON          citaMedica.com_Id = comportamientos.com_Id
    LEFT JOIN   Refugio.tbVacunas AS vacunas
    ON          citaMedica.vac_Id = vacunas.vac_Id
    WHERE       citaMedica.medic_EsEliminado != 1
    ORDER BY ISNULL(citaMedica.medic_FechaModifica, citaMedica.medic_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Comportamiento_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(com_FechaModifica, com_FechaCrea) DESC) AS Fila,
            com_Id,
            com_Descripcion
    FROM [Refugio].[tbComportamientos] AS comportamientos
    WHERE comportamientos.com_EsEliminado != 1
    ORDER BY ISNULL(com_FechaModifica, com_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Donaciones_List]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ROW_NUMBER() OVER(ORDER BY ISNULL(d.dona_FechaModifica, d.dona_FechaCrea) DESC) AS Fila,
        d.dona_Id,
        d.dona_TipoDonacion,
        d.dona_NombreDonante,
        d.dona_TelefonoDonante,
        d.dona_EmailDonante,
        d.dona_MontoMonetario,
        d.dona_DescripcionArticulos,
        d.dona_ValorEstimado,
        d.dona_FechaDonacion,
        d.dona_Estado,
        d.dona_Observaciones,
        r.refg_Nombre,
        d.dona_FechaCrea,
        uc.Usu_Nombre AS dona_NombreUsuarioCrea
    FROM [Refugio].[tbDonaciones] d
    INNER JOIN [Refugio].[tbRefugios] r ON d.refg_Id = r.refg_Id
    INNER JOIN [Seguridad].[tbUsuarios] uc ON d.dona_UsuarioCrea = uc.usu_Id
    WHERE d.dona_EsEliminado = 0
    ORDER BY ISNULL(d.dona_FechaModifica, d.dona_FechaCrea) DESC;
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Empleados_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(persona.per_FechaModifica, persona.per_FechaCrea) DESC) AS Fila,
            emp_Id,
            emp_Codigo,
            Help.ConcatEspace(persona.per_PrimerNombre, persona.per_ApellidoPaterno) AS [emp_Nombres],
            empleadocargo.cag_Descripcion,
            refugio.refg_Nombre,
            Help.IsActive(emp_EsActivo) AS EsActivo
    FROM        [Refugio].[tbEmpleados] AS empleado
    INNER JOIN  [General].[tbPersonas] AS persona
    ON          empleado.per_Id = persona.per_Id
    INNER JOIN  [Refugio].[tbRefugios] AS refugio
    ON          empleado.refg_Id = refugio.refg_Id
    INNER JOIN  [Refugio].[tbEmpleadosCargos] AS empleadocargo
    ON          empleado.cag_Id = empleadocargo.cag_Id
    WHERE       persona.per_EsEliminado != 1
    ORDER BY ISNULL(persona.per_FechaModifica, persona.per_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_EmpleadosCargos_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(cag_FechaModifica, cag_FechaCrea) DESC) AS Fila,
            cag_Id,
            cag_Descripcion,
            cag_Salario,
            Help.IsActive(cag_EsActivo) AS EsActivo
    FROM [Refugio].[tbEmpleadosCargos] AS empleadocargo
    WHERE empleadocargo.cag_EsEliminado != 1
    ORDER BY ISNULL(cag_FechaModifica, cag_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Eventos_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(eventos.eve_FechaModifica, eventos.eve_FechaCrea) DESC) AS Fila,
            eve_Id,
            eve_Descripcion,
            eventos.refg_Id,
            refugios.refg_Nombre,
            eve_Fecha
    FROM    [Refugio].[tbEventos] AS eventos
    INNER JOIN [Refugio].[tbRefugios] AS refugios
    ON      eventos.refg_Id = refugios.refg_Id
    WHERE   eve_EsEliminado != 1
    ORDER BY ISNULL(eventos.eve_FechaModifica, eventos.eve_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Mascotas_List]
AS
BEGIN
    SELECT      mascotas.masc_Id,
                ROW_NUMBER() OVER(ORDER BY ISNULL(mascotas.masc_FechaModifica, mascotas.masc_FechaCrea) DESC) AS masc_Fila,
                mascotas.masc_Imagen,
                mascotas.masc_Nombre,
                raza.raza_Descripcion,
                mascotas.masc_Edad,
                mascotas.masc_Sexo,
                mascotas.masc_EsAdoptado
    FROM        [Refugio].[tbMascotas] AS mascotas
    INNER JOIN  [Refugio].[tbRazas] AS raza
    ON          mascotas.raza_Id = raza.raza_Id
    INNER JOIN  [Refugio].[tbRefugios] AS albergue
    ON          mascotas.refg_Id = albergue.refg_Id
    INNER JOIN  [Refugio].[tbProcedencias] AS procedencia
    ON          mascotas.proc_Id = procedencia.proc_Id
    WHERE       mascotas.masc_EsEliminado != 1 AND mascotas.masc_EsAdoptado != 1
    ORDER BY ISNULL(mascotas.masc_FechaModifica, mascotas.masc_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Procedencias_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(proc_FechaModifica, proc_FechaCrea) DESC) AS Fila,
            proc_Id,
            proc_Descripcion,
            CASE WHEN proc_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS proc_EsActivo
    FROM [Refugio].[tbProcedencias] AS procedencias
    WHERE proc_EsEliminado != 1
    ORDER BY ISNULL(proc_FechaModifica, proc_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Razas_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(raza_FechaModifica, raza_FechaCrea) DESC) AS Fila,
            raza_Id,
            raza_Descripcion,
            raza_Tamano,
            raza_TipoAnimal,
            raza_TipoPelaje,
            raza_ImagenUrl,
            CASE WHEN raza_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS raza_EsActivo
    FROM [Refugio].[tbRazas]
    WHERE raza_EsEliminado != 1
    ORDER BY ISNULL(raza_FechaModifica, raza_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Refugios_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(refg_FechaModifica, refg_FechaCrea) DESC) AS Fila,
            refg_Id,
            refg_Nombre,
            refg_RTN,
            refg_Ubicacion,
            CASE WHEN refg_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS EsActivo
    FROM [Refugio].[tbRefugios]
    WHERE refg_EsEliminado != 1
    ORDER BY ISNULL(refg_FechaModifica, refg_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Solicitudes_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(solicitudes.sol_FechaModifica, solicitudes.sol_FechaCrea) DESC) AS Fila,
            solicitudes.sol_Id,
            solicitudes.sol_Identidad,
            solicitudes.sol_Nombres,
            mascota.masc_Nombre,
            solicitudes.sol_Correo
    FROM [Refugio].[tbSolicitudes] AS solicitudes
    INNER JOIN [Refugio].[tbMascotas] AS mascota
    ON      solicitudes.masc_Id = mascota.masc_Id
    WHERE   solicitudes.sol_EsEliminado != 1
    ORDER BY ISNULL(solicitudes.sol_FechaModifica, solicitudes.sol_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Vacunas_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(vac_FechaModifica, vac_FechaCrea) DESC) AS Fila,
            vac_Id,
            vac_Descripcion,
            vacu_Especie,
            vacu_DosisRecomendada,
            vacu_PeriodoRefuerzo,
            Help.IsActive(vac_EsActivo) AS EsActivo
    FROM [Refugio].[tbVacunas]
    WHERE vac_EsEliminado != 1
    ORDER BY ISNULL(vac_FechaModifica, vac_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Voluntarios_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(persona.per_FechaModifica, persona.per_FechaCrea) DESC) AS Fila,
            vol_Id,
            vol_HorasTrabajadas,
            Help.ConcatEspace(persona.per_PrimerNombre, persona.per_ApellidoPaterno) AS [vol_Nombres],
            persona.per_Identidad
    FROM        [Refugio].[tbVoluntarios] AS voluntario
    INNER JOIN  [General].[tbPersonas] AS persona
    ON          voluntario.per_Id = persona.per_Id
    WHERE       persona.per_EsEliminado != 1
    ORDER BY ISNULL(persona.per_FechaModifica, persona.per_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Rescate].[PR_Rescate_Ingresos_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(ingr.ingr_FechaModifica, ingr.ingr_FechaCrea) DESC) AS Fila,
            ingr.ingr_Id,
            ingr.repa_Id,
            ingr.refg_Id,
            refg.refg_Nombre,
            ingr.ingr_FechaIngreso,
            ingr.ingr_LugarRescate,
            ingr.ingr_CondicionInicial,
            ingr.ingr_PersonaRescatista,
            ingr.ingr_MedioTransporte,
            ingr.ingr_Observaciones,
            ingr.ingr_EsEmergencia,
            -- Datos del reporte (si existe)
            repa.repa_UbicacionIncidente AS LugarReporte,
            repa.repa_DescripcionAnimal,
            -- Verificar si ya tiene mascota asociada
            (SELECT COUNT(*) FROM [Refugio].[tbMascotas] WHERE masc_IngresoId = ingr.ingr_Id AND masc_EsEliminado != 1) AS TieneMascota
    FROM [Rescate].[tbIngresos] AS ingr
    INNER JOIN [Refugio].[tbRefugios] AS refg
        ON ingr.refg_Id = refg.refg_Id
    LEFT JOIN [Rescate].[tbReportesAbandono] AS repa
        ON ingr.repa_Id = repa.repa_Id
    WHERE ingr.ingr_EsEliminado != 1
    ORDER BY ISNULL(ingr.ingr_FechaModifica, ingr.ingr_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(reptip_FechaModifica, reptip_FechaCrea) DESC) AS Fila,
            reptip_Id,
            reptip_Descripcion,
            reptip_EsActivo
    FROM [Rescate].[tbReportantesTipo]
    WHERE reptip_EsEliminado != 1
    ORDER BY ISNULL(reptip_FechaModifica, reptip_FechaCrea) DESC
END
GO


CREATE OR ALTER PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(repa.repa_FechaModifica, repa.repa_FechaCrea) DESC) AS Fila,
            repa.repa_Id,
            repa.reptip_Id,
            reptip.reptip_Descripcion AS TipoReportante,
            repa.repa_NombreReportante,
            repa.repa_TelefonoContacto,
            repa.repa_Email,
            repa.repa_FechaReporte,
            repa.repa_UbicacionIncidente,
            repa.repa_DescripcionAnimal,
            repa.repa_EstadoAtencion,
            repa.repa_Observaciones,
            repa.repa_EsAnonimo,
            repa.refg_Id,
            refg.refg_Nombre AS NombreRefugio
    FROM [Rescate].[tbReportesAbandono] AS repa
    INNER JOIN [Rescate].[tbReportantesTipo] AS reptip
        ON repa.reptip_Id = reptip.reptip_Id
    INNER JOIN [Refugio].[tbRefugios] AS refg
        ON repa.refg_Id = refg.refg_Id
    WHERE repa.repa_EsEliminado != 1
    ORDER BY ISNULL(repa.repa_FechaModifica, repa.repa_FechaCrea) DESC
END
GO



GO

-- ============================================================
-- Mascotas: Insert, Update
-- (Delete ya estaba en script)
-- ============================================================
ALTER PROCEDURE [Refugio].[PR_Refugio_Mascotas_Insert]
    @masc_Imagen            VARBINARY(MAX) = NULL,
    @masc_Nombre            NVARCHAR(100),
    @raza_Id                INT,
    @masc_Edad              INT            = NULL,
    @masc_Sexo              NVARCHAR(10)   = NULL,
    @masc_Peso              FLOAT          = NULL,
    @masc_Color             NVARCHAR(50)   = NULL,
    @masc_Historia          NVARCHAR(MAX)  = NULL,
    @refg_Id                INT,
    @proc_Id                INT            = NULL,
    @masc_UsuarioCrea       INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        BEGIN TRANSACTION
            INSERT INTO Refugio.tbMascotas
                (masc_Imagen, masc_Nombre, raza_Id, masc_Edad, masc_Sexo, masc_Peso,
                 masc_Color, masc_Historia, refg_Id, proc_Id, masc_UsuarioCrea, masc_FechaCrea)
            VALUES
                (@masc_Imagen, @masc_Nombre, @raza_Id, @masc_Edad, @masc_Sexo, @masc_Peso,
                 @masc_Color, @masc_Historia, @refg_Id, @proc_Id, @masc_UsuarioCrea, GETDATE())
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Mascota creada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

ALTER PROCEDURE [Refugio].[PR_Refugio_Mascotas_Update]
    @masc_Id                INT,
    @masc_Imagen            VARBINARY(MAX) = NULL,
    @masc_Nombre            NVARCHAR(100),
    @raza_Id                INT,
    @masc_Edad              INT            = NULL,
    @masc_Sexo              NVARCHAR(10)   = NULL,
    @masc_Peso              FLOAT          = NULL,
    @masc_Color             NVARCHAR(50)   = NULL,
    @masc_Historia          NVARCHAR(MAX)  = NULL,
    @refg_Id                INT,
    @proc_Id                INT            = NULL,
    @masc_EsAdoptado        INT            = NULL,
    @masc_EsReservado       INT            = NULL,
    @masc_UsuarioModifica   INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Refugio.tbMascotas WHERE masc_Id = @masc_Id AND masc_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'La mascota no fue encontrada.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE Refugio.tbMascotas
            SET masc_Imagen           = @masc_Imagen,
                masc_Nombre           = @masc_Nombre,
                raza_Id               = @raza_Id,
                masc_Edad             = @masc_Edad,
                masc_Sexo             = @masc_Sexo,
                masc_Peso             = @masc_Peso,
                masc_Color            = @masc_Color,
                masc_Historia         = @masc_Historia,
                refg_Id               = @refg_Id,
                proc_Id               = @proc_Id,
                masc_EsAdoptado       = ISNULL(@masc_EsAdoptado,  masc_EsAdoptado),
                masc_EsReservado      = ISNULL(@masc_EsReservado, masc_EsReservado),
                masc_UsuarioModifica  = @masc_UsuarioModifica,
                masc_FechaModifica    = GETDATE()
            WHERE masc_Id = @masc_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Mascota actualizada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ============================================================
-- Empleados: Insert, Update, Delete
-- Dependencia: General.tbPersonas (transaccion multi-tabla)
-- ============================================================
ALTER PROCEDURE [Refugio].[PR_Refugio_Empleados_Insert]
    @emp_Codigo             NVARCHAR(20),
    @refg_Id                INT,
    @cag_Id                 INT,
    @emp_EsActivo           BIT           = 1,
    @per_Identidad          NVARCHAR(20),
    @per_PrimerNombre       NVARCHAR(50),
    @per_SegundoNombre      NVARCHAR(50)  = NULL,
    @per_ApellidoPaterno    NVARCHAR(50),
    @per_ApellidoMaterno    NVARCHAR(50)  = NULL,
    @per_FechaNacimiento    DATE          = NULL,
    @per_Domicilio          NVARCHAR(200) = NULL,
    @per_Telefono           NVARCHAR(20)  = NULL,
    @per_Correo             NVARCHAR(200) = NULL,
    @per_UsuarioCrea        INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM Refugio.tbEmpleados e
            INNER JOIN General.tbPersonas p ON e.per_Id = p.per_Id
            WHERE p.per_Identidad = @per_Identidad AND p.per_EsEliminado = 0
        )
        BEGIN
            SELECT -3 AS CodeStatus, 'Ya existe un empleado con esa identidad.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            DECLARE @per_Id INT
            IF EXISTS (SELECT 1 FROM General.tbPersonas WHERE per_Identidad = @per_Identidad AND per_EsEliminado = 0)
            BEGIN
                SELECT @per_Id = per_Id FROM General.tbPersonas WHERE per_Identidad = @per_Identidad AND per_EsEliminado = 0
                UPDATE General.tbPersonas
                SET per_PrimerNombre    = @per_PrimerNombre,
                    per_SegundoNombre   = @per_SegundoNombre,
                    per_ApellidoPaterno = @per_ApellidoPaterno,
                    per_ApellidoMaterno = @per_ApellidoMaterno,
                    per_FechaNacimiento = @per_FechaNacimiento,
                    per_Domicilio       = @per_Domicilio,
                    per_Telefono        = @per_Telefono,
                    per_Correo          = @per_Correo,
                    per_UsuarioModifica = @per_UsuarioCrea,
                    per_FechaModifica   = GETDATE()
                WHERE per_Id = @per_Id
            END
            ELSE
            BEGIN
                INSERT INTO General.tbPersonas
                    (per_Identidad, per_PrimerNombre, per_SegundoNombre, per_ApellidoPaterno,
                     per_ApellidoMaterno, per_FechaNacimiento, per_Domicilio, per_Telefono,
                     per_Correo, per_UsuarioCrea, per_FechaCrea)
                VALUES
                    (@per_Identidad, @per_PrimerNombre, @per_SegundoNombre, @per_ApellidoPaterno,
                     @per_ApellidoMaterno, @per_FechaNacimiento, @per_Domicilio, @per_Telefono,
                     @per_Correo, @per_UsuarioCrea, GETDATE())
                SET @per_Id = SCOPE_IDENTITY()
            END
            INSERT INTO Refugio.tbEmpleados
                (emp_Codigo, per_Id, refg_Id, cag_Id, emp_EsActivo)
            VALUES
                (@emp_Codigo, @per_Id, @refg_Id, @cag_Id, @emp_EsActivo)
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Empleado creado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

ALTER PROCEDURE [Refugio].[PR_Refugio_Empleados_Update]
    @emp_Id                 INT,
    @emp_Codigo             NVARCHAR(20),
    @per_Id                 INT,
    @refg_Id                INT,
    @cag_Id                 INT,
    @emp_EsActivo           BIT,
    @per_Identidad          NVARCHAR(20),
    @per_PrimerNombre       NVARCHAR(50),
    @per_SegundoNombre      NVARCHAR(50)  = NULL,
    @per_ApellidoPaterno    NVARCHAR(50),
    @per_ApellidoMaterno    NVARCHAR(50)  = NULL,
    @per_FechaNacimiento    DATE          = NULL,
    @per_Domicilio          NVARCHAR(200) = NULL,
    @per_Telefono           NVARCHAR(20)  = NULL,
    @per_Correo             NVARCHAR(200) = NULL,
    @per_UsuarioModifica    INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM Refugio.tbEmpleados e
            INNER JOIN General.tbPersonas p ON e.per_Id = p.per_Id
            WHERE e.emp_Id = @emp_Id AND p.per_EsEliminado = 0
        )
        BEGIN
            SELECT -1 AS CodeStatus, 'El empleado no fue encontrado.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE General.tbPersonas
            SET per_Identidad       = @per_Identidad,
                per_PrimerNombre    = @per_PrimerNombre,
                per_SegundoNombre   = @per_SegundoNombre,
                per_ApellidoPaterno = @per_ApellidoPaterno,
                per_ApellidoMaterno = @per_ApellidoMaterno,
                per_FechaNacimiento = @per_FechaNacimiento,
                per_Domicilio       = @per_Domicilio,
                per_Telefono        = @per_Telefono,
                per_Correo          = @per_Correo,
                per_UsuarioModifica = @per_UsuarioModifica,
                per_FechaModifica   = GETDATE()
            WHERE per_Id = @per_Id
            UPDATE Refugio.tbEmpleados
            SET emp_Codigo   = @emp_Codigo,
                refg_Id      = @refg_Id,
                cag_Id       = @cag_Id,
                emp_EsActivo = @emp_EsActivo
            WHERE emp_Id = @emp_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Empleado actualizado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

ALTER PROCEDURE [Refugio].[PR_Refugio_Empleados_Delete]
    @emp_Id INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM Refugio.tbEmpleados e
            INNER JOIN General.tbPersonas p ON e.per_Id = p.per_Id
            WHERE e.emp_Id = @emp_Id AND p.per_EsEliminado = 0
        )
        BEGIN
            SELECT -1 AS CodeStatus, 'El empleado no fue encontrado.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE General.tbPersonas
            SET per_EsEliminado = 1
            WHERE per_Id = (SELECT per_Id FROM Refugio.tbEmpleados WHERE emp_Id = @emp_Id)
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Empleado eliminado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ============================================================
-- Voluntarios: Insert, Update, Delete
-- Dependencia: General.tbPersonas (transaccion multi-tabla)
-- ============================================================
ALTER PROCEDURE [Refugio].[PR_Refugio_Voluntarios_Insert]
    @vol_HorasTrabajadas    INT,
    @vol_Recurrente         BIT           = 0,
    @per_Identidad          NVARCHAR(20),
    @per_PrimerNombre       NVARCHAR(50),
    @per_SegundoNombre      NVARCHAR(50)  = NULL,
    @per_ApellidoPaterno    NVARCHAR(50),
    @per_ApellidoMaterno    NVARCHAR(50)  = NULL,
    @per_FechaNacimiento    DATE          = NULL,
    @per_Domicilio          NVARCHAR(200) = NULL,
    @per_Telefono           NVARCHAR(20)  = NULL,
    @per_Correo             NVARCHAR(200) = NULL,
    @per_UsuarioCrea        INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM Refugio.tbVoluntarios v
            INNER JOIN General.tbPersonas p ON v.per_Id = p.per_Id
            WHERE p.per_Identidad = @per_Identidad AND p.per_EsEliminado = 0
        )
        BEGIN
            SELECT -3 AS CodeStatus, 'Ya existe un voluntario con esa identidad.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            DECLARE @per_Id INT
            IF EXISTS (SELECT 1 FROM General.tbPersonas WHERE per_Identidad = @per_Identidad AND per_EsEliminado = 0)
            BEGIN
                SELECT @per_Id = per_Id FROM General.tbPersonas WHERE per_Identidad = @per_Identidad AND per_EsEliminado = 0
                UPDATE General.tbPersonas
                SET per_PrimerNombre    = @per_PrimerNombre,
                    per_SegundoNombre   = @per_SegundoNombre,
                    per_ApellidoPaterno = @per_ApellidoPaterno,
                    per_ApellidoMaterno = @per_ApellidoMaterno,
                    per_FechaNacimiento = @per_FechaNacimiento,
                    per_Domicilio       = @per_Domicilio,
                    per_Telefono        = @per_Telefono,
                    per_Correo          = @per_Correo,
                    per_UsuarioModifica = @per_UsuarioCrea,
                    per_FechaModifica   = GETDATE()
                WHERE per_Id = @per_Id
            END
            ELSE
            BEGIN
                INSERT INTO General.tbPersonas
                    (per_Identidad, per_PrimerNombre, per_SegundoNombre, per_ApellidoPaterno,
                     per_ApellidoMaterno, per_FechaNacimiento, per_Domicilio, per_Telefono,
                     per_Correo, per_UsuarioCrea, per_FechaCrea)
                VALUES
                    (@per_Identidad, @per_PrimerNombre, @per_SegundoNombre, @per_ApellidoPaterno,
                     @per_ApellidoMaterno, @per_FechaNacimiento, @per_Domicilio, @per_Telefono,
                     @per_Correo, @per_UsuarioCrea, GETDATE())
                SET @per_Id = SCOPE_IDENTITY()
            END
            INSERT INTO Refugio.tbVoluntarios
                (vol_HorasTrabajadas, per_Id, vol_Recurrente)
            VALUES
                (@vol_HorasTrabajadas, @per_Id, @vol_Recurrente)
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Voluntario creado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

ALTER PROCEDURE [Refugio].[PR_Refugio_Voluntarios_Update]
    @vol_Id                 INT,
    @vol_HorasTrabajadas    INT,
    @per_Id                 INT,
    @vol_Recurrente         BIT,
    @per_Identidad          NVARCHAR(20),
    @per_PrimerNombre       NVARCHAR(50),
    @per_SegundoNombre      NVARCHAR(50)  = NULL,
    @per_ApellidoPaterno    NVARCHAR(50),
    @per_ApellidoMaterno    NVARCHAR(50)  = NULL,
    @per_FechaNacimiento    DATE          = NULL,
    @per_Domicilio          NVARCHAR(200) = NULL,
    @per_Telefono           NVARCHAR(20)  = NULL,
    @per_Correo             NVARCHAR(200) = NULL,
    @per_UsuarioModifica    INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM Refugio.tbVoluntarios v
            INNER JOIN General.tbPersonas p ON v.per_Id = p.per_Id
            WHERE v.vol_Id = @vol_Id AND p.per_EsEliminado = 0
        )
        BEGIN
            SELECT -1 AS CodeStatus, 'El voluntario no fue encontrado.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE General.tbPersonas
            SET per_Identidad       = @per_Identidad,
                per_PrimerNombre    = @per_PrimerNombre,
                per_SegundoNombre   = @per_SegundoNombre,
                per_ApellidoPaterno = @per_ApellidoPaterno,
                per_ApellidoMaterno = @per_ApellidoMaterno,
                per_FechaNacimiento = @per_FechaNacimiento,
                per_Domicilio       = @per_Domicilio,
                per_Telefono        = @per_Telefono,
                per_Correo          = @per_Correo,
                per_UsuarioModifica = @per_UsuarioModifica,
                per_FechaModifica   = GETDATE()
            WHERE per_Id = @per_Id
            UPDATE Refugio.tbVoluntarios
            SET vol_HorasTrabajadas = @vol_HorasTrabajadas,
                vol_Recurrente      = @vol_Recurrente
            WHERE vol_Id = @vol_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Voluntario actualizado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

ALTER PROCEDURE [Refugio].[PR_Refugio_Voluntarios_Delete]
    @vol_Id INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM Refugio.tbVoluntarios v
            INNER JOIN General.tbPersonas p ON v.per_Id = p.per_Id
            WHERE v.vol_Id = @vol_Id AND p.per_EsEliminado = 0
        )
        BEGIN
            SELECT -1 AS CodeStatus, 'El voluntario no fue encontrado.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE General.tbPersonas
            SET per_EsEliminado = 1
            WHERE per_Id = (SELECT per_Id FROM Refugio.tbVoluntarios WHERE vol_Id = @vol_Id)
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Voluntario eliminado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ============================================================
-- CitaMedica: Insert, Update
-- (Delete ya estaba en script)
-- ============================================================
ALTER PROCEDURE [Medico].[PR_Medico_CitaMedica_Insert]
    @masc_Id                        INT,
    @cita_FechaConsulta             DATETIME,
    @tipoCon_Id                     INT           = NULL,
    @grav_Id                        INT           = NULL,
    @cita_MotivoConsulta            NVARCHAR(500) = NULL,
    @cita_Diagnostico               NVARCHAR(500) = NULL,
    @cita_Peso                      DECIMAL(5,2)  = NULL,
    @cita_Temperatura               DECIMAL(4,2)  = NULL,
    @cita_FrecuenciaCardiaca        INT           = NULL,
    @cita_FrecuenciaRespiratoria    INT           = NULL,
    @com_Id                         INT           = NULL,
    @vac_Id                         INT           = NULL,
    @cita_ProcedimientosRealizados  NVARCHAR(500) = NULL,
    @cita_ResultadosExamenes        NVARCHAR(500) = NULL,
    @cita_ProximaCita               DATETIME      = NULL,
    @cita_MotivoProximaCita         NVARCHAR(200) = NULL,
    @cita_UsuarioCrea               INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Refugio.tbMascotas WHERE masc_Id = @masc_Id AND masc_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'La mascota no fue encontrada.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            INSERT INTO Medico.tbCitaMedica
                (masc_Id, cita_FechaConsulta, tipoCon_Id, grav_Id, cita_MotivoConsulta,
                 cita_Diagnostico, cita_Peso, cita_Temperatura, cita_FrecuenciaCardiaca,
                 cita_FrecuenciaRespiratoria, com_Id, vac_Id, cita_ProcedimientosRealizados,
                 cita_ResultadosExamenes, cita_ProximaCita, cita_MotivoProximaCita,
                 cita_UsuarioCrea, cita_FechaCrea)
            VALUES
                (@masc_Id, @cita_FechaConsulta, @tipoCon_Id, @grav_Id, @cita_MotivoConsulta,
                 @cita_Diagnostico, @cita_Peso, @cita_Temperatura, @cita_FrecuenciaCardiaca,
                 @cita_FrecuenciaRespiratoria, @com_Id, @vac_Id, @cita_ProcedimientosRealizados,
                 @cita_ResultadosExamenes, @cita_ProximaCita, @cita_MotivoProximaCita,
                 @cita_UsuarioCrea, GETDATE())
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Cita medica creada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

ALTER PROCEDURE [Medico].[PR_Medico_CitaMedica_Update]
    @cita_Id                        INT,
    @masc_Id                        INT,
    @cita_FechaConsulta             DATETIME,
    @tipoCon_Id                     INT           = NULL,
    @grav_Id                        INT           = NULL,
    @cita_MotivoConsulta            NVARCHAR(500) = NULL,
    @cita_Diagnostico               NVARCHAR(500) = NULL,
    @cita_Peso                      DECIMAL(5,2)  = NULL,
    @cita_Temperatura               DECIMAL(4,2)  = NULL,
    @cita_FrecuenciaCardiaca        INT           = NULL,
    @cita_FrecuenciaRespiratoria    INT           = NULL,
    @com_Id                         INT           = NULL,
    @vac_Id                         INT           = NULL,
    @cita_ProcedimientosRealizados  NVARCHAR(500) = NULL,
    @cita_ResultadosExamenes        NVARCHAR(500) = NULL,
    @cita_ProximaCita               DATETIME      = NULL,
    @cita_MotivoProximaCita         NVARCHAR(200) = NULL,
    @cita_UsuarioModifica           INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Medico.tbCitaMedica WHERE cita_Id = @cita_Id AND cita_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'La cita medica no fue encontrada.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE Medico.tbCitaMedica
            SET masc_Id                        = @masc_Id,
                cita_FechaConsulta             = @cita_FechaConsulta,
                tipoCon_Id                     = @tipoCon_Id,
                grav_Id                        = @grav_Id,
                cita_MotivoConsulta            = @cita_MotivoConsulta,
                cita_Diagnostico               = @cita_Diagnostico,
                cita_Peso                      = @cita_Peso,
                cita_Temperatura               = @cita_Temperatura,
                cita_FrecuenciaCardiaca        = @cita_FrecuenciaCardiaca,
                cita_FrecuenciaRespiratoria    = @cita_FrecuenciaRespiratoria,
                com_Id                         = @com_Id,
                vac_Id                         = @vac_Id,
                cita_ProcedimientosRealizados  = @cita_ProcedimientosRealizados,
                cita_ResultadosExamenes        = @cita_ResultadosExamenes,
                cita_ProximaCita               = @cita_ProximaCita,
                cita_MotivoProximaCita         = @cita_MotivoProximaCita,
                cita_UsuarioModifica           = @cita_UsuarioModifica,
                cita_FechaModifica             = GETDATE()
            WHERE cita_Id = @cita_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Cita medica actualizada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ============================================================
-- Recetas: Insert, Update
-- (Delete ya estaba en script)
-- ============================================================
ALTER PROCEDURE [Medico].[PR_Medico_Recetas_Insert]
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
    @receta_UsuarioCrea     INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        BEGIN TRANSACTION
            INSERT INTO Medico.tbRecetas
                (cita_Id, masc_Id, receta_Medicamento, tipoMed_Id, viaAdmin_Id,
                 receta_Dosis, receta_Frecuencia, receta_Duracion, receta_Instrucciones,
                 receta_FechaInicio, receta_FechaFin, receta_Estado,
                 receta_UsuarioCrea, receta_FechaCrea)
            VALUES
                (@cita_Id, @masc_Id, @receta_Medicamento, @tipoMed_Id, @viaAdmin_Id,
                 @receta_Dosis, @receta_Frecuencia, @receta_Duracion, @receta_Instrucciones,
                 @receta_FechaInicio, @receta_FechaFin, @receta_Estado,
                 @receta_UsuarioCrea, GETDATE())
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Receta creada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

ALTER PROCEDURE [Medico].[PR_Medico_Recetas_Update]
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

-- ============================================================
-- Tratamientos: Insert, Update
-- (Delete ya estaba en script)
-- ============================================================
ALTER PROCEDURE [Medico].[PR_Medico_Tratamientos_Insert]
    @masc_Id                INT,
    @cita_Id                INT           = NULL,
    @receta_Id              INT           = NULL,
    @tipoPar_Id             INT           = NULL,
    @trat_ParasitoDetectado NVARCHAR(200) = NULL,
    @trat_Medicamento       NVARCHAR(200) = NULL,
    @tipoMed_Id             INT           = NULL,
    @viaAdmin_Id            INT           = NULL,
    @trat_FechaAplicacion   DATETIME      = NULL,
    @trat_AplicadoPor       NVARCHAR(100) = NULL,
    @trat_ProximaDosis      DATETIME      = NULL,
    @trat_Estado            NVARCHAR(50)  = NULL,
    @trat_Observaciones     NVARCHAR(500) = NULL,
    @trat_UsuarioCrea       INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        BEGIN TRANSACTION
            INSERT INTO Medico.tbTratamientos
                (masc_Id, cita_Id, receta_Id, tipoPar_Id, trat_ParasitoDetectado, trat_Medicamento,
                 tipoMed_Id, viaAdmin_Id, trat_FechaAplicacion, trat_AplicadoPor,
                 trat_ProximaDosis, trat_Estado, trat_Observaciones,
                 trat_UsuarioCrea, trat_FechaCrea)
            VALUES
                (@masc_Id, @cita_Id, @receta_Id, @tipoPar_Id, @trat_ParasitoDetectado, @trat_Medicamento,
                 @tipoMed_Id, @viaAdmin_Id, @trat_FechaAplicacion, @trat_AplicadoPor,
                 @trat_ProximaDosis, @trat_Estado, @trat_Observaciones,
                 @trat_UsuarioCrea, GETDATE())
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Tratamiento creado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

ALTER PROCEDURE [Medico].[PR_Medico_Tratamientos_Update]
    @trat_Id                INT,
    @masc_Id                INT,
    @cita_Id                INT           = NULL,
    @receta_Id              INT           = NULL,
    @tipoPar_Id             INT           = NULL,
    @trat_ParasitoDetectado NVARCHAR(200) = NULL,
    @trat_Medicamento       NVARCHAR(200) = NULL,
    @tipoMed_Id             INT           = NULL,
    @viaAdmin_Id            INT           = NULL,
    @trat_FechaAplicacion   DATETIME      = NULL,
    @trat_AplicadoPor       NVARCHAR(100) = NULL,
    @trat_ProximaDosis      DATETIME      = NULL,
    @trat_Estado            NVARCHAR(50)  = NULL,
    @trat_Observaciones     NVARCHAR(500) = NULL,
    @trat_UsuarioModifica   INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Medico.tbTratamientos WHERE trat_Id = @trat_Id AND trat_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'El tratamiento no fue encontrado.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE Medico.tbTratamientos
            SET masc_Id                = @masc_Id,
                cita_Id                = @cita_Id,
                receta_Id              = @receta_Id,
                tipoPar_Id             = @tipoPar_Id,
                trat_ParasitoDetectado = @trat_ParasitoDetectado,
                trat_Medicamento       = @trat_Medicamento,
                tipoMed_Id             = @tipoMed_Id,
                viaAdmin_Id            = @viaAdmin_Id,
                trat_FechaAplicacion   = @trat_FechaAplicacion,
                trat_AplicadoPor       = @trat_AplicadoPor,
                trat_ProximaDosis      = @trat_ProximaDosis,
                trat_Estado            = @trat_Estado,
                trat_Observaciones     = @trat_Observaciones,
                trat_UsuarioModifica   = @trat_UsuarioModifica,
                trat_FechaModifica     = GETDATE()
            WHERE trat_Id = @trat_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Tratamiento actualizado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ============================================================
-- RecepcionesDetalles: ByRecepcion (consulta filtrada)
-- ============================================================
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_RecepcionesDetalles_ByRecepcion]
    @recep_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  rd.recdet_Id,
            rd.recep_Id,
            rd.itm_Id,
            i.itm_Codigo,
            i.itm_Descripcion,
            rd.recdet_Cantidad,
            rd.recdet_PrecioUnitario,
            rd.recdet_FechaVencimiento,
            rd.recdet_NumeroLote
    FROM Inventario.tbRecepcionesDetalles rd
    INNER JOIN Inventario.tbItems i ON rd.itm_Id = i.itm_Id
    WHERE rd.recep_Id = @recep_Id
      AND rd.recdet_EsEliminado = 0
    ORDER BY rd.recdet_Id ASC
END
GO

-- ============================================================
-- Razas: Insert, Update, Delete
-- ============================================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Razas_Insert]
    @raza_Descripcion   NVARCHAR(100),
    @raza_Tamano        NVARCHAR(20)  = NULL,
    @raza_TipoAnimal    NVARCHAR(50)  = NULL,
    @raza_TipoPelaje    NVARCHAR(30)  = NULL,
    @raza_ImagenUrl     NVARCHAR(500) = NULL,
    @raza_EsActivo      BIT           = 1,
    @raza_UsuarioCrea   INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF EXISTS (
            SELECT 1 FROM Refugio.tbRazas
            WHERE raza_Descripcion = @raza_Descripcion AND raza_EsEliminado = 0
        )
        BEGIN
            SELECT -3 AS CodeStatus, 'Ya existe una raza con esa descripción.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            INSERT INTO Refugio.tbRazas
                (raza_Descripcion, raza_Tamano, raza_TipoAnimal, raza_TipoPelaje,
                 raza_ImagenUrl, raza_EsActivo, raza_EsEliminado,
                 raza_UsuarioCrea, raza_FechaCrea)
            VALUES
                (@raza_Descripcion, @raza_Tamano, @raza_TipoAnimal, @raza_TipoPelaje,
                 @raza_ImagenUrl, @raza_EsActivo, 0,
                 @raza_UsuarioCrea, GETDATE())
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Raza creada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Razas_Update]
    @raza_Id            INT,
    @raza_Descripcion   NVARCHAR(100),
    @raza_Tamano        NVARCHAR(20)  = NULL,
    @raza_TipoAnimal    NVARCHAR(50)  = NULL,
    @raza_TipoPelaje    NVARCHAR(30)  = NULL,
    @raza_ImagenUrl     NVARCHAR(500) = NULL,
    @raza_EsActivo      BIT           = 1,
    @raza_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM Refugio.tbRazas
            WHERE raza_Id = @raza_Id AND raza_EsEliminado = 0
        )
        BEGIN
            SELECT -1 AS CodeStatus, 'La raza no fue encontrada.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE Refugio.tbRazas
            SET raza_Descripcion    = @raza_Descripcion,
                raza_Tamano         = @raza_Tamano,
                raza_TipoAnimal     = @raza_TipoAnimal,
                raza_TipoPelaje     = @raza_TipoPelaje,
                raza_ImagenUrl      = @raza_ImagenUrl,
                raza_EsActivo       = @raza_EsActivo,
                raza_UsuarioModifica = @raza_UsuarioModifica,
                raza_FechaModifica  = GETDATE()
            WHERE raza_Id = @raza_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Raza actualizada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Razas_Delete]
    @raza_Id INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM Refugio.tbRazas
            WHERE raza_Id = @raza_Id AND raza_EsEliminado = 0
        )
        BEGIN
            SELECT -1 AS CodeStatus, 'La raza no fue encontrada.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE Refugio.tbRazas
            SET raza_EsEliminado = 1
            WHERE raza_Id = @raza_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Raza eliminada correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ============================================================
-- Validaciones de duplicados (SPs de consulta)
-- ============================================================
CREATE OR ALTER PROCEDURE [Refugio].[PR_Refugio_Razas_Existe]
    @raza_Descripcion NVARCHAR(100),
    @raza_Id          INT = 0
AS
BEGIN
    SET NOCOUNT ON
    IF EXISTS (
        SELECT 1 FROM Refugio.tbRazas
        WHERE raza_Descripcion = @raza_Descripcion
          AND raza_EsEliminado = 0
          AND raza_Id != @raza_Id
    )
        SELECT CAST(1 AS BIT) AS Existe
    ELSE
        SELECT CAST(0 AS BIT) AS Existe
END
GO

PRINT 'Script 16 completado: todos los SPs estandarizados con TRY/CATCH y codigos de retorno.'
GO
