using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_UltimasCitasResult
    {
        public int cita_Id { get; set; }
public DateTime cita_FechaConsulta { get; set; }
public string TipoConsulta { get; set; }
public string cita_Diagnostico { get; set; }
public string Veterinario { get; set; }
    }
}
