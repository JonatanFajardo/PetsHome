using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Fila de la serie diaria de ingresos para el reporte de /Reportes.
    /// Una fila por dia del periodo (Fecha, Monto facturado/cobrado ese dia).
    /// </summary>
    public class PR_Veterinaria_Reportes_IngresosResult
    {
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
    }
}
