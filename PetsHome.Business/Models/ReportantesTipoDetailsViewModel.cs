using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para mostrar los detalles de un tipo de reportante.
    /// </summary>
    public class ReportantesTipoDetailsViewModel
    {
        [Display(Name = "Id")]
        public int reptip_Id { get; set; }

        [Display(Name = "Descripción")]
        public string reptip_Descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool reptip_EsActivo { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime reptip_FechaCrea { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? reptip_FechaModifica { get; set; }
    }
}
