using PetsHome.Common.Entities;
using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    public class AlertaMedicaViewModel
    {
        public List<PR_Medico_AlertaMedica_VacunasResult> VacunasVencidas { get; set; }
            = new List<PR_Medico_AlertaMedica_VacunasResult>();

        public List<PR_Medico_AlertaMedica_TratamientosResult> TratamientosPorVencer { get; set; }
            = new List<PR_Medico_AlertaMedica_TratamientosResult>();

        public List<PR_Medico_AlertaMedica_RecetasResult> RecetasSinRevision { get; set; }
            = new List<PR_Medico_AlertaMedica_RecetasResult>();

        public List<PR_Medico_AlertaMedica_SinConsultaResult> SinConsulta { get; set; }
            = new List<PR_Medico_AlertaMedica_SinConsultaResult>();

        public int TotalVacunasVencidas => VacunasVencidas?.Count ?? 0;
        public int TotalTratamientosPorVencer => TratamientosPorVencer?.Count ?? 0;
        public int TotalRecetasSinRevision => RecetasSinRevision?.Count ?? 0;
        public int TotalSinConsulta => SinConsulta?.Count ?? 0;
        public int TotalAlertas => TotalVacunasVencidas + TotalTratamientosPorVencer
                                 + TotalRecetasSinRevision + TotalSinConsulta;
    }
}
