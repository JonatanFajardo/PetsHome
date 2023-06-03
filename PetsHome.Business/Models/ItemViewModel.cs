using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para un item.
    /// </summary>
    public partial class ItemViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del item.
        /// </summary>
        [Key]
        public int itm_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código del item.
        /// </summary>
        [Display(Name = "Codigo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string itm_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del item.
        /// </summary>
        [Display(Name = "Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string itm_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la categoría del item.
        /// </summary>
        [Display(Name = "Id categoría")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? cat_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del refugio del item.
        /// </summary>
        [Display(Name = "Id refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del refugio del item.
        /// </summary>
        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el precio del item.
        /// </summary>
        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal itm_Precio { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de entradas del item.
        /// </summary>
        [Display(Name = "Entrada")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? itm_Entrada { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de salidas del item.
        /// </summary>
        [Display(Name = "Salida")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? itm_Salida { get; set; }

        /// <summary>
        /// Obtiene o establece el stock actual del item.
        /// </summary>
        [Display(Name = "Stock")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? itm_Stock { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el item.
        /// </summary>
        public int itm_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de items.
        /// </summary>
        public SelectList itemList { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del item.
        /// </summary>
        public DateTime itm_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el item.
        /// </summary>
        public int? itm_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del item.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? itm_FechaModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la categoría del item.
        /// </summary>
        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string cat_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el item.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? itm_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el item.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? itm_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.itm_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        /// <summary>
        /// Carga la lista desplegable de categorías de items.
        /// </summary>
        public void LoadDropDownList(IEnumerable<CategoriaViewModel> categoriaViewModels)
        {
            itemList = new SelectList(categoriaViewModels, "cat_Id", "cat_Descripcion");
        }

        #endregion Dropdown
    }
}