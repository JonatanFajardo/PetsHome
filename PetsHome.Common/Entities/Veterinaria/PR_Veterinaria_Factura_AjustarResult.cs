namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado de aplicar una nota de credito o reembolso a una factura.
    /// </summary>
    public class PR_Veterinaria_Factura_AjustarResult
    {
        public int? faj_Id { get; set; }
        public decimal nuevo_saldo { get; set; }
        public string fact_Estado { get; set; }
        public int CodeStatus { get; set; }
        public string MessageStatus { get; set; }
    }
}
