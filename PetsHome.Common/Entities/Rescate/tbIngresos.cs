using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Registros de ingresos de animales al refugio
    /// </summary>
    public partial class tbIngresos
    {
        public tbIngresos()
        {
            tbMascotas = new HashSet<tbMascotas>();
        }

        /// <summary>
        /// Identificador único del ingreso
        /// </summary>
        public int ingr_Id { get; set; }

        /// <summary>
        /// ID del reporte de abandono (NULL si el ingreso no viene de un reporte)
        /// </summary>
        public int? repa_Id { get; set; }

        /// <summary>
        /// ID del refugio al que ingresa el animal
        /// </summary>
        public int refg_Id { get; set; }

        /// <summary>
        /// Fecha y hora del ingreso
        /// </summary>
        public DateTime ingr_FechaIngreso { get; set; }

        /// <summary>
        /// Lugar exacto donde se rescató al animal
        /// </summary>
        public string ingr_LugarRescate { get; set; }

        /// <summary>
        /// Condición del animal al momento del ingreso
        /// </summary>
        public string ingr_CondicionInicial { get; set; }

        /// <summary>
        /// Nombre de la persona que rescató al animal
        /// </summary>
        public string ingr_PersonaRescatista { get; set; }

        /// <summary>
        /// Medio de transporte utilizado (ambulancia, vehículo particular, etc.)
        /// </summary>
        public string ingr_MedioTransporte { get; set; }

        /// <summary>
        /// Observaciones adicionales del ingreso
        /// </summary>
        public string ingr_Observaciones { get; set; }

        /// <summary>
        /// Indica si el ingreso es una emergencia
        /// </summary>
        public bool ingr_EsEmergencia { get; set; }

        /// <summary>
        /// Indica si el registro está eliminado lógicamente
        /// </summary>
        public bool ingr_EsEliminado { get; set; }

        /// <summary>
        /// Usuario que creó el registro
        /// </summary>
        public int ingr_UsuarioCrea { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime ingr_FechaCrea { get; set; }

        /// <summary>
        /// Usuario que modificó el registro
        /// </summary>
        public int? ingr_UsuarioModifica { get; set; }

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime? ingr_FechaModifica { get; set; }

        // Navigation properties
        public virtual tbReportesAbandono repa { get; set; }
        public virtual tbRefugios refg { get; set; }
        public virtual tbUsuarios ingr_UsuarioCreaNavigation { get; set; }
        public virtual tbUsuarios ingr_UsuarioModificaNavigation { get; set; }
        public virtual ICollection<tbMascotas> tbMascotas { get; set; }
    }
}
