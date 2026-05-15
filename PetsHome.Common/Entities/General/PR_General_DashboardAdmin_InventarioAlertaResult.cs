namespace PetsHome.Common.Entities
{
    public class PR_General_DashboardAdmin_InventarioAlertaResult
    {
        public string Descripcion  { get; set; }
        public string Categoria    { get; set; }
        public int      StockActual  { get; set; }
        public decimal  StockMinimo  { get; set; }
        public string Estado       { get; set; }
    }
}
