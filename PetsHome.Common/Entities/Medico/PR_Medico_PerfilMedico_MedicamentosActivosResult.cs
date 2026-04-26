using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_MedicamentosActivosResult
    {
        public int trat_Id { get; set; }
public string Medicamento { get; set; }
public string Dosis { get; set; }
public int? DiasRestantes { get; set; }
public int PorcentajeCompletado { get; set; }
    }
}
