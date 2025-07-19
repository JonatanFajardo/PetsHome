using System;
using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// ViewModel para el dashboard de reportes
    /// </summary>
    public class ReportesDashboardViewModel
    {
        public int TotalMascotas { get; set; }
        public int MascotasAdoptadas { get; set; }
        public int MascotasDisponibles { get; set; }
        public int CitasMedicasPendientes { get; set; }
        public int VoluntariosActivos { get; set; }
        public int EventosEsteMes { get; set; }
        public decimal PorcentajeAdopciones { get; set; }
        public List<ReporteMascotasPorRaza> MascotasPorRaza { get; set; } = new List<ReporteMascotasPorRaza>();
        public List<ReporteAdopcionesPorMes> AdopcionesPorMes { get; set; } = new List<ReporteAdopcionesPorMes>();
        public List<ReporteCitasMedicasPorTipo> CitasMedicasPorTipo { get; set; } = new List<ReporteCitasMedicasPorTipo>();
    }

    /// <summary>
    /// ViewModel para reporte de mascotas por raza
    /// </summary>
    public class ReporteMascotasPorRaza
    {
        public string Raza { get; set; }
        public int Cantidad { get; set; }
        public int Adoptadas { get; set; }
        public int Disponibles { get; set; }
    }

    /// <summary>
    /// ViewModel para reporte de adopciones por mes
    /// </summary>
    public class ReporteAdopcionesPorMes
    {
        public string Mes { get; set; }
        public int Cantidad { get; set; }
        public int Año { get; set; }
    }

    /// <summary>
    /// ViewModel para reporte de citas médicas por tipo
    /// </summary>
    public class ReporteCitasMedicasPorTipo
    {
        public string TipoConsulta { get; set; }
        public int Cantidad { get; set; }
        public decimal PorcentajeCitas { get; set; }
    }

    /// <summary>
    /// ViewModel para reporte de voluntarios
    /// </summary>
    public class ReporteVoluntarios
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public int EventosParticipados { get; set; }
        public DateTime? UltimaParticipacion { get; set; }
        public string Estado { get; set; }
    }

    /// <summary>
    /// ViewModel para reporte de inventario
    /// </summary>
    public class ReporteInventario
    {
        public int ItemId { get; set; }
        public string NombreItem { get; set; }
        public string Categoria { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public string EstadoStock { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }

    /// <summary>
    /// ViewModel para reporte de eventos
    /// </summary>
    public class ReporteEventos
    {
        public int Id { get; set; }
        public string NombreEvento { get; set; }
        public DateTime FechaEvento { get; set; }
        public int VoluntariosParticipantes { get; set; }
        public string Estado { get; set; }
        public string Descripcion { get; set; }
    }

    /// <summary>
    /// ViewModel para reporte de salud de mascotas
    /// </summary>
    public class ReporteSaludMascotas
    {
        public int MascotaId { get; set; }
        public string NombreMascota { get; set; }
        public string Raza { get; set; }
        public DateTime? UltimaCitaMedica { get; set; }
        public string EstadoSalud { get; set; }
        public int TotalCitas { get; set; }
        public bool VacunasAlDia { get; set; }
        public string Refugio { get; set; }
    }

    /// <summary>
    /// ViewModel para filtros de reportes
    /// </summary>
    public class ReporteFiltrosViewModel
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? RefugioId { get; set; }
        public int? RazaId { get; set; }
        public string EstadoAdopcion { get; set; }
        public string TipoReporte { get; set; }
    }
}