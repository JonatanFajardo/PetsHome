using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del procedimiento para obtener métricas del dashboard de reportes
    /// </summary>
    public partial class PR_Reportes_Dashboard_Result
    {
        /// <summary>
        /// Total de mascotas en el sistema
        /// </summary>
        public int TotalMascotas { get; set; }

        /// <summary>
        /// Total de mascotas adoptadas
        /// </summary>
        public int MascotasAdoptadas { get; set; }

        /// <summary>
        /// Total de mascotas disponibles
        /// </summary>
        public int MascotasDisponibles { get; set; }

        /// <summary>
        /// Total de citas médicas pendientes
        /// </summary>
        public int CitasMedicasPendientes { get; set; }

        /// <summary>
        /// Total de voluntarios activos
        /// </summary>
        public int VoluntariosActivos { get; set; }

        /// <summary>
        /// Total de eventos en el mes actual
        /// </summary>
        public int EventosEsteMes { get; set; }

        /// <summary>
        /// Porcentaje de adopciones exitosas
        /// </summary>
        public decimal PorcentajeAdopciones { get; set; }
    }
}