using Microsoft.AspNetCore.Mvc.Rendering;
using PetsHome.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PetsHome.Business.Models
{
    public partial class EmpleadoViewModel
    {
        [Key]
        [Display(Name = "Id")]
        public int emp_Id { get; set; }

        [Display(Name = "Codigo empleado")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(7)]
        public string emp_Codigo { get; set; }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int per_Id { get; set; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string per_Nombres { get; set; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string per_Apellidos { get; set; }

        [Display(Name = "Id")] //*
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? refg_Id { get; set; }

        [Display(Name = "Refugio")] //*
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Refugio { get; set; }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? cag_Id { get; set; }

        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Categoria { get; set; }

        [Display(Name = "Es Activo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool? esActivo { get; set; }

        public int emp_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? emp_NombreUsuarioCrea { get; set; }
        public DateTime emp_FechaCrea { get; set; }
        public int? emp_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? emp_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? emp_FechaModifica { get; set; }

        #region Dropdown
        public SelectList refugioList { get; set; }
        public SelectList empleadoCargoList { get; set; }
        public SelectList procedenciaList { get; set; }

        public void LoadDropDownList(IEnumerable<PR_Refugio_Refugio_DropdownResult> refugioDropdownResults,
                                        IEnumerable<PR_Refugio_EmpleadosCargos_DropdownResult> empleadoCargoDropdownResults,
                                        IEnumerable<PR_Refugio_Procedencia_DropdownResult> procedenciaDropdownResults)
        {
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
            empleadoCargoList = new SelectList(empleadoCargoDropdownResults, "cag_Id", "cag_Descripcion");
            procedenciaList = new SelectList(procedenciaDropdownResults, "proc_Id", "proc_Descripcion");
        }
        #endregion Dropdown

    }
}