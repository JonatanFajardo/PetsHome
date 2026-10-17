using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Fila de la cola de sala de espera ([Medico].[PR_Medico_SalaEspera_List]).
    /// Paciente BINARIO: DuenoNombre es null cuando EsInterno = true.
    /// </summary>
    public class PR_Medico_SalaEspera_ListResult
    {
        public int cita_Id { get; set; }
        public int masc_Id { get; set; }
        public string MascotaNombre { get; set; }
        public string DuenoNombre { get; set; }
        public bool EsInterno { get; set; }
        public string Motivo { get; set; }
        public string TipoConsulta { get; set; }
        public string Prioridad { get; set; }
        public DateTime? HoraLlegada { get; set; }
        public int MinutosEspera { get; set; }
        public string Estado { get; set; }
        public string VetAsignado { get; set; }
    }
}
