using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista para un municipio.
    /// </summary>
    public partial class MunicipioViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del municipio.
        /// </summary>
        [Key]
        [Display(Name = "Id")]
        public int mpio_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código del municipio.
        /// </summary>
        [Display(Name = "Código de municipio")]
        public string mpio_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del municipio.
        /// </summary>
        [Display(Name = "Municipio")]
        [Required(ErrorMessage = "El campo municipio es requerido")]
        [StringLength(100, ErrorMessage = "El municipio debe tener un máximo de {1} caracteres")]
        public string mpio_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del departamento al que pertenece el municipio.
        /// </summary>
        [Display(Name = "Id dep")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int depto_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código del departamento al que pertenece el municipio.
        /// </summary>
        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo código es requerido")]
        [RegularExpression(@"^\d*$", ErrorMessage = "Solo se permiten números")]
        public string depto_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del departamento al que pertenece el municipio.
        /// </summary>

        [Display(Name = "Usuario creación")]
        public int? mpio_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del departamento al que pertenece el municipio.
        /// </summary>

        [Display(Name = "Usuario creación")]
        public string UsuarioCreacion { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del departamento al que pertenece el municipio.
        /// </summary>

        [Display(Name = "Fecha creación")]
        public DateTime? mpio_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del departamento al que pertenece el municipio.
        /// </summary>

        [Display(Name = "Usuario modificación")]
        public int? mpio_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del departamento al que pertenece el municipio.
        /// </summary>

        [Display(Name = "Usuario modificación")]
        public string UsuarioModificacion { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del departamento al que pertenece el municipio.
        /// </summary>

        [Display(Name = "Fecha modificación")]
        public DateTime? mpio_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.mpio_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}