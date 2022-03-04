using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetsHome.Common.Entities;
using PetsHome.Common.InternalEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{
    public partial class MascotaViewModel
    {
        [Key]
        [Display(Name = "Id mascota")]
        public int masc_Id { get; set; }

        //[Display(Name = "Imagen")]
        //[Required(ErrorMessage = "El campo {0} es requerido")]
        public byte[] masc_Imagen { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string masc_Nombre { get; set; }

        [Display(Name = "Id raza")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? raza_Id { get; set; }

        [Display(Name = "Raza")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string? raza_Descripcion { get; set; }

        [Display(Name = "Edad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? masc_Edad { get; set; }

        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        //[StringLength(1)]
        public string masc_Sexo { get; set; }

        [Display(Name = "Peso")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? masc_Peso { get; set; }

        [Display(Name = "Talla")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? masc_Talla { get; set; }

        [Display(Name = "Color")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string masc_Color { get; set; }

        [Display(Name = "Historia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500)]
        public string masc_Historia { get; set; }

        [Display(Name = "Id refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? refg_Id { get; set; }

        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string? refg_Nombre { get; set; }

        [Display(Name = "Id procedencia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? proc_Id { get; set; }

        [Display(Name = "Procedencia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string? proc_Descripcion { get; set; }

        public bool? masc_EsAdoptado { get; set; }

        public bool? masc_EsReservado { get; set; }


        public int masc_UsuarioCrea { get; set; }

        public DateTime masc_masc_FechaCrea { get; set; }
        public int? masc_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? masc_FechaModifica { get; set; }

        // Propiedades Extras
        [Display(Name = "Usuario creación")]
        public string? NombreUsuarioCrea { get; set; }
        [Display(Name = "Usuario modificación")]
        public string? masc_NombreUsuarioModifica { get; set; }
        public long? masc_Fila { get; set; }
        public IFormFile ImageFile { get; set; }

        public string pathMascotaImage { get; set; }



        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.masc_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        public SelectList razaList { get; set; }
        public SelectList sexoList { get; set; }
        public SelectList refugioList { get; set; }
        public SelectList procedenciaList { get; set; }
        //public void LoadDropDownList(IEnumerable<PR_Refugio_Raza_DropdownResult> razaDropdownResults,
        //                               IEnumerable<Dropdown> dropdownlists,
        //                               IEnumerable<PR_Refugio_Refugio_DropdownResult> refugioDropdownResults,
        //                               IEnumerable<PR_Refugio_Procedencia_DropdownResult> procedenciaDropdownResults)
        public void LoadDropDownList(IEnumerable<RazaViewModel> razaDropdownResults,
                                        IEnumerable<Dropdown> dropdownlists,
                                        IEnumerable<RefugioViewModel> refugioDropdownResults,
                                        IEnumerable<ProcedenciaViewModel> procedenciaDropdownResults)
        {
            razaList = new SelectList(razaDropdownResults, "raza_Id", "raza_Descripcion");
            sexoList = new SelectList(dropdownlists, "Value", "Text");
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
            procedenciaList = new SelectList(procedenciaDropdownResults, "proc_Id", "proc_Descripcion");
        }
        #endregion Dropdown

    }
}