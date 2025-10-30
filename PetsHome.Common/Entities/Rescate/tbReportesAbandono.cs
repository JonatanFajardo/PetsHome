using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Registros de reportes de animales abandonados
    /// </summary>
    public partial class tbReportesAbandono
    {
        public tbReportesAbandono()
        {
            tbIngresos = new HashSet<tbIngresos>();
        }

        /// <summary>
        /// Identificador único del reporte de abandono
        /// </summary>
        public int repa_Id { get; set; }

        /// <summary>
        /// ID del tipo de reportante
        /// </summary>
        public int reptip_Id { get; set; }

        /// <summary>
        /// Nombre del reportante (NULL si es anónimo)
        /// </summary>
        public string repa_NombreReportante { get; set; }

        /// <summary>
        /// Teléfono del reportante
        /// </summary>
        public string repa_TelefonoContacto { get; set; }

        /// <summary>
        /// Email del reportante
        /// </summary>
        public string repa_Email { get; set; }

        /// <summary>
        /// Fecha del reporte
        /// </summary>
        public DateTime repa_FechaReporte { get; set; }

        /// <summary>
        /// Lugar donde se encuentra el animal reportado
        /// </summary>
        public string repa_UbicacionIncidente { get; set; }

        /// <summary>
        /// Descripción del animal reportado
        /// </summary>
        public string repa_DescripcionAnimal { get; set; }

        /// <summary>
        /// Estado de atención del reporte (Pendiente, En Proceso, Atendido)
        /// </summary>
        public string repa_EstadoAtencion { get; set; }

        /// <summary>
        /// Observaciones adicionales
        /// </summary>
        public string repa_Observaciones { get; set; }

        /// <summary>
        /// Indica si el reporte es anónimo
        /// </summary>
        public bool repa_EsAnonimo { get; set; }

        /// <summary>
        /// ID del refugio al que se asigna el reporte
        /// </summary>
        public int refg_Id { get; set; }

        /// <summary>
        /// Indica si el registro está eliminado lógicamente
        /// </summary>
        public bool repa_EsEliminado { get; set; }

        /// <summary>
        /// Usuario que creó el registro
        /// </summary>
        public int repa_UsuarioCrea { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime repa_FechaCrea { get; set; }

        /// <summary>
        /// Usuario que modificó el registro
        /// </summary>
        public int? repa_UsuarioModifica { get; set; }

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime? repa_FechaModifica { get; set; }

        // Navigation properties
        public virtual tbReportantesTipo reptip { get; set; }
        public virtual tbRefugios refg { get; set; }
        public virtual tbUsuarios repa_UsuarioCreaNavigation { get; set; }
        public virtual tbUsuarios repa_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbIngresos> tbIngresos { get; set; }
    }
}
