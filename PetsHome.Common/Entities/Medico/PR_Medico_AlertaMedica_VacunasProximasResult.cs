using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_AlertaMedica_VacunasProximasResult
    {
        public string MascotaNombre { get; set; }
        public int masc_Id { get; set; }
        public string VacunaNombre { get; set; }
        public DateTime? FechaAplicada { get; set; }
        public DateTime? ProximoRefuerzo { get; set; }
        public string Estado { get; set; }
    }
}
