using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_FichaMascotaResult
    {
        public int masc_Id { get; set; }
public string masc_Nombre { get; set; }
public string Raza { get; set; }
public string Edad { get; set; }
public string Sexo { get; set; }
public bool EsEsterilizada { get; set; }
public string Microchip { get; set; }
public decimal? Peso { get; set; }
public string Adoptante { get; set; }
public string Refugio { get; set; }
public DateTime? UltimaVisita { get; set; }
public string EstadoSalud { get; set; }
public int TotalCitas { get; set; }
public int TratamientosActivos { get; set; }
public int VacunasAlDia { get; set; }
    }
}
