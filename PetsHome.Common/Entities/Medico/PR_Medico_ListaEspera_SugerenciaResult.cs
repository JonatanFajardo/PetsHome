using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_ListaEspera_SugerenciaResult
    {
        public int Fila { get; set; }
        public int lesp_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string Telefono { get; set; }
        public int? tipoCon_Id { get; set; }
        public string TipoConsulta { get; set; }
        public DateTime? lesp_FechaDeseada { get; set; }
        public string lesp_Prioridad { get; set; }
        public DateTime lesp_FechaCrea { get; set; }
        public int DiasEsperando { get; set; }
    }
}
