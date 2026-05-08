using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_ControlVacunacion_MatrizVacunacionResult
    {
        public int masc_Id { get; set; }
public string masc_Nombre { get; set; }
public string masc_Especie { get; set; }
public string masc_Raza { get; set; }
public string masc_Refugio { get; set; }
public int vac_Id { get; set; }
public string vac_Nombre { get; set; }
public string cvac_Estado { get; set; }
public DateTime? cvac_FechaAplicacion { get; set; }
public DateTime? cvac_FechaVencimiento { get; set; }
    }
}
