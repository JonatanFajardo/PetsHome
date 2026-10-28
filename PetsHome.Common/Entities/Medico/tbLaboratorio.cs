using System;

namespace PetsHome.Common.Entities
{
    public partial class tbLaboratorio
    {
        public int lab_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cita_Id { get; set; }
        public string lab_Examen { get; set; }
        public string lab_Tipo { get; set; }
        public string lab_Estado { get; set; }
        public DateTime lab_FechaSolicitud { get; set; }
        public DateTime? lab_FechaResultado { get; set; }
        public int? lab_Veterinario { get; set; }
        public string lab_Observaciones { get; set; }
        public bool lab_EsEliminado { get; set; }
        public int lab_UsuarioCrea { get; set; }
        public DateTime lab_FechaCrea { get; set; }
        public int? lab_UsuarioModifica { get; set; }
        public DateTime? lab_FechaModifica { get; set; }
    }
}
