        using PetsHome.Common.Entities;
using System.Collections.Generic;

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

                // ── Conteos calculados ────────────────────────────
                public int TotalFichaMascota => FichaMascota?.Count ?? 0;
        public int TotalUltimasCitas => UltimasCitas?.Count ?? 0;
        public int TotalMedicamentosActivos => MedicamentosActivos?.Count ?? 0;
        public int TotalTodasCitas => TodasCitas?.Count ?? 0;
        public int TotalTratamientos => Tratamientos?.Count ?? 0;
        public int TotalVacunas => Vacunas?.Count ?? 0;
                public int TotalAlertas => TotalFichaMascota + TotalUltimasCitas + TotalMedicamentosActivos + TotalTodasCitas + TotalTratamientos + TotalVacunas;
            }
        }
