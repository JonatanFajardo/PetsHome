using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_ListaEspera_DetailResult
    {
        public int lesp_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string ResponsableContacto { get; set; }
        public int? tipoCon_Id { get; set; }
        public string TipoConsulta { get; set; }
        public DateTime? lesp_FechaDeseada { get; set; }
        public string lesp_Prioridad { get; set; }
        public string lesp_Estado { get; set; }
        public string lesp_Notas { get; set; }
        public int DiasEsperando { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime lesp_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? lesp_FechaModifica { get; set; }
    }
}
