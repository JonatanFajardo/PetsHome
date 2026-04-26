using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_VacunasResult
    {
        public int vac_Id { get; set; }
public string VacunaNombre { get; set; }
public DateTime? FechaAplicada { get; set; }
public DateTime? FechaProxima { get; set; }
public string EstadoVacuna { get; set; }
    }
}
