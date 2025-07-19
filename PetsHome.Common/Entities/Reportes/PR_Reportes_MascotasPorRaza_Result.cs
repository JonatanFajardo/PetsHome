namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del procedimiento para obtener reporte de mascotas por raza
    /// </summary>
    public partial class PR_Reportes_MascotasPorRaza_Result
    {
        /// <summary>
        /// ID de la raza
        /// </summary>
        public int raza_Id { get; set; }

        /// <summary>
        /// Descripción de la raza
        /// </summary>
        public string raza_Descripcion { get; set; }

        /// <summary>
        /// Total de mascotas de esta raza
        /// </summary>
        public int TotalMascotas { get; set; }

        /// <summary>
        /// Total de mascotas adoptadas de esta raza
        /// </summary>
        public int MascotasAdoptadas { get; set; }

        /// <summary>
        /// Total de mascotas disponibles de esta raza
        /// </summary>
        public int MascotasDisponibles { get; set; }

        /// <summary>
        /// Porcentaje de adopción de esta raza
        /// </summary>
        public decimal PorcentajeAdopcion { get; set; }
    }
}