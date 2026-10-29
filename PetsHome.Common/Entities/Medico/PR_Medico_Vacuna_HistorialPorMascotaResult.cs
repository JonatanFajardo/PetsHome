using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_Vacuna_HistorialPorMascotaResult
    {
        public string VacunaNombre { get; set; }
        public DateTime? FechaAplicada { get; set; }
        public DateTime? ProximoRefuerzo { get; set; }
        public string Estado { get; set; }
    }
}
