using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para crear o editar tipos de reportantes.
    /// </summary>
    public class ReportantesTipoFormViewModel
    {
        public long? Fila { get; set; }

        [Display(Name = "Id")]
        public int reptip_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string reptip_Descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool reptip_EsActivo { get; set; }

        public int reptip_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string reptip_NombreUsuarioCrea { get; set; }

        public DateTime reptip_FechaCrea { get; set; }

        public int? reptip_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string reptip_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? reptip_FechaModifica { get; set; }

        public bool isEdit => reptip_Id != 0;
    }
}
