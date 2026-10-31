using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Recordatorio / recall clinico (care-reminder). Tarea interna: "a esta
    /// mascota le toca algo en tal fecha". No envia correo ni SMS.
    /// </summary>
    public partial class tbRecordatorios
    {
        public int rec_Id { get; set; }
        public int masc_Id { get; set; }
        public string rec_Tipo { get; set; }
        public DateTime rec_FechaObjetivo { get; set; }
        public string rec_Estado { get; set; }
        public string rec_Nota { get; set; }
        public string rec_Origen { get; set; }
        public DateTime? rec_FechaCompletado { get; set; }
        public bool rec_EsEliminado { get; set; }
        public int rec_UsuarioCrea { get; set; }
        public DateTime rec_FechaCrea { get; set; }
        public int? rec_UsuarioModifica { get; set; }
        public DateTime? rec_FechaModifica { get; set; }
    }
}
