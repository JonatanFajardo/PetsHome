using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetsHome.Common.InternalEntities;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo utilizado para crear o editar mascotas.
    /// </summary>
    public class MascotaFormViewModel
    {
        [Key]
        [Display(Name = "Id mascota")]
        public int masc_Id { get; set; }

        public byte[] masc_Imagen { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ0-9]+(?: [a-zA-ZáéíóúÁÉÍÓÚñÑüÜ0-9]+)*$", ErrorMessage = "El {0} no debe tener espacios al inicio o final")]
        public string masc_Nombre { get; set; }

        [Display(Name = "Id raza")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? raza_Id { get; set; }

        [Display(Name = "Raza")]
        [StringLength(50)]
        public string? raza_Descripcion { get; set; }

        [Display(Name = "Edad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 100, ErrorMessage = "La {0} debe estar entre {1} y {2}")]
        public int? masc_Edad { get; set; }

        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string masc_Sexo { get; set; }

        [Display(Name = "Peso")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "decimal(18, 0)")]
        [Range(0.1, 500, ErrorMessage = "El {0} debe estar entre {1} y {2}")]
        public decimal? masc_Peso { get; set; }

        [Display(Name = "Talla")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? tall_Id { get; set; }

        [Display(Name = "Talla descripción")]
        public string? tall_Descripcion { get; set; }

        [Display(Name = "Color")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ]+(?: [a-zA-ZáéíóúÁÉÍÓÚñÑüÜ]+)*$", ErrorMessage = "El {0} solo debe contener letras, sin espacios al inicio o final")]
        public string masc_Color { get; set; }

        [Display(Name = "Historia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500)]
        public string masc_Historia { get; set; }

        [Display(Name = "Id refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? refg_Id { get; set; }

        [Display(Name = "Refugio")]
        [StringLength(50)]
        public string? refg_Nombre { get; set; }

        [Display(Name = "Id procedencia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? proc_Id { get; set; }

        [Display(Name = "Procedencia")]
        [StringLength(50)]
        public string? proc_Descripcion { get; set; }

        public bool? masc_EsAdoptado { get; set; }

        public bool? masc_EsReservado { get; set; }

        public int masc_UsuarioCrea { get; set; }

        public DateTime masc_masc_FechaCrea { get; set; }

        public int? masc_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? masc_FechaModifica { get; set; }

        [Display(Name = "Usuario creación")]
        public string? NombreUsuarioCrea { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? masc_NombreUsuarioModifica { get; set; }

        public long? masc_Fila { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string pathMascotaImage { get; set; }

        public bool isEdit => masc_Id != 0;

        #region Dropdown

        public SelectList razaList { get; set; }

        public SelectList sexoList { get; set; }

        public SelectList refugioList { get; set; }

        public SelectList procedenciaList { get; set; }

        public SelectList tallaList { get; set; }

        public void LoadDropDownList(IEnumerable<RazaDropdownViewModel> razaDropdownResults,
                                     IEnumerable<Dropdown> dropdownlists,
                                     IEnumerable<RefugioDropdownViewModel> refugioDropdownResults,
                                     IEnumerable<ProcedenciaViewModel> procedenciaDropdownResults,
                                     IEnumerable<TallaDropdownViewModel> tallaDropdownResults)
        {
            razaList = new SelectList(razaDropdownResults, "raza_Id", "raza_Descripcion");
            sexoList = new SelectList(dropdownlists, "Value", "Text");
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
            procedenciaList = new SelectList(procedenciaDropdownResults, "proc_Id", "proc_Descripcion");
            tallaList = new SelectList(tallaDropdownResults, "tall_Id", "tall_Descripcion");
        }

        #endregion Dropdown
    }
}
