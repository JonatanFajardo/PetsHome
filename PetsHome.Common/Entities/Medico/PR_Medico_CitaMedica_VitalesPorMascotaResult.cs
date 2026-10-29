using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_CitaMedica_VitalesPorMascotaResult
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
}
