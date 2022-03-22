using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetsHome.Business.Models
{
    public class EntradaDetalleViewModel
    {

        public int entdet_Id { get; set; }
        public int ent_Id { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int itm_Id { get; set; }
        public string itm_Descripcion { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int entdet_Cantidad { get; set; }
        public int entdet_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? entdet_NombreUsuarioCrea { get; set; }

        [Display(Name = "Fecha creación")]
        public DateTime entdet_FechaCrea { get; set; }

        public int? entdet_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? entdet_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? entdet_FechaModifica { get; set; }

        #region Dropdown
        public SelectList itemList { get; set; }

        public void LoadDropDownList(IEnumerable<ItemViewModel> itemDropdownResults)
        {
            itemList = new SelectList(itemDropdownResults, "itm_Id", "itm_Descripcion");
        }
        #endregion Dropdown

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.entdet_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
