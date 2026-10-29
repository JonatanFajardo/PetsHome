using System.Collections.Generic;
using PetsHome.Common.Entities;

namespace PetsHome.Business.Models
{
    /// <summary>Comprobante / recibo imprimible de una factura (encabezado + líneas).</summary>
    public class ComprobanteViewModel
    {
        public PR_Veterinaria_Facturas_FindResult Header { get; set; }
        public List<PR_Veterinaria_FacturaDetalle_ListResult> Lineas { get; set; } = new List<PR_Veterinaria_FacturaDetalle_ListResult>();
    }
}
