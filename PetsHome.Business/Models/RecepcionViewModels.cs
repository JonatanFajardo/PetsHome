using System;
using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// ViewModel del tablero de Recepcion / Sala de espera.
    /// </summary>
    public class RecepcionViewModel
    {
        public DateTime Fecha { get; set; } = DateTime.Today;
        public List<SalaEsperaItemViewModel> Cola { get; set; } = new List<SalaEsperaItemViewModel>();

        public int TotalEnEspera { get; set; }
        public int TotalEnAtencion { get; set; }
    }

    /// <summary>
    /// Fila de la cola de sala de espera (una visita del dia).
    /// </summary>
    public class SalaEsperaItemViewModel
    {
        public int CitaId { get; set; }
        public int MascId { get; set; }
        public string MascotaNombre { get; set; }

        /// <summary>Nombre del dueno (Cliente) o null si el paciente es interno.</summary>
        public string DuenoNombre { get; set; }

        /// <summary>True = paciente interno (refugio/rescate, sin dueno).</summary>
        public bool EsInterno { get; set; }

        public string Motivo { get; set; }
        public string TipoConsulta { get; set; }
        public string Prioridad { get; set; }
        public DateTime? HoraLlegada { get; set; }
        public int MinutosEspera { get; set; }
        public string Estado { get; set; }
        public string VetAsignado { get; set; }
    }

    /// <summary>
    /// Payload del CheckIn que envia recepcion (o el vet auto-sirviendose).
    /// </summary>
    public class CheckInDto
    {
        public int MascId { get; set; }
        public int? TipoConsultaId { get; set; }
        public string Motivo { get; set; }
        public string Prioridad { get; set; }
        public int? VetId { get; set; }
        public decimal? Peso { get; set; }
    }

    /// <summary>
    /// Opcion del dropdown de veterinarios (selector opcional del check-in).
    /// </summary>
    public class VeterinarioOpcionViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    /// <summary>
    /// Resultado del CheckIn.
    /// </summary>
    public class CheckInResultDto
    {
        public bool Ok { get; set; }
        public int CitaId { get; set; }
        public string Mensaje { get; set; }
    }

    /// <summary>
    /// Resultado de un cambio de estado de visita. Propaga el CodeStatus/MessageStatus
    /// del SP, que es quien valida la transicion entre los 7 estados de la cita.
    /// </summary>
    public class CambiarEstadoResultDto
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
    }

    /// <summary>
    /// Los 7 estados de una cita/visita (mismos que OpenVPM), guardados SIN acento.
    /// Fuente unica de verdad en el C#; la validacion de transiciones vive en el SP
    /// [Medico].[PR_Medico_Visita_CambiarEstado].
    /// Flujo: Agendada -> Confirmada -> En espera -> En atencion -> Finalizada.
    /// Ramas: -> No asistio / -> Cancelada. Finalizada / No asistio / Cancelada = finales.
    /// </summary>
    public static class EstadosCita
    {
        public const string Agendada = "Agendada";
        public const string Confirmada = "Confirmada";
        public const string EnEspera = "En espera";
        public const string EnAtencion = "En atencion";
        public const string Finalizada = "Finalizada";
        public const string NoAsistio = "No asistio";
        public const string Cancelada = "Cancelada";

        /// <summary>Los 7 estados en orden de flujo.</summary>
        public static readonly string[] Todos =
        {
            Agendada, Confirmada, EnEspera, EnAtencion, Finalizada, NoAsistio, Cancelada
        };
    }
}
