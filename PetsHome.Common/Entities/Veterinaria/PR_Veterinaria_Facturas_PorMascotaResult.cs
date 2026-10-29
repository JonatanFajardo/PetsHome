using System;

namespace PetsHome.Common.Entities
{
    public class PR_Veterinaria_Facturas_PorMascotaResult
    {
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public decimal Saldo { get; set; }
        public string Estado { get; set; }
    }
}
