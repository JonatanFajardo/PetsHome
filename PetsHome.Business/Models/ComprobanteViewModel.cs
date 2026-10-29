using System;
using System.Collections.Generic;
using PetsHome.Common.Entities;

namespace PetsHome.Business.Models
{
    /// <summary>Comprobante / recibo imprimible de una factura (encabezado + lineas + pagos).</summary>
    public class ComprobanteViewModel
    {
        public PR_Veterinaria_Facturas_FindResult Header { get; set; }
        public List<PR_Veterinaria_FacturaDetalle_ListResult> Lineas { get; set; } = new List<PR_Veterinaria_FacturaDetalle_ListResult>();

        /// <summary>Monto pagado acumulado de la factura.</summary>
        public decimal MontoPagado { get; set; }

        /// <summary>Saldo pendiente = Total - MontoPagado.</summary>
        public decimal Saldo { get; set; }

        /// <summary>Historial de pagos/abonos de la factura.</summary>
        public List<PagoComprobanteVM> Pagos { get; set; } = new List<PagoComprobanteVM>();
    }

    /// <summary>Un pago/abono mostrado en el comprobante.</summary>
    public class PagoComprobanteVM
    {
        public decimal Monto { get; set; }
        public string Metodo { get; set; }
        public DateTime Fecha { get; set; }
        public string RecibidoPor { get; set; }
        public string Referencia { get; set; }
    }
}
