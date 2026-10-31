using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    // ─── Payload de creacion de factura (POS) ─────────────
    public class FacturaCrearViewModel
    {
        public int? cliId { get; set; }
        public int? mascId { get; set; }
        public decimal descuento { get; set; }   // descuento/ajuste a nivel factura
        public List<FacturaLineaViewModel> lineas { get; set; } = new List<FacturaLineaViewModel>();
    }

    public class FacturaLineaViewModel
    {
        public string tipo { get; set; }        // "Servicio" | "Producto"
        public int? servId { get; set; }
        public int? itmId { get; set; }
        public string descripcion { get; set; }
        public decimal cantidad { get; set; }
        public decimal precioUnit { get; set; }
        public decimal descuento { get; set; }  // descuento (monto) de la linea
        public bool? exento { get; set; }        // si es exento de impuesto (null -> lo decide el catalogo)
    }

    // ─── Payload para aplicar nota de credito / reembolso ─────────
    public class FacturaAjusteViewModel
    {
        public int factId { get; set; }
        public string tipo { get; set; }         // "NotaCredito" | "Reembolso"
        public decimal monto { get; set; }
        public string motivo { get; set; }
        public string claveOperacion { get; set; }
    }

    // ─── Resultado de la operacion de creacion ────────────
    public class FacturaResultViewModel
    {
        public bool success { get; set; }
        public int? factId { get; set; }
        public string message { get; set; }
    }
}
