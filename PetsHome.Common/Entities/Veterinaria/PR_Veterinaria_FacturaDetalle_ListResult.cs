namespace PetsHome.Common.Entities
{
    public class PR_Veterinaria_FacturaDetalle_ListResult
    {
        public int fdet_Id { get; set; }
        public int fact_Id { get; set; }
        public string fdet_Tipo { get; set; }
        public int? serv_Id { get; set; }
        public int? itm_Id { get; set; }
        public string fdet_Descripcion { get; set; }
        public decimal fdet_Cantidad { get; set; }
        public decimal fdet_PrecioUnit { get; set; }
        public decimal fdet_Subtotal { get; set; }
    }
}
