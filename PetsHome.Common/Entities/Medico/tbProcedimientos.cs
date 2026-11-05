using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Procedimiento clinico estructurado (adaptado de OpenVPM "procedures").
    /// Reemplaza al texto libre Medico.tbCitaMedica.cita_ProcedimientosRealizados
    /// (aditivo: el texto libre sigue existiendo por compatibilidad).
    /// Si serv_Id esta ligado, es un item facturable de la visita.
    /// </summary>
    public partial class tbProcedimientos
    {
        public int proc_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cita_Id { get; set; }
        public string proc_Nombre { get; set; }
        public string proc_Descripcion { get; set; }
        public int? proc_RealizadoPor { get; set; }
        public string proc_Anestesia { get; set; }
        public int? proc_DuracionMinutos { get; set; }
        public string proc_Notas { get; set; }
        public DateTime proc_Fecha { get; set; }
        public int? serv_Id { get; set; }

        public bool proc_EsEliminado { get; set; }
        public int proc_UsuarioCrea { get; set; }
        public DateTime proc_FechaCrea { get; set; }
        public int? proc_UsuarioModifica { get; set; }
        public DateTime? proc_FechaModifica { get; set; }
    }
}
