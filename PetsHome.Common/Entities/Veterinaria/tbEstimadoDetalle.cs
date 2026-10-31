using System;

namespace PetsHome.Common.Entities
{
    public partial class tbEstimadoDetalle
    {
        public int estd_Id { get; set; }
        public int est_Id { get; set; }
        public string estd_Tipo { get; set; }
        public int? serv_Id { get; set; }
        public int? itm_Id { get; set; }
        public string estd_Descripcion { get; set; }
        public decimal estd_Cantidad { get; set; }
        public decimal estd_PrecioUnit { get; set; }
        public decimal estd_Subtotal { get; set; }
        public bool estd_EsEliminado { get; set; }
        public int estd_UsuarioCrea { get; set; }
        public DateTime estd_FechaCrea { get; set; }
        public int? estd_UsuarioModifica { get; set; }
        public DateTime? estd_FechaModifica { get; set; }
    }
}
