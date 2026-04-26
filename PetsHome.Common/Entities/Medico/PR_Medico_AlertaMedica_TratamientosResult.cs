using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_AlertaMedica_TratamientosResult
    {
        public int trat_Id { get; set; }
        public int masc_Id { get; set; }
        public string MascotaNombre { get; set; }
        public string Raza { get; set; }
        public string Edad { get; set; }
        public string TratamientoNombre { get; set; }
        public DateTime? trat_ProximaDosis { get; set; }
        public int DiasRestantes { get; set; }
        public int PorcentajeRestante { get; set; }
    }
}
