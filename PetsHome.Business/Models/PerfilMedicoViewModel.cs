        using PetsHome.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

        namespace PetsHome.Business.Models
        {
            public class PerfilMedicoViewModel
            {
                // ── Secciones ────────────────────────────────────
                public List<PR_Medico_PerfilMedico_FichaMascotaResult> FichaMascota { get; set; }
            = new List<PR_Medico_PerfilMedico_FichaMascotaResult>();

        public List<PR_Medico_PerfilMedico_UltimasCitasResult> UltimasCitas { get; set; }
            = new List<PR_Medico_PerfilMedico_UltimasCitasResult>();

        public List<PR_Medico_PerfilMedico_MedicamentosActivosResult> MedicamentosActivos { get; set; }
            = new List<PR_Medico_PerfilMedico_MedicamentosActivosResult>();

        public List<PR_Medico_PerfilMedico_TodasCitasResult> TodasCitas { get; set; }
            = new List<PR_Medico_PerfilMedico_TodasCitasResult>();

        public List<PR_Medico_PerfilMedico_TratamientosResult> Tratamientos { get; set; }
            = new List<PR_Medico_PerfilMedico_TratamientosResult>();

        public List<PR_Medico_PerfilMedico_VacunasResult> Vacunas { get; set; }
            = new List<PR_Medico_PerfilMedico_VacunasResult>();

                // ── Pestanas nuevas vs OpenVPM (script 88) ────────
                // Facturas · Signos vitales · Historial de peso · Laboratorio
                public List<FichaFacturaVM> Facturas { get; set; } = new List<FichaFacturaVM>();
        public List<FichaVitalVM> SignosVitales { get; set; } = new List<FichaVitalVM>();
        public List<FichaPesoVM> HistorialPeso { get; set; } = new List<FichaPesoVM>();
        public List<FichaLabVM> Laboratorios { get; set; } = new List<FichaLabVM>();
        public List<FichaProcedimientoVM> Procedimientos { get; set; } = new List<FichaProcedimientoVM>();

        // Puntos para la grafica de tendencia de laboratorio (un punto por
        // parametro y fecha). La vista los agrupa por parametro.
        public List<FichaLabTendenciaVM> LaboratorioTendencia { get; set; } = new List<FichaLabTendenciaVM>();

                // ── Conteos calculados ────────────────────────────
                public int TotalFichaMascota => FichaMascota?.Count ?? 0;
        public int TotalUltimasCitas => UltimasCitas?.Count ?? 0;
        public int TotalMedicamentosActivos => MedicamentosActivos?.Count ?? 0;
        public int TotalTodasCitas => TodasCitas?.Count ?? 0;
        public int TotalTratamientos => Tratamientos?.Count ?? 0;
        public int TotalVacunas => Vacunas?.Count ?? 0;
        public int TotalFacturas => Facturas?.Count ?? 0;
        public int TotalLaboratorios => Laboratorios?.Count ?? 0;
        public int TotalProcedimientos => Procedimientos?.Count ?? 0;
        // Saldo pendiente total del paciente (suma de saldos de sus facturas).
        public decimal SaldoPendiente => Facturas?.Sum(f => f.Saldo) ?? 0m;
                public int TotalAlertas => TotalFichaMascota + TotalUltimasCitas + TotalMedicamentosActivos + TotalTodasCitas + TotalTratamientos + TotalVacunas;
            }

            // ── Sub-ViewModels de las pestanas (nombres de propiedad exactos:
            //    el frontend de la ficha los consume tal cual) ──────────────
            public class FichaFacturaVM
            {
                public string Numero { get; set; }
                public DateTime Fecha { get; set; }
                public decimal Total { get; set; }
                public decimal Saldo { get; set; }
                public string Estado { get; set; }
            }

            public class FichaVitalVM
            {
                public DateTime Fecha { get; set; }
                public decimal? Peso { get; set; }
                public decimal? Temperatura { get; set; }
                public int? FrecuenciaCardiaca { get; set; }
                public int? FrecuenciaRespiratoria { get; set; }
                public string Mucosas { get; set; }
                public decimal? LlenadoCapilar { get; set; }
                public int? EscalaDolor { get; set; }
                public decimal? CondicionCorporal { get; set; }
            }

            public class FichaPesoVM
            {
                public DateTime Fecha { get; set; }
                public decimal Peso { get; set; }
            }

            public class FichaLabVM
            {
                public string Examen { get; set; }
                public string Tipo { get; set; }
                public string Estado { get; set; }
                public DateTime? FechaSolicitud { get; set; }
                public DateTime? FechaResultado { get; set; }
                public string Observaciones { get; set; }
            }

            // Un punto de la grafica de tendencia: el valor de un parametro
            // (ej. Glucosa) en una fecha, con su rango de referencia.
            public class FichaLabTendenciaVM
            {
                public string Parametro { get; set; }
                public string Valor { get; set; }     // texto; la vista intenta parsear a numero
                public string Unidad { get; set; }
                public decimal? RangoMin { get; set; }
                public decimal? RangoMax { get; set; }
                public DateTime Fecha { get; set; }
            }

            public class FichaProcedimientoVM
            {
                public string Nombre { get; set; }
                public string Descripcion { get; set; }
                public string RealizadoPor { get; set; }
                public string Anestesia { get; set; }
                public int? DuracionMinutos { get; set; }
                public string Notas { get; set; }
                public DateTime Fecha { get; set; }
                public string ServicioNombre { get; set; }
            }
        }
