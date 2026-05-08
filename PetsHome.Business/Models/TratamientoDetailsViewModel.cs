using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo utilizado para mostrar el detalle de un tratamiento.
    /// </summary>
    public class TratamientoDetailsViewModel
    {
        [Key]
        [Display(Name = "Id tratamiento")]
        public int trat_Id { get; set; }

        public int masc_Id { get; set; }

        [Display(Name = "Mascota")]
        public string Mascota { get; set; }

        [Display(Name = "Tipo de parásito")]
        public string TipoParasito { get; set; }

        [Display(Name = "Categoría de parásito")]
        public string CategoriaParasito { get; set; }

        [Display(Name = "Parásito detectado")]
        public string trat_ParasitoDetectado { get; set; }

        [Display(Name = "Medicamento")]
        public string trat_Medicamento { get; set; }

        [Display(Name = "Tipo de medicamento")]
        public string TipoMedicamento { get; set; }

        [Display(Name = "Vía de administración")]
        public string ViaAdministracion { get; set; }

        [Display(Name = "Fecha de aplicación")]
        public DateTime trat_FechaAplicacion { get; set; }

        [Display(Name = "Aplicado por")]
        public string trat_AplicadoPor { get; set; }

        [Display(Name = "Próxima dosis")]
        public DateTime? trat_ProximaDosis { get; set; }

        [Display(Name = "Estado")]
        public string trat_Estado { get; set; }

        [Display(Name = "Observaciones")]
        public string trat_Observaciones { get; set; }

        [Display(Name = "Usuario creación")]
        public string UsuarioCreacion { get; set; }

        [Display(Name = "Fecha creación")]
        public DateTime trat_FechaCrea { get; set; }

        [Display(Name = "Usuario modificación")]
        public string UsuarioModificacion { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? trat_FechaModifica { get; set; }
    }
}
