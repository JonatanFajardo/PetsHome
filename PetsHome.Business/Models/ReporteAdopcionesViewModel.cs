        using PetsHome.Common.Entities;
using System.Collections.Generic;

        namespace PetsHome.Business.Models
        {
            public class ReporteAdopcionesViewModel
            {
                // ── Secciones ────────────────────────────────────
                public List<PR_Refugio_ReporteAdopciones_ResumenResult> Resumen { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_ResumenResult>();

        public List<PR_Refugio_ReporteAdopciones_AdopcionesPorMesResult> AdopcionesPorMes { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_AdopcionesPorMesResult>();

        public List<PR_Refugio_ReporteAdopciones_EstadoSolicitudesResult> EstadoSolicitudes { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_EstadoSolicitudesResult>();

        public List<PR_Refugio_ReporteAdopciones_TopRazasResult> TopRazas { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_TopRazasResult>();

        public List<PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult> AdopcionesRecientes { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult>();

                // ── Conteos calculados ────────────────────────────
                public int TotalResumen => Resumen?.Count ?? 0;
        public int TotalAdopcionesPorMes => AdopcionesPorMes?.Count ?? 0;
        public int TotalEstadoSolicitudes => EstadoSolicitudes?.Count ?? 0;
        public int TotalTopRazas => TopRazas?.Count ?? 0;
        public int TotalAdopcionesRecientes => AdopcionesRecientes?.Count ?? 0;
                public int TotalAlertas => TotalResumen + TotalAdopcionesPorMes + TotalEstadoSolicitudes + TotalTopRazas + TotalAdopcionesRecientes;
            }
        }
