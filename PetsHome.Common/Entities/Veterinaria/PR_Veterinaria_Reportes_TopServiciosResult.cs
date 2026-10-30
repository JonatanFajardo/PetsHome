namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Fila del ranking de servicios mas usados en el periodo para /Reportes.
    /// (Servicio, Cantidad de veces, Ingreso total generado).
    /// </summary>
    public class PR_Veterinaria_Reportes_TopServiciosResult
    {
        public string Servicio { get; set; }
        public int Cantidad { get; set; }
        public decimal Ingreso { get; set; }
    }
}
