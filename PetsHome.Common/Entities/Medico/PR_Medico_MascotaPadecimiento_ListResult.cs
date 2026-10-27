using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_MascotaPadecimiento_ListResult
    {
        public int Fila { get; set; }
        public int pade_Id { get; set; }
        public int masc_Id { get; set; }
        public string pade_Nombre { get; set; }
        public string pade_Estado { get; set; }
        public DateTime? pade_Desde { get; set; }
        public DateTime? pade_Hasta { get; set; }
        public DateTime pade_FechaCrea { get; set; }
    }
}
