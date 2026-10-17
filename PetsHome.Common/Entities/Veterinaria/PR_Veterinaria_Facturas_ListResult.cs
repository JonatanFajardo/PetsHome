using System;

namespace PetsHome.Common.Entities
{
    public class PR_Veterinaria_Facturas_ListResult
    {
        public int Fila { get; set; }
        public int fact_Id { get; set; }
        public string fact_Numero { get; set; }
        public int? cli_Id { get; set; }
        public string Cliente { get; set; }
        public int? masc_Id { get; set; }
        public string Mascota { get; set; }
        public DateTime fact_Fecha { get; set; }
        public decimal fact_Subtotal { get; set; }
        public decimal fact_Impuesto { get; set; }
        public decimal fact_Total { get; set; }
        public string fact_Estado { get; set; }
    }
}
