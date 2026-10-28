namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resumen de saldo por farmaco controlado.
    /// Saldo = (Recibido + Devuelto) - (Administrado + Desechado).
    /// </summary>
    public partial class PR_Medico_SustanciaSaldo_ListResult
    {
        public int Fila { get; set; }
        public int itm_Id { get; set; }
        public string itm_Codigo { get; set; }
        public string Farmaco { get; set; }
        public decimal TotalRecibido { get; set; }
        public decimal TotalDevuelto { get; set; }
        public decimal TotalAdministrado { get; set; }
        public decimal TotalDesechado { get; set; }
        public decimal Saldo { get; set; }
        public string DeaSchedule { get; set; }
        public int TotalMovimientos { get; set; }
    }
}
