using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class TipoConsultaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del tipo de consulta.
        /// </summary>
        [Key]
        [Display(Name = "Id tipo de consulta")]
        public int tipoCon_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del tipo de consulta.
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string tipoCon_Descripcion { get; set; }

        public string tipoCon_EsActivo { get; set; }

        [Display(Name = "Estado")]
        public bool tipoCon_EsActivoBool { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el tipo de consulta.
        /// </summary>
        public int tipoCon_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el tipo de consulta.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string tipoCon_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del tipo de consulta.
        /// </summary>
        public DateTime tipoCon_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el tipo de consulta.
        /// </summary>
        public int? tipoCon_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el tipo de consulta.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string tipoCon_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del tipo de consulta.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? tipoCon_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.tipoCon_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
