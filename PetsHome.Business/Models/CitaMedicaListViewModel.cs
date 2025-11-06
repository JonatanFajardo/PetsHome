using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para el historial médico de una mascota.
    /// </summary>
    public partial class CitaMedicaListViewModel
    {

        //public CitaMedicaViewModel()
        //{
        //    //ComportamientoList = new SelectList(new List<ComportamientoViewModel>(), "Com_Id", "Com_Nombre");
        //}
        /// <summary>
        /// Obtiene o establece el ID de la cita médica.
        /// </summary>
        [Key]
        [Display(Name = "Id cita médica")]
        public int cita_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la mascota.
        /// </summary>
        [Display(Name = "Id Mascota ")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int masc_Id { get; set; }

        public string masc_Nombre { get; set; }


        /// <summary>
        /// Obtiene o establece el ID del comportamiento.
        /// </summary>
        [Display(Name = "Comportamiento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Com_Id { get; set; }

        public string Com_Nombre { get; set; }


        /// <summary>
        /// Obtiene o establece la fecha de la consulta médica.
        /// </summary>
        [Display(Name = "Fecha de consulta")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime cita_FechaConsulta { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de consulta médica.
        /// </summary>
        [Display(Name = "Tipo de consulta")]
        [StringLength(255)]
        public string cita_TipoConsulta { get; set; }

        /// <summary>
        /// Obtiene o establece el motivo de la consulta médica.
        /// </summary>
        [Display(Name = "Motivo de consulta")]
        [StringLength(255)]
        public string cita_MotivoConsulta { get; set; }

        /// <summary>
        /// Obtiene o establece el diagnóstico de la consulta médica.
        /// </summary>
        [Display(Name = "Diagnóstico")]
        [StringLength(255)]
        public string cita_Diagnostico { get; set; }

        /// <summary>
        /// Obtiene o establece el peso de la mascota en gramos.
        /// </summary>
        [Display(Name = "Peso (gr)")]
        [Range(0, int.MaxValue, ErrorMessage = "El peso debe ser un valor positivo")]
        public int? cita_Peso { get; set; }

        /// <summary>
        /// Obtiene o establece la temperatura de la mascota.
        /// </summary>
        [Display(Name = "Temperatura (°C)")]
        [Range(0, int.MaxValue, ErrorMessage = "La temperatura debe ser un valor positivo")]
        public int? cita_Temperatura { get; set; }

        /// <summary>
        /// Obtiene o establece la frecuencia cardíaca de la mascota.
        /// </summary>
        [Display(Name = "Frecuencia cardíaca")]
        [Range(0, int.MaxValue, ErrorMessage = "La frecuencia cardíaca debe ser un valor positivo")]
        public int? cita_FrecuenciaCardiaca { get; set; }

        /// <summary>
        /// Obtiene o establece la frecuencia respiratoria de la mascota.
        /// </summary>
        [Display(Name = "Frecuencia respiratoria")]
        [Range(0, int.MaxValue, ErrorMessage = "La frecuencia respiratoria debe ser un valor positivo")]
        public int? cita_FrecuenciaRespiratoria { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la vacuna aplicada.
        /// </summary>
        [Display(Name = "Vacuna")]
        public int? vac_Id { get; set; }

        public string vacuna { get; set; }


        /// <summary>
        /// Obtiene o establece los medicamentos recetados.
        /// </summary>
        [Display(Name = "Medicamentos recetados")]
        [StringLength(255)]
        public string cita_MedicamentosRecetados { get; set; }

        /// <summary>
        /// Obtiene o establece la dosificación de los medicamentos.
        /// </summary>
        [Display(Name = "Dosificación")]
        [StringLength(255)]
        public string cita_Dosificacion { get; set; }

        /// <summary>
        /// Obtiene o establece los procedimientos realizados.
        /// </summary>
        [Display(Name = "Procedimientos realizados")]
        [StringLength(255)]
        public string cita_ProcedimientosRealizados { get; set; }

        /// <summary>
        /// Obtiene o establece los resultados de los exámenes.
        /// </summary>
        [Display(Name = "Resultados de exámenes")]
        [StringLength(255)]
        public string cita_ResultadosExamenes { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la próxima cita.
        /// </summary>
        [Display(Name = "Próxima cita")]
        public DateTime? cita_ProximaCita { get; set; }

        /// <summary>
        /// Obtiene o establece el motivo de la próxima cita.
        /// </summary>
        [Display(Name = "Motivo próxima cita")]
        [StringLength(255)]
        public string cita_MotivoProximaCita { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la cita médica.
        /// </summary>
        public int cita_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la cita médica.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string cita_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la cita médica.
        /// </summary>
        [Display(Name = "Fecha creación")]
        public DateTime cita_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la cita médica.
        /// </summary>
        public int? cita_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la cita médica.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string cita_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la cita médica.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? cita_FechaModifica { get; set; }

        // Propiedades adicionales para mostrar información relacionada
        //public List<SelectListItem> MascotaList { get; set; } = new List<SelectListItem>();
        public SelectList MascotaList { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de comportamiento.
        /// </summary>
        public SelectList comportamientoList { get; set; } 

        public SelectList VacunaList { get; set; }

        /// <summary>
        /// Carga los elementos de las listas desplegables.
        /// </summary>
        /// <param name="refugioDropdownResults">Resultados de la lista desplegable de refugios.</param>
        /// <param name="empleadoCargoDropdownResults">Resultados de la lista desplegable de cargos de empleados.</param>
        public void LoadDropDownList(IEnumerable<ComportamientoViewModel> comportamientoViewModels, IEnumerable<VacunaDropdownViewModel> vacunaViewModel)
        {
            comportamientoList = new SelectList(comportamientoViewModels, "Com_Id", "Com_Descripcion");
            VacunaList = new SelectList(vacunaViewModel, "vac_Id", "vac_Descripcion");
        }
         
        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.cita_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
