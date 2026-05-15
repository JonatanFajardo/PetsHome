using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_DashboardCuidador_CitasHoyResult
    {
        public int cita_Id { get; set; }
        public string masc_Nombre { get; set; }
        public string masc_Especie { get; set; }
        public DateTime cita_FechaHora { get; set; }
        public string cita_TipoConsulta { get; set; }
        public string cita_Estado { get; set; }
    }
}
