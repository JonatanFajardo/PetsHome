using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Inventario_Items_PorVencerResult
    {
        public int itm_Id { get; set; }
        public string itm_Codigo { get; set; }
        public string itm_Descripcion { get; set; }
        public string cat_Descripcion { get; set; }
        public string recdet_NumeroLote { get; set; }
        public decimal recdet_Cantidad { get; set; }
        public DateTime recdet_FechaVencimiento { get; set; }
        public int DiasRestantes { get; set; }
    }
}
