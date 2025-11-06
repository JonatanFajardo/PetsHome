using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class TratamientoViewModel
    {
        [Key]
        [Display(Name = "Id tratamiento")]
        public int trat_Id { get; set; }

        [Display(Name = "Mascota")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int masc_Id { get; set; }

        public string masc_Nombre { get; set; }

        [Display(Name = "Cita médica")]
        public int? cita_Id { get; set; }

        [Display(Name = "Receta")]
        public int? receta_Id { get; set; }

        [Display(Name = "Tipo de parásito")]
        public int? tipoPar_Id { get; set; }

        public string TipoParasito { get; set; }

        [Display(Name = "Parásito detectado")]
        [StringLength(200)]
        public string trat_ParasitoDetectado { get; set; }

        [Display(Name = "Medicamento")]
        [StringLength(200)]
        public string trat_Medicamento { get; set; }

        [Display(Name = "Tipo de medicamento")]
        public int? tipoMed_Id { get; set; }

        public string TipoMedicamento { get; set; }

        [Display(Name = "Vía de administración")]
        public int? viaAdmin_Id { get; set; }

        public string ViaAdministracion { get; set; }

        [Display(Name = "Fecha de aplicación")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime trat_FechaAplicacion { get; set; }

        [Display(Name = "Aplicado por")]
        [StringLength(200)]
        public string trat_AplicadoPor { get; set; }

        [Display(Name = "Próxima dosis")]
        public DateTime? trat_ProximaDosis { get; set; }

        [Display(Name = "Estado")]
        [StringLength(20)]
        public string trat_Estado { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(500)]
        public string trat_Observaciones { get; set; }

        public int trat_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string trat_NombreUsuarioCrea { get; set; }

        public DateTime trat_FechaCrea { get; set; }

        public int? trat_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string trat_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? trat_FechaModifica { get; set; }

        // Propiedades adicionales para listas desplegables
        public SelectList MascotaList { get; set; }
        public SelectList CitaMedicaList { get; set; }
        public SelectList RecetaList { get; set; }
        public SelectList TipoParasitoList { get; set; }
        public SelectList TipoMedicamentoList { get; set; }
        public SelectList ViaAdministracionList { get; set; }

        public void LoadDropDownList(
            IEnumerable<MascotaDropdownViewModel> mascotaViewModels,
            IEnumerable<object> citaMedicaViewModels,
            IEnumerable<object> recetaViewModels,
            IEnumerable<TipoParasitoViewModel> tipoParasitoViewModels,
            IEnumerable<TipoMedicamentoViewModel> tipoMedicamentoViewModels,
            IEnumerable<ViaAdministracionViewModel> viaAdministracionViewModels)
        {
            MascotaList = new SelectList(mascotaViewModels, "masc_Id", "masc_Nombre");
            CitaMedicaList = new SelectList(citaMedicaViewModels, "cita_Id", "Descripcion");
            RecetaList = new SelectList(recetaViewModels, "receta_Id", "Descripcion");
            TipoParasitoList = new SelectList(tipoParasitoViewModels, "tipoPar_Id", "tipoPar_Descripcion");
            TipoMedicamentoList = new SelectList(tipoMedicamentoViewModels, "tipoMed_Id", "tipoMed_Descripcion");
            ViaAdministracionList = new SelectList(viaAdministracionViewModels, "viaAdmin_Id", "viaAdmin_Descripcion");
        }

        public bool isEdit
        {
            get
            {
                if (this.trat_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
