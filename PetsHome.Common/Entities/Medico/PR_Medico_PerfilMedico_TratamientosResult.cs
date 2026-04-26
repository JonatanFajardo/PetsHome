using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_TratamientosResult
    {
        public int trat_Id { get; set; }
public string NombreTratamiento { get; set; }
public string Medicamento { get; set; }
public DateTime trat_FechaInicio { get; set; }
public DateTime? trat_FechaFin { get; set; }
public int PorcentajeCompletado { get; set; }
public string EstadoTratamiento { get; set; }
    }
}
