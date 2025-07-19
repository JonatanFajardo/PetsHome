using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del procedimiento para obtener reporte de eventos
    /// </summary>
    public partial class PR_Reportes_Eventos_Result
    {
        /// <summary>
        /// ID del evento
        /// </summary>
        public int eve_Id { get; set; }

        /// <summary>
        /// Descripción del evento
        /// </summary>
        public string eve_Descripcion { get; set; }

        /// <summary>
        /// Fecha del evento
        /// </summary>
        public DateTime eve_Fecha { get; set; }

        /// <summary>
        /// Hora de inicio del evento
        /// </summary>
        public TimeSpan eve_HoraInicio { get; set; }

        /// <summary>
        /// Hora de finalización del evento
        /// </summary>
        public TimeSpan eve_HoraFinal { get; set; }

        /// <summary>
        /// Nombre del refugio
        /// </summary>
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Total de voluntarios participantes
        /// </summary>
        public int VoluntariosParticipantes { get; set; }

        /// <summary>
        /// Estado del evento (Próximo, En curso, Realizado)
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Fecha de creación del evento
        /// </summary>
        public DateTime eve_FechaCrea { get; set; }
    }
}