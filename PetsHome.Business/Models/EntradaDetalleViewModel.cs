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
        public int itm_Id { get; set; }
        public int entdet_Cantidad { get; set; }
        public int cat_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? NombreUsuarioCrea { get; set; }

        [Display(Name = "")]
        public DateTime entdet_FechaCrea { get; set; }

        public int? entdet_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? entdet_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? entdet_FechaModifica { get; set; }

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
