namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resumen de cuentas por cobrar (A/R) para las tarjetas de /Facturacion.
    /// Replica el arSummary de OpenVPM: Por cobrar, Vencido, Cobrado este mes.
    /// </summary>
    public class PR_Veterinaria_Facturas_ResumenARResult
    {
        public decimal PorCobrar { get; set; }
        public decimal Vencido { get; set; }
        public decimal CobradoMes { get; set; }
    }
}
