namespace PetsHome.Common.Entities
{
    public class PR_Veterinaria_EstimadoDetalle_ListResult
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
    }
}
