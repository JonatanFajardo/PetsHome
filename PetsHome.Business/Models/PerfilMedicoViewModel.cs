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

                // ── Conteos calculados ────────────────────────────
                public int TotalFichaMascota => FichaMascota?.Count ?? 0;
        public int TotalUltimasCitas => UltimasCitas?.Count ?? 0;
        public int TotalMedicamentosActivos => MedicamentosActivos?.Count ?? 0;
        public int TotalTodasCitas => TodasCitas?.Count ?? 0;
        public int TotalTratamientos => Tratamientos?.Count ?? 0;
        public int TotalVacunas => Vacunas?.Count ?? 0;
        public int TotalFacturas => Facturas?.Count ?? 0;
        public int TotalLaboratorios => Laboratorios?.Count ?? 0;
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
        }
