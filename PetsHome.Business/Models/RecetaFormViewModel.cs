using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para crear o editar recetas médicas.
    /// </summary>
    public class RecetaFormViewModel
    {
        [Key]
        [Display(Name = "Id receta")]
        public int receta_Id { get; set; }

        [Display(Name = "Cita médica")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int cita_Id { get; set; }

        [Display(Name = "Mascota")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int masc_Id { get; set; }

        public string masc_Nombre { get; set; }

        [Display(Name = "Medicamento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(200)]
        public string receta_Medicamento { get; set; }

        [Display(Name = "Tipo de medicamento")]
        public int? tipoMed_Id { get; set; }

        public string TipoMedicamento { get; set; }

        [Display(Name = "Vía de administración")]
        public int? viaAdmin_Id { get; set; }

        public string ViaAdministracion { get; set; }

        [Display(Name = "Dosis")]
        [StringLength(100)]
        public string receta_Dosis { get; set; }

        [Display(Name = "Frecuencia")]
        [StringLength(100)]
        public string receta_Frecuencia { get; set; }

        [Display(Name = "Duración")]
        [StringLength(100)]
        public string receta_Duracion { get; set; }

        [Display(Name = "Instrucciones")]
        [StringLength(500)]
        public string receta_Instrucciones { get; set; }

        [Display(Name = "Fecha inicio")]
        public DateTime? receta_FechaInicio { get; set; }

        [Display(Name = "Fecha fin")]
        public DateTime? receta_FechaFin { get; set; }

        [Display(Name = "Estado")]
        [StringLength(20)]
        public string receta_Estado { get; set; }

        public int receta_UsuarioCrea { get; set; }

        public DateTime receta_FechaCrea { get; set; }

        public int? receta_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? receta_FechaModifica { get; set; }

        public bool isEdit => receta_Id != 0;

        #region Dropdown

        public SelectList MascotaList { get; set; }

        public SelectList CitaMedicaList { get; set; }

        public SelectList TipoMedicamentoList { get; set; }

        public SelectList ViaAdministracionList { get; set; }

        public void LoadDropDownList(
            IEnumerable<MascotaDropdownViewModel> mascotaViewModels,
            IEnumerable<object> citaMedicaViewModels,
            IEnumerable<TipoMedicamentoViewModel> tipoMedicamentoViewModels,
            IEnumerable<ViaAdministracionViewModel> viaAdministracionViewModels)
        {
            MascotaList = new SelectList(mascotaViewModels, "masc_Id", "masc_Nombre");
            CitaMedicaList = new SelectList(citaMedicaViewModels, "cita_Id", "Descripcion");
            TipoMedicamentoList = new SelectList(tipoMedicamentoViewModels, "tipoMed_Id", "tipoMed_Descripcion");
            ViaAdministracionList = new SelectList(viaAdministracionViewModels, "viaAdmin_Id", "viaAdmin_Descripcion");
        }

        #endregion Dropdown
    }
}
