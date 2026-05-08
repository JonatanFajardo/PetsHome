using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_DashboardVeterinario_AgendaHoyResult
    {
        public int cita_Id { get; set; }
public string masc_Nombre { get; set; }
public string masc_Especie { get; set; }
public string raz_Descripcion { get; set; }
public DateTime cita_FechaHora { get; set; }
public string cita_TipoConsulta { get; set; }
public string cita_Estado { get; set; }
public bool cita_EsUrgente { get; set; }
    }
}
