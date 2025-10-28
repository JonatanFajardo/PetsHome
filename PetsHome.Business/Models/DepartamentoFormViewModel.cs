using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para formularios de creación y edición de departamentos.
    /// </summary>
    public class DepartamentoFormViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del departamento.
        /// </summary>
        [Key]
        [Display(Name = "Id Departamento")]
        public int depto_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código del departamento.
        /// </summary>
        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo código es requerido")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "El código debe tener {1} caracteres")]
        [RegularExpression(@"^\d*$", ErrorMessage = "Solo se permiten números")]
        public string depto_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del departamento.
        /// </summary>
        [Display(Name = "Departamento")]
        [Required(ErrorMessage = "El campo departamento es requerido")]
        [StringLength(100, ErrorMessage = "El departamento debe tener un máximo de {1} caracteres")]
        public string depto_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el departamento.
        /// </summary>
        public int? depto_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el departamento.
        /// </summary>
        public int? depto_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la lista de municipios asociados al departamento.
        /// </summary>
        public List<MunicipioListViewModel> ListadoMunicipios { get; set; }

        /// <summary>
        /// Obtiene o establece el municipio asociado al departamento (para formulario anidado).
        /// </summary>
        public MunicipioFormViewModel Municipio { get; set; } = new MunicipioFormViewModel();

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                return this.depto_Id != 0;
            }
        }
    }
}
