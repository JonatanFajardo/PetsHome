using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class ProcedenciaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la procedencia.
        /// </summary>
        [Key]
        [Display(Name = "Id procedencia")]
        public int proc_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la procedencia.
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string proc_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la procedencia.
        /// </summary>
        public int proc_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la procedencia.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? proc_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la procedencia.
        /// </summary>
        public DateTime proc_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la procedencia.
        /// </summary>
        public int? proc_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la procedencia.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? proc_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la procedencia.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? proc_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.proc_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}