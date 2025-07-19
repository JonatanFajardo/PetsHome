using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio para generar reportes del sistema PetsHome
    /// </summary>
    public class ReportesService
    {
        private readonly ReportesRepository _reportesRepository;
        private readonly ILogger<ReportesService> _logger;
        private readonly IMapper _mapper;

        public ReportesService(ReportesRepository reportesRepository, ILogger<ReportesService> logger, IMapper mapper)
        {
            _reportesRepository = reportesRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene el dashboard principal con métricas generales
        /// </summary>
        public async Task<ReportesDashboardViewModel> GetDashboardAsync()
        {
            try
            {
                var dashboard = new ReportesDashboardViewModel();
                
                // Obtener métricas principales del procedimiento
                var dashboardData = await _reportesRepository.GetDashboardAsync();
                
                if (dashboardData != null)
                {
                    dashboard.TotalMascotas = dashboardData.TotalMascotas;
                    dashboard.MascotasAdoptadas = dashboardData.MascotasAdoptadas;
                    dashboard.MascotasDisponibles = dashboardData.MascotasDisponibles;
                    dashboard.CitasMedicasPendientes = dashboardData.CitasMedicasPendientes;
                    dashboard.VoluntariosActivos = dashboardData.VoluntariosActivos;
                    dashboard.EventosEsteMes = dashboardData.EventosEsteMes;
                    dashboard.PorcentajeAdopciones = dashboardData.PorcentajeAdopciones;
                }

                // Obtener datos para gráficos
                dashboard.MascotasPorRaza = await GetMascotasPorRazaAsync();
                dashboard.AdopcionesPorMes = await GetAdopcionesPorMesAsync();
                dashboard.CitasMedicasPorTipo = await GetCitasMedicasPorTipoAsync();

                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar dashboard de reportes");
                return new ReportesDashboardViewModel();
            }
        }

        /// <summary>
        /// Obtiene reporte de mascotas por raza
        /// </summary>
        public async Task<List<ReporteMascotasPorRaza>> GetMascotasPorRazaAsync(int? refugioId = null)
        {
            try
            {
                var datos = await _reportesRepository.GetMascotasPorRazaAsync(refugioId);
                
                var resultado = datos.Select(d => new ReporteMascotasPorRaza
                {
                    Raza = d.raza_Descripcion,
                    Cantidad = d.TotalMascotas,
                    Adoptadas = d.MascotasAdoptadas,
                    Disponibles = d.MascotasDisponibles
                }).ToList();


                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de mascotas por raza");
                return new List<ReporteMascotasPorRaza>();
            }
        }

        /// <summary>
        /// Obtiene reporte de adopciones por mes
        /// </summary>
        public async Task<List<ReporteAdopcionesPorMes>> GetAdopcionesPorMesAsync(int mesesAtras = 6, int? refugioId = null)
        {
            try
            {
                var datos = await _reportesRepository.GetAdopcionesPorMesAsync(mesesAtras, refugioId);
                
                var resultado = datos.Select(d => new ReporteAdopcionesPorMes
                {
                    Año = d.Año,
                    Mes = d.NombreMes,
                    Cantidad = d.TotalAdopciones
                }).ToList();


                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de adopciones por mes");
                return new List<ReporteAdopcionesPorMes>();
            }
        }

        /// <summary>
        /// Obtiene reporte de citas médicas por tipo
        /// </summary>
        public async Task<List<ReporteCitasMedicasPorTipo>> GetCitasMedicasPorTipoAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? refugioId = null)
        {
            try
            {
                var datos = await _reportesRepository.GetCitasMedicasPorTipoAsync(fechaInicio, fechaFin, refugioId);
                
                var resultado = datos.Select(d => new ReporteCitasMedicasPorTipo
                {
                    TipoConsulta = d.medic_TipoConsulta,
                    Cantidad = d.TotalCitas,
                    PorcentajeCitas = d.PorcentajeCitas
                }).ToList();


                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de citas médicas por tipo");
                return new List<ReporteCitasMedicasPorTipo>();
            }
        }

        /// <summary>
        /// Obtiene reporte de voluntarios con su participación
        /// </summary>
        public async Task<List<ReporteVoluntarios>> GetReporteVoluntariosAsync(bool soloActivos = false, int? refugioId = null)
        {
            try
            {
                var datos = await _reportesRepository.GetVoluntariosAsync(soloActivos, refugioId);
                
                var resultado = datos.Select(d => new ReporteVoluntarios
                {
                    Id = d.vol_Id,
                    NombreCompleto = d.NombreCompleto,
                    Telefono = d.per_Telefono,
                    Email = d.per_Correo,
                    EventosParticipados = d.EventosParticipados,
                    UltimaParticipacion = d.UltimaParticipacion,
                    Estado = d.Estado
                }).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de voluntarios");
                return new List<ReporteVoluntarios>();
            }
        }

        /// <summary>
        /// Obtiene reporte de inventario con stock crítico
        /// </summary>
        public async Task<List<ReporteInventario>> GetReporteInventarioAsync(int? refugioId = null, bool soloCriticos = false)
        {
            try
            {
                var datos = await _reportesRepository.GetInventarioAsync(refugioId, soloCriticos);
                
                var resultado = datos.Select(d => new ReporteInventario
                {
                    ItemId = d.itm_Id,
                    NombreItem = d.itm_Descripcion,
                    Categoria = d.cat_Descripcion,
                    StockActual = d.StockActual,
                    StockMinimo = d.StockMinimo,
                    EstadoStock = d.EstadoStock,
                    CostoUnitario = d.itm_Precio,
                    ValorTotal = d.ValorTotal
                }).ToList();


                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de inventario");
                return new List<ReporteInventario>();
            }
        }

        /// <summary>
        /// Obtiene reporte de eventos del refugio
        /// </summary>
        public async Task<List<ReporteEventos>> GetReporteEventosAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? refugioId = null, bool soloFuturos = false)
        {
            try
            {
                var datos = await _reportesRepository.GetEventosAsync(fechaInicio, fechaFin, refugioId, soloFuturos);
                
                var resultado = datos.Select(d => new ReporteEventos
                {
                    Id = d.eve_Id,
                    NombreEvento = d.eve_Descripcion,
                    FechaEvento = d.eve_Fecha,
                    VoluntariosParticipantes = d.VoluntariosParticipantes,
                    Estado = d.Estado,
                    Descripcion = d.eve_Descripcion
                }).ToList();


                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de eventos");
                return new List<ReporteEventos>();
            }
        }

        /// <summary>
        /// Obtiene reporte de salud de mascotas
        /// </summary>
        public async Task<List<ReporteSaludMascotas>> GetReporteSaludMascotasAsync(int? refugioId = null, bool soloProblematicas = false)
        {
            try
            {
                var datos = await _reportesRepository.GetSaludMascotasAsync(refugioId, soloProblematicas);
                
                var resultado = datos.Select(d => new ReporteSaludMascotas
                {
                    MascotaId = d.masc_Id,
                    NombreMascota = d.masc_Nombre,
                    Raza = d.raza_Descripcion,
                    UltimaCitaMedica = d.UltimaCitaMedica,
                    EstadoSalud = d.EstadoSalud,
                    TotalCitas = d.TotalCitas,
                    VacunasAlDia = d.VacunasAlDia,
                    Refugio = d.refg_Nombre
                }).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte de salud de mascotas");
                return new List<ReporteSaludMascotas>();
            }
        }
    }
}