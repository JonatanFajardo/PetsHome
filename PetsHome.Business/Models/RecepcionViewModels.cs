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
}
