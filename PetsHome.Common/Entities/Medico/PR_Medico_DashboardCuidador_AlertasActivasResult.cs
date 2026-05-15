using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_DashboardCuidador_AlertasActivasResult
    {
        public string alert_Tipo { get; set; }
        public string alert_Descripcion { get; set; }
        public string alert_MascotaNombre { get; set; }
        public string alert_Detalle { get; set; }
        public DateTime? alert_FechaRef { get; set; }
    }
}
