using Microsoft.AspNetCore.Mvc.Rendering;
using PetsHome.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PetsHome.Business.Models
{
    public partial class RefugioViewModel
    {
        [Key]
        [Display(Name = "Id")]
        public int refg_Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string refg_Nombre { get; set; }

        [Display(Name = "Ubicacion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string refg_Ubicacion { get; set; }

        [Display(Name = "RTN")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(14)]
        public string refg_RTN { get; set; }

        [Display(Name = "Telefono")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(8)]
        public string refg_Telefono { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string refg_Correo { get; set; }

        [Display(Name = "Departamento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string depto_Id { get; set; }

        [Display(Name = "Municipio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string mpio_Id { get; set; }

        [Display(Name = "InformacionAdicional")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500)]
        public string refg_InformacionAdicional { get; set; }
        public bool refg_EsActivo { get; set; }

        public int refg_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? refg_NombreUsuarioCrea { get; set; }

        [Display(Name = "Fecha creación")]
        public DateTime refg_FechaCrea { get; set; }
        public int? refg_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? refg_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? refg_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.refg_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown
        public SelectList departamentoList { get; set; }
        public SelectList municipioList { get; set; }

        public void LoadDropDownList(IEnumerable<DepartamentoViewModel> departamentoResults,
                                        IEnumerable<MunicipioViewModel> municipioResults)
        {
            departamentoList = new SelectList(departamentoResults, "depto_Id", "depto_Descripcion");
            municipioList = new SelectList(municipioResults, "mpio_Id", "mpio_Descripcion");
            #endregion Dropdown

        }
    }
}