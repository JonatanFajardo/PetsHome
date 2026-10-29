using System;

namespace PetsHome.Common.Entities
{
    /// <summary>Un pago/abono de una factura, con el nombre de quien lo recibio.</summary>
    public class PR_Veterinaria_Pago_ListPorFacturaResult
    {
        public int pago_Id { get; set; }
        public decimal pago_Monto { get; set; }
        public string pago_Metodo { get; set; }
        public DateTime pago_Fecha { get; set; }
        public int pago_RecibidoPor { get; set; }
        public string RecibidoPorNombre { get; set; }
        public string pago_Referencia { get; set; }
        public string pago_Notas { get; set; }
    }
}
