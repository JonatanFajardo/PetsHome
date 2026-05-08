        using PetsHome.Common.Entities;
using System.Collections.Generic;

        namespace PetsHome.Business.Models
        {
            public class DashboardVeterinarioViewModel
            {
                // ── Secciones ────────────────────────────────────
                public List<PR_Medico_DashboardVeterinario_AgendaHoyResult> AgendaHoy { get; set; }
            = new List<PR_Medico_DashboardVeterinario_AgendaHoyResult>();

        public List<PR_Medico_DashboardVeterinario_TratamientosActivosResult> TratamientosActivos { get; set; }
            = new List<PR_Medico_DashboardVeterinario_TratamientosActivosResult>();

        public List<PR_Medico_DashboardVeterinario_AlertasVeterinarioResult> AlertasVeterinario { get; set; }
            = new List<PR_Medico_DashboardVeterinario_AlertasVeterinarioResult>();

        public List<PR_Medico_DashboardVeterinario_ResumenMesResult> ResumenMes { get; set; }
            = new List<PR_Medico_DashboardVeterinario_ResumenMesResult>();

                // ── Conteos calculados ────────────────────────────
                public int TotalAgendaHoy => AgendaHoy?.Count ?? 0;
        public int TotalTratamientosActivos => TratamientosActivos?.Count ?? 0;
        public int TotalAlertasVeterinario => AlertasVeterinario?.Count ?? 0;
        public int TotalResumenMes => ResumenMes?.Count ?? 0;
                public int TotalAlertas => TotalAgendaHoy + TotalTratamientosActivos + TotalAlertasVeterinario + TotalResumenMes;
            }
        }
