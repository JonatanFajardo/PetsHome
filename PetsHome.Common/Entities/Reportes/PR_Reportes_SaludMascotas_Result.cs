using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del procedimiento para obtener reporte de salud de mascotas
    /// </summary>
    public partial class PR_Reportes_SaludMascotas_Result
    {
        /// <summary>
        /// ID de la mascota
        /// </summary>
        public int masc_Id { get; set; }

        /// <summary>
        /// Nombre de la mascota
        /// </summary>
        public string masc_Nombre { get; set; }

        /// <summary>
        /// Descripción de la raza
        /// </summary>
        public string raza_Descripcion { get; set; }

        /// <summary>
        /// Nombre del refugio
        /// </summary>
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Fecha de la última cita médica
        /// </summary>
        public DateTime? UltimaCitaMedica { get; set; }

        /// <summary>
        /// Estado de salud de la mascota
        /// </summary>
        public string EstadoSalud { get; set; }

        /// <summary>
        /// Total de citas médicas registradas
        /// </summary>
        public int TotalCitas { get; set; }

        /// <summary>
        /// Días desde la última cita médica
        /// </summary>
        public int? DiasSinCita { get; set; }

        /// <summary>
        /// Indica si las vacunas están al día
        /// </summary>
        public bool VacunasAlDia { get; set; }

        /// <summary>
        /// Prioridad de atención (Alta, Media, Baja)
        /// </summary>
        public string PrioridadAtencion { get; set; }

        /// <summary>
        /// Peso actual de la mascota
        /// </summary>
        public decimal? masc_Peso { get; set; }

        /// <summary>
        /// Edad de la mascota
        /// </summary>
        public int? masc_Edad { get; set; }
    }
}