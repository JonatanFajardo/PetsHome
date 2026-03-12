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
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(empleado.emp_FechaModifica, empleado.emp_FechaCrea) DESC) AS Fila,
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
    ORDER BY ISNULL(empleado.emp_FechaModifica, empleado.emp_FechaCrea) DESC
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
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(voluntario.vol_FechaModifica, voluntario.vol_FechaCrea) DESC) AS Fila,
            vol_Id,
            vol_HorasTrabajadas,
            Help.ConcatEspace(persona.per_PrimerNombre, persona.per_ApellidoPaterno) AS [vol_Nombres],
            persona.per_Identidad
    FROM        [Refugio].[tbVoluntarios] AS voluntario
    INNER JOIN  [General].[tbPersonas] AS persona
    ON          voluntario.per_Id = persona.per_Id
    WHERE       persona.per_EsEliminado != 1
    ORDER BY ISNULL(voluntario.vol_FechaModifica, voluntario.vol_FechaCrea) DESC
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


