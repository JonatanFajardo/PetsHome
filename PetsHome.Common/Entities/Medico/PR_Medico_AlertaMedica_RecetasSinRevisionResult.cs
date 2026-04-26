using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_AlertaMedica_RecetasSinRevisionResult
    {
        public int receta_Id { get; set; }
public int masc_Id { get; set; }
public string MascotaNombre { get; set; }
public string Raza { get; set; }
public string receta_Medicamento { get; set; }
public string receta_Estado { get; set; }
public int DuracionDias { get; set; }
    }
}
