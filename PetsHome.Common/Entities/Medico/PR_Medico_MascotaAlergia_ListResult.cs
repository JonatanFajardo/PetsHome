using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_MascotaAlergia_ListResult
    {
        public int Fila { get; set; }
        public int alrg_Id { get; set; }
        public int masc_Id { get; set; }
        public string alrg_Sustancia { get; set; }
        public string alrg_Reaccion { get; set; }
        public string alrg_Gravedad { get; set; }
        public DateTime alrg_FechaCrea { get; set; }
    }
}
