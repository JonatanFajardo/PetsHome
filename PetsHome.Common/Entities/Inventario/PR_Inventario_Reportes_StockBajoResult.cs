namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Fila de items con stock por debajo del minimo para /Reportes.
    /// (Item, Stock actual, Minimo configurado). Stock/Minimo son decimales
    /// para alinearse con el inventario (itm_StockActual / itm_StockMinimo).
    /// </summary>
    public class PR_Inventario_Reportes_StockBajoResult
    {
        public string Item { get; set; }
        public decimal Stock { get; set; }
        public decimal Minimo { get; set; }
    }
}
