using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Representa un registro de la tabla tbSubmenu.
    /// </summary>
    [Table("tbSubmenu")]
    public partial class SubmenuViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador del registro.
        /// </summary>
        [Key]
        public int sub_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string sub_Nombre { get; set; }

        /// <summary>
        /// Indica si el formulario se esta editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.sub_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}