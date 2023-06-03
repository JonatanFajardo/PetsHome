using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    public partial class EntradaViewModel
    {
        [Key]
        [Display(Name = "Id entrada")]
        public int ent_Id { get; set; }

        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int refg_Id { get; set; }

        public string refg_Nombre { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? item_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? item_Descripcion { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string ent_Descripcion { get; set; }

        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? ent_Fecha { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? ent_Cantidad { get; set; }

        public int? ent_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? ent_NombreUsuarioCrea { get; set; }

        public DateTime ent_FechaCrea { get; set; }
        public int? ent_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? ent_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? ent_FechaModifica { get; set; }

        public List<EntradaDetalleViewModel> ListadoEntradasDetalles { get; set; }
        public EntradaDetalleViewModel EntradaDetalle { get; set; } = new EntradaDetalleViewModel();

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.ent_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        public SelectList refugioList { get; set; }
        //public SelectList empleadoCargoList { get; set; }
        //public SelectList procedenciaList { get; set; }

        public void LoadDropDownList(IEnumerable<RefugioViewModel> refugioDropdownResults)
        //                                IEnumerable<Dropdown> dropdownlists,
        //                                IEnumerable<PR_Refugio_Procedencia_DropdownResult> procedenciaDropdownResults)
        {
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
            //empleadoCargoList = new SelectList(dropdownlists, "cag_Id", "cag_Descripcion");
            //procedenciaList = new SelectList(procedenciaDropdownResults, "proc_Id", "proc_Descripcion");
        }

        #endregion Dropdown
    }
}