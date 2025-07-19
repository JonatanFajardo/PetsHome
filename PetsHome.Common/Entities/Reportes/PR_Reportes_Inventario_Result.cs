namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del procedimiento para obtener reporte de inventario
    /// </summary>
    public partial class PR_Reportes_Inventario_Result
    {
        /// <summary>
        /// ID del item
        /// </summary>
        public int itm_Id { get; set; }

        /// <summary>
        /// Código del item
        /// </summary>
        public string itm_Codigo { get; set; }

        /// <summary>
        /// Descripción del item
        /// </summary>
        public string itm_Descripcion { get; set; }

        /// <summary>
        /// Descripción de la categoría
        /// </summary>
        public string cat_Descripcion { get; set; }

        /// <summary>
        /// Stock actual del item
        /// </summary>
        public int StockActual { get; set; }

        /// <summary>
        /// Stock mínimo requerido
        /// </summary>
        public int StockMinimo { get; set; }

        /// <summary>
        /// Estado del stock (Crítico, Bajo, Normal)
        /// </summary>
        public string EstadoStock { get; set; }

        /// <summary>
        /// Precio unitario del item
        /// </summary>
        public decimal itm_Precio { get; set; }

        /// <summary>
        /// Valor total del stock (precio * cantidad)
        /// </summary>
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// Nombre del refugio
        /// </summary>
        public string refg_Nombre { get; set; }
    }
}