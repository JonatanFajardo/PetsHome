using PetsHome.Common.Entities;
using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    public class DashboardAdminViewModel
    {
        public PR_General_DashboardAdmin_KPIsResult KPIs { get; set; }
            = new PR_General_DashboardAdmin_KPIsResult();

        public List<PR_General_DashboardAdmin_TendenciasResult> Tendencias { get; set; }
            = new List<PR_General_DashboardAdmin_TendenciasResult>();

        public List<PR_General_DashboardAdmin_MascotasEstadoResult> MascotasEstado { get; set; }
            = new List<PR_General_DashboardAdmin_MascotasEstadoResult>();

        public List<PR_General_DashboardAdmin_TopRazasResult> TopRazas { get; set; }
            = new List<PR_General_DashboardAdmin_TopRazasResult>();

        public List<PR_General_DashboardAdmin_CitasHoyResult> CitasHoy { get; set; }
            = new List<PR_General_DashboardAdmin_CitasHoyResult>();

        public List<PR_General_DashboardAdmin_SolicitudesPendientesResult> SolicitudesPendientes { get; set; }
            = new List<PR_General_DashboardAdmin_SolicitudesPendientesResult>();

        public List<PR_General_DashboardAdmin_UsuariosPorRolResult> UsuariosPorRol { get; set; }
            = new List<PR_General_DashboardAdmin_UsuariosPorRolResult>();

        public List<PR_General_DashboardAdmin_HeatmapCitasResult> HeatmapCitas { get; set; }
            = new List<PR_General_DashboardAdmin_HeatmapCitasResult>();

        public List<PR_General_DashboardAdmin_InventarioAlertaResult> InventarioAlertas { get; set; }
            = new List<PR_General_DashboardAdmin_InventarioAlertaResult>();

        public List<PR_General_DashboardAdmin_EmbudoResult> EmbudoAdopcion { get; set; }
            = new List<PR_General_DashboardAdmin_EmbudoResult>();
    }
}
