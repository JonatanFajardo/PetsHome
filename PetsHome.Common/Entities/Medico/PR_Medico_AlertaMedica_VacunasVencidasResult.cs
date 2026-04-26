using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_AlertaMedica_VacunasVencidasResult
    {
        public int masc_Id { get; set; }
public string MascotaNombre { get; set; }
public string Raza { get; set; }
public string Edad { get; set; }
public string VacunaNombre { get; set; }
public DateTime FechaUltimaVacuna { get; set; }
public int DiasVencida { get; set; }
    }
}
