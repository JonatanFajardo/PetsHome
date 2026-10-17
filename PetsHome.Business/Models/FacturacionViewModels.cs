using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    // ─── Payload de creacion de factura (POS) ─────────────
    public class FacturaCrearViewModel
    {
        public int? cliId { get; set; }
        public int? mascId { get; set; }
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
    }

    // ─── Resultado de la operacion de creacion ────────────
    public class FacturaResultViewModel
    {
        public bool success { get; set; }
        public int? factId { get; set; }
        public string message { get; set; }
    }
}
