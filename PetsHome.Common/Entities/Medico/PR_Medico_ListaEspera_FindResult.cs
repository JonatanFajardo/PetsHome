using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_ListaEspera_FindResult
    {
        public int lesp_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cli_Id { get; set; }
        public int? tipoCon_Id { get; set; }
        public DateTime? lesp_FechaDeseada { get; set; }
        public string lesp_Prioridad { get; set; }
        public string lesp_Estado { get; set; }
        public string lesp_Notas { get; set; }
    }
}
