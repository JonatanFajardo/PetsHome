using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_DashboardVeterinario_TratamientosActivosResult
    {
        public int trat_Id { get; set; }
public string masc_Nombre { get; set; }
public string masc_Especie { get; set; }
public string raz_Descripcion { get; set; }
public string trat_Descripcion { get; set; }
public int trat_DiaActual { get; set; }
public int trat_DuracionTotal { get; set; }
public DateTime trat_FechaFin { get; set; }
public string trat_Estado { get; set; }
    }
}
