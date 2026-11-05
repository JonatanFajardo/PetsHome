using System;

namespace PetsHome.Common.Entities
{
    /// <summary>Datos crudos de un procedimiento para pre-llenar el formulario de edicion.</summary>
    public partial class PR_Medico_Procedimiento_FindResult
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
    }
}
