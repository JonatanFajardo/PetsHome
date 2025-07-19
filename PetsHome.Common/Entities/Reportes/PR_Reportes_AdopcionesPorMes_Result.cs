namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del procedimiento para obtener reporte de adopciones por mes
    /// </summary>
    public partial class PR_Reportes_AdopcionesPorMes_Result
    {
        /// <summary>
        /// Año de la adopción
        /// </summary>
        public int Año { get; set; }

        /// <summary>
        /// Mes de la adopción (número)
        /// </summary>
        public int Mes { get; set; }

        /// <summary>
        /// Nombre del mes
        /// </summary>
        public string NombreMes { get; set; }

        /// <summary>
        /// Total de adopciones en el mes
        /// </summary>
        public int TotalAdopciones { get; set; }

        /// <summary>
        /// Año y mes en formato texto para gráficos
        /// </summary>
        public string Periodo { get; set; }
    }
}