using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del procedimiento para obtener reporte de voluntarios
    /// </summary>
    public partial class PR_Reportes_Voluntarios_Result
    {
        /// <summary>
        /// ID del voluntario
        /// </summary>
        public int vol_Id { get; set; }

        /// <summary>
        /// ID de la persona
        /// </summary>
        public int per_Id { get; set; }

        /// <summary>
        /// Nombre completo del voluntario
        /// </summary>
        public string NombreCompleto { get; set; }

        /// <summary>
        /// Primer nombre
        /// </summary>
        public string per_PrimerNombre { get; set; }

        /// <summary>
        /// Apellido paterno
        /// </summary>
        public string per_ApellidoPaterno { get; set; }

        /// <summary>
        /// Teléfono del voluntario
        /// </summary>
        public string per_Telefono { get; set; }

        /// <summary>
        /// Correo electrónico del voluntario
        /// </summary>
        public string per_Correo { get; set; }

        /// <summary>
        /// Total de eventos en los que ha participado
        /// </summary>
        public int EventosParticipados { get; set; }

        /// <summary>
        /// Total de horas trabajadas
        /// </summary>
        public int vol_HorasTrabajadas { get; set; }

        /// <summary>
        /// Fecha de la última participación en un evento
        /// </summary>
        public DateTime? UltimaParticipacion { get; set; }

        /// <summary>
        /// Estado del voluntario (Activo/Inactivo)
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Es voluntario recurrente
        /// </summary>
        public bool vol_Recurrente { get; set; }
    }
}