using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_Laboratorio_PorMascotaResult
    {
        public string Examen { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaResultado { get; set; }
        public string Observaciones { get; set; }
    }
}
