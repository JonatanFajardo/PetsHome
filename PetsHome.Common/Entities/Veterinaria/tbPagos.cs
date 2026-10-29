using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Pago / abono recibido sobre una factura del cliente (cobro por la atencion
    /// de su mascota). Espejo de OpenVPM payments. Permite abonos parciales:
    /// la factura se salda cuando la suma de pagos cubre fact_Total.
    /// Metodos: Efectivo | TarjetaCredito | TarjetaDebito | Cheque | Transferencia | Otro.
    /// </summary>
    public partial class tbPagos
    {
        public int pago_Id { get; set; }
        public int fact_Id { get; set; }
        public decimal pago_Monto { get; set; }
        public string pago_Metodo { get; set; }
        public int pago_RecibidoPor { get; set; }
        public DateTime pago_Fecha { get; set; }
        public string pago_Referencia { get; set; }
        public string pago_Notas { get; set; }

        public bool pago_EsEliminado { get; set; }
        public int pago_UsuarioCrea { get; set; }
        public DateTime pago_FechaCrea { get; set; }
        public int? pago_UsuarioModifica { get; set; }
        public DateTime? pago_FechaModifica { get; set; }
    }
}
