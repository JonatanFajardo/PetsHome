using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para un cargo de empleado.
    /// </summary>
    public partial class EmpleadoCargoViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del cargo.
        /// </summary>
        [Key]
        [Display(Name = "Id")]
        public int cag_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del cargo.
        /// </summary>
        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string cag_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el salario del cargo.
        /// </summary>
        [Display(Name = "Salario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? cag_Salario { get; set; }

        /// <summary>
        /// Obtiene o establece el estado del cargo.
        /// </summary>
        public string EsActivo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el cargo.
        /// </summary>
        public int cag_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el cargo.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? cag_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del cargo.
        /// </summary>
        public DateTime cag_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el cargo.
        /// </summary>
        public int? cag_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el cargo.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? cag_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del cargo.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? cag_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.cag_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}