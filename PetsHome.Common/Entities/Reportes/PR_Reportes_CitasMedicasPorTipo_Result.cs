namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del procedimiento para obtener reporte de citas médicas por tipo
    /// </summary>
    public partial class PR_Reportes_CitasMedicasPorTipo_Result
    {
        /// <summary>
        /// Tipo de consulta médica
        /// </summary>
        public string medic_TipoConsulta { get; set; }

        /// <summary>
        /// Total de citas de este tipo
        /// </summary>
        public int TotalCitas { get; set; }

        /// <summary>
        /// Porcentaje que representa este tipo de cita
        /// </summary>
        public decimal PorcentajeCitas { get; set; }
    }
}