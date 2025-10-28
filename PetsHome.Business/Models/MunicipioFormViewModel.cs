using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para formularios de creación y edición de municipios.
    /// </summary>
    public class MunicipioFormViewModel
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
        [Display(Name = "Id Departamento")]
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
        /// Obtiene o establece el ID del usuario que crea el municipio.
        /// </summary>
        public int? mpio_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el municipio.
        /// </summary>
        public int? mpio_UsuarioModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                return this.mpio_Id != 0;
            }
        }
    }
}
