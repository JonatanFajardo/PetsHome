using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para el historial médico de una mascota.
    /// </summary>
    public partial class CitaMedicaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la cita médica.
        /// </summary>
        [Key]
        [Display(Name = "Id cita médica")]
        public int medic_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la mascota.
        /// </summary>
        [Display(Name = "Id Mascota ")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int masc_Id { get; set; }
         

        /// <summary>
        /// Obtiene o establece el ID del comportamiento.
        /// </summary>
        [Display(Name = "Comportamiento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int com_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la consulta médica.
        /// </summary>
        [Display(Name = "Fecha de consulta")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime medic_FechaConsulta { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de consulta médica.
        /// </summary>
        [Display(Name = "Tipo de consulta")]
        [StringLength(255)]
        public string medic_TipoConsulta { get; set; }

        /// <summary>
        /// Obtiene o establece el motivo de la consulta médica.
        /// </summary>
        [Display(Name = "Motivo de consulta")]
        [StringLength(255)]
        public string medic_MotivoConsulta { get; set; }

        /// <summary>
        /// Obtiene o establece el diagnóstico de la consulta médica.
        /// </summary>
        [Display(Name = "Diagnóstico")]
        [StringLength(255)]
        public string medic_Diagnostico { get; set; }

        /// <summary>
        /// Obtiene o establece el peso de la mascota en gramos.
        /// </summary>
        [Display(Name = "Peso (gr)")]
        [Range(0, int.MaxValue, ErrorMessage = "El peso debe ser un valor positivo")]
        public int? medic_Peso { get; set; }

        /// <summary>
        /// Obtiene o establece la temperatura de la mascota.
        /// </summary>
        [Display(Name = "Temperatura (°C)")]
        [Range(0, int.MaxValue, ErrorMessage = "La temperatura debe ser un valor positivo")]
        public int? medic_Temperatura { get; set; }

        /// <summary>
        /// Obtiene o establece la frecuencia cardíaca de la mascota.
        /// </summary>
        [Display(Name = "Frecuencia cardíaca")]
        [Range(0, int.MaxValue, ErrorMessage = "La frecuencia cardíaca debe ser un valor positivo")]
        public int? medic_FrecuenciaCardiaca { get; set; }

        /// <summary>
        /// Obtiene o establece la frecuencia respiratoria de la mascota.
        /// </summary>
        [Display(Name = "Frecuencia respiratoria")]
        [Range(0, int.MaxValue, ErrorMessage = "La frecuencia respiratoria debe ser un valor positivo")]
        public int? medic_FrecuenciaRespiratoria { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la vacuna aplicada.
        /// </summary>
        [Display(Name = "Vacuna")]
        public int? vac_Id { get; set; }

        /// <summary>
        /// Obtiene o establece los medicamentos recetados.
        /// </summary>
        [Display(Name = "Medicamentos recetados")]
        [StringLength(255)]
        public string medic_MedicamentosRecetados { get; set; }

        /// <summary>
        /// Obtiene o establece la dosificación de los medicamentos.
        /// </summary>
        [Display(Name = "Dosificación")]
        [StringLength(255)]
        public string medic_Dosificacion { get; set; }

        /// <summary>
        /// Obtiene o establece los procedimientos realizados.
        /// </summary>
        [Display(Name = "Procedimientos realizados")]
        [StringLength(255)]
        public string medic_ProcedimientosRealizados { get; set; }

        /// <summary>
        /// Obtiene o establece los resultados de los exámenes.
        /// </summary>
        [Display(Name = "Resultados de exámenes")]
        [StringLength(255)]
        public string medic_ResultadosExamenes { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la próxima cita.
        /// </summary>
        [Display(Name = "Próxima cita")]
        public DateTime? medic_ProximaCita { get; set; }

        /// <summary>
        /// Obtiene o establece el motivo de la próxima cita.
        /// </summary>
        [Display(Name = "Motivo próxima cita")]
        [StringLength(255)]
        public string medic_MotivoProximaCita { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la cita médica.
        /// </summary>
        public int medic_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la cita médica.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string medic_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la cita médica.
        /// </summary>
        [Display(Name = "Fecha creación")]
        public DateTime medic_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la cita médica.
        /// </summary>
        public int? medic_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la cita médica.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string medic_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la cita médica.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? medic_FechaModifica { get; set; }

        // Propiedades adicionales para mostrar información relacionada
        public string Mascota { get; set; }
        public string Comportamiento { get; set; }
        public string Vacuna { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.medic_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}