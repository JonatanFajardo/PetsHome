using System;

namespace PetsHome.Common.Entities
{
    public partial class tbConsentimiento
    {
        public int cons_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cli_Id { get; set; }
        public int? cita_Id { get; set; }
        public string cons_Tipo { get; set; }
        public string cons_Estado { get; set; }
        public string cons_Texto { get; set; }
        public string cons_FirmanteNombre { get; set; }
        public DateTime? cons_FechaFirma { get; set; }
        public string cons_Notas { get; set; }
        public bool cons_EsEliminado { get; set; }
        public int cons_UsuarioCrea { get; set; }
        public DateTime cons_FechaCrea { get; set; }
        public int? cons_UsuarioModifica { get; set; }
        public DateTime? cons_FechaModifica { get; set; }
    }
}
