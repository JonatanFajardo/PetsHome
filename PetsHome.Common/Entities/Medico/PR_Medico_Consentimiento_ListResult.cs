using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_Consentimiento_ListResult
    {
        public int Fila { get; set; }
        public int cons_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public int? cita_Id { get; set; }
        public string cons_Tipo { get; set; }
        public string cons_Estado { get; set; }
        public string cons_FirmanteNombre { get; set; }
        public DateTime? cons_FechaFirma { get; set; }
        public DateTime cons_FechaCrea { get; set; }
    }
}
