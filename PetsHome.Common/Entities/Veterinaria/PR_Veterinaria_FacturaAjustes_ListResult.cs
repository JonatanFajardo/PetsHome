using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Historial de ajustes (nota de credito / reembolso) de una factura.
    /// </summary>
    public class PR_Veterinaria_FacturaAjustes_ListResult
    {
        public int faj_Id { get; set; }
        public int fact_Id { get; set; }
        public string faj_Tipo { get; set; }
        public decimal faj_Monto { get; set; }
        public string faj_Motivo { get; set; }
        public decimal? faj_SaldoDespues { get; set; }
        public DateTime faj_FechaCrea { get; set; }
        public string RegistradoPor { get; set; }
    }
}
