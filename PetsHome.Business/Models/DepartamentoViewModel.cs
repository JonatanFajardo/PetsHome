using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para un departamento.
    /// </summary>
    public class DepartamentoViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del departamento.
        /// </summary>
        [Key]
        [Display(Name = "Id Departamentos")]
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
        [Display(Name = "Usuario creación")]
        public int? depto_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el departamento.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string UsuarioCreacion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del departamento.
        /// </summary>
        [Display(Name = "Fecha creación")]
        public DateTime? depto_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el departamento.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public int? depto_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el departamento.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string UsuarioModificacion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del departamento.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? depto_FechaModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la lista de municipios asociados al departamento.
        /// </summary>
        public List<MunicipioViewModel> ListadoMunicipios { get; set; }

        /// <summary>
        /// Obtiene o establece el municipio asociado al departamento.
        /// </summary>
        public MunicipioViewModel Municipio { get; set; } = new MunicipioViewModel();

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.depto_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}