        using PetsHome.Common.Entities;
using System.Collections.Generic;

        namespace PetsHome.Business.Models
        {
            public class ControlVacunacionViewModel
            {
                // ── Secciones ────────────────────────────────────
                public List<PR_Medico_ControlVacunacion_DashboardResult> Dashboard { get; set; }
            = new List<PR_Medico_ControlVacunacion_DashboardResult>();

        public List<PR_Medico_ControlVacunacion_MatrizVacunacionResult> MatrizVacunacion { get; set; }
            = new List<PR_Medico_ControlVacunacion_MatrizVacunacionResult>();

                // ── Conteos calculados ────────────────────────────
                public int TotalDashboard => Dashboard?.Count ?? 0;
        public int TotalMatrizVacunacion => MatrizVacunacion?.Count ?? 0;
                public int TotalAlertas => TotalDashboard + TotalMatrizVacunacion;
            }
        }
