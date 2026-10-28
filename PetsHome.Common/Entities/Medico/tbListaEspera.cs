using System;

namespace PetsHome.Common.Entities
{
    public partial class tbListaEspera
    {
        public int lesp_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cli_Id { get; set; }
        public int? tipoCon_Id { get; set; }
        public DateTime? lesp_FechaDeseada { get; set; }
        public string lesp_Prioridad { get; set; }
        public string lesp_Estado { get; set; }
        public string lesp_Notas { get; set; }
        public bool lesp_EsEliminado { get; set; }
        public int lesp_UsuarioCrea { get; set; }
        public DateTime lesp_FechaCrea { get; set; }
        public int? lesp_UsuarioModifica { get; set; }
        public DateTime? lesp_FechaModifica { get; set; }
    }
}
