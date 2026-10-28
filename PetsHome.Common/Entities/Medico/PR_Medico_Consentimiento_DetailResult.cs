using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_Consentimiento_DetailResult
    {
        public int cons_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string ResponsableContacto { get; set; }
        public int? cita_Id { get; set; }
        public string cons_Tipo { get; set; }
        public string cons_Estado { get; set; }
        public string cons_Texto { get; set; }
        public string cons_FirmanteNombre { get; set; }
        public DateTime? cons_FechaFirma { get; set; }
        public string cons_Notas { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime cons_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? cons_FechaModifica { get; set; }
    }
}
