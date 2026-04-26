using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_AlertaMedica_SinConsultaResult
    {
        public int masc_Id { get; set; }
        public string MascotaNombre { get; set; }
        public string Raza { get; set; }
        public string Edad { get; set; }
        public DateTime? UltimaVisita { get; set; }
        public string TiempoSinConsulta { get; set; }
    }
}
