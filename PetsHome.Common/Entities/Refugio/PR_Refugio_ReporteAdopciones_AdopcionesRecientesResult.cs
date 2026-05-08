using System;

namespace PetsHome.Common.Entities
{
    public class PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult
    {
        public string MascotaNombre { get; set; }
public string Raza { get; set; }
public string Adoptante { get; set; }
public DateTime FechaAdopcion { get; set; }
public string Estado { get; set; }
public int DiasTranscurridos { get; set; }
    }
}
