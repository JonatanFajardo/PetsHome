using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del SP [Medico].[PR_Medico_Consulta_FirmaEstado].
    /// Estado de firma (bloqueo) de la nota clinica de una cita.
    /// </summary>
    public class PR_Medico_Consulta_FirmaEstadoResult
    {
        public int cita_Id { get; set; }
        public bool cita_Firmada { get; set; }
        public DateTime? cita_FechaFirma { get; set; }
        public int? cita_FirmadaPor { get; set; }
        public string FirmadaPorNombre { get; set; }
    }
}
