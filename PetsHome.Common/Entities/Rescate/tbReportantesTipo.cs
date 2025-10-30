using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Catálogo de tipos de reportantes (Ciudadano, Policía, Bomberos, etc.)
    /// </summary>
    public partial class tbReportantesTipo
    {
        public tbReportantesTipo()
        {
            tbReportesAbandono = new HashSet<tbReportesAbandono>();
        }

        /// <summary>
        /// Identificador único del tipo de reportante
        /// </summary>
        public int reptip_Id { get; set; }

        /// <summary>
        /// Descripción del tipo de reportante
        /// </summary>
        public string reptip_Descripcion { get; set; }

        /// <summary>
        /// Indica si el tipo de reportante está activo
        /// </summary>
        public bool reptip_EsActivo { get; set; }

        /// <summary>
        /// Indica si el registro está eliminado lógicamente
        /// </summary>
        public bool reptip_EsEliminado { get; set; }

        /// <summary>
        /// Usuario que creó el registro
        /// </summary>
        public int reptip_UsuarioCrea { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime reptip_FechaCrea { get; set; }

        /// <summary>
        /// Usuario que modificó el registro
        /// </summary>
        public int? reptip_UsuarioModifica { get; set; }

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime? reptip_FechaModifica { get; set; }

        // Navigation properties
        public virtual tbUsuarios reptip_UsuarioCreaNavigation { get; set; }
        public virtual tbUsuarios reptip_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbReportesAbandono> tbReportesAbandono { get; set; }
    }
}
