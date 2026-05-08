using System;

namespace PetsHome.Common.Entities
{
    public class PR_Supervisor_Dashboard_MovimientosInventarioResult
    {
        public int recep_Id { get; set; }
        public string recep_Descripcion { get; set; }
        public DateTime recep_Fecha { get; set; }
        public string recep_NumeroDocumento { get; set; }
        public string mov_PrimerItem { get; set; }
        public int? mov_TotalUnidades { get; set; }
        public string mov_Refugio { get; set; }
    }
}
