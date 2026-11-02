using System;

namespace PetsHome.Common.Entities
{
    // Una fila = un parametro de laboratorio medido en una fecha,
    // con su rango de referencia, para graficar la evolucion.
    public class PR_Medico_Laboratorio_TendenciaResult
    {
        public string Parametro { get; set; }
        public string Valor { get; set; }          // puede ser texto; se parsea a numero cuando se pueda
        public string Unidad { get; set; }
        public decimal? RangoMin { get; set; }
        public decimal? RangoMax { get; set; }
        public DateTime FechaResultado { get; set; }
    }
}
