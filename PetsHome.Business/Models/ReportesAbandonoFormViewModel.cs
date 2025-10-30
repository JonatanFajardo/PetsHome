using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para crear o editar reportes de abandono.
    /// </summary>
    public class ReportesAbandonoFormViewModel
    {
        [Key]
        public int repa_Id { get; set; }

        [Display(Name = "Tipo de Reportante")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int reptip_Id { get; set; }

        [Display(Name = "Nombre del Reportante")]
        [StringLength(150)]
        public string repa_NombreReportante { get; set; }

        [Display(Name = "Teléfono de Contacto")]
        [StringLength(20)]
        public string repa_TelefonoContactoContacto { get; set; }

        [Display(Name = "Fecha del Reporte")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime repa_FechaReporte { get; set; }

        [Display(Name = "Ubicación del Incidente")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(300)]
        public string repa_UbicacionIncidente { get; set; }

        [Display(Name = "Descripción del Animal")]
        [StringLength(500)]
        public string repa_DescripcionAnimal { get; set; }

        [Display(Name = "Estado del Animal")]
        [StringLength(500)]
        public string repa_EstadoAnimal { get; set; }

        [Display(Name = "Estado de Atención")]
        [StringLength(50)]
        public string repa_EstadoAtencion { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(500)]
        public string repa_Observaciones { get; set; }

        [Display(Name = "Refugio")]
        public int? refg_Id { get; set; }

        public int repa_UsuarioCrea { get; set; }
        public string repa_NombreUsuarioCrea { get; set; }
        public DateTime repa_FechaCrea { get; set; }
        public int? repa_UsuarioModifica { get; set; }
        public string repa_NombreUsuarioModifica { get; set; }
        public DateTime? repa_FechaModifica { get; set; }

        // Campos relacionados
        public string reptip_Descripcion { get; set; }
        public string refg_Nombre { get; set; }

        public bool isEdit => repa_Id != 0;

        #region Dropdown

        public SelectList reportantesTipoList { get; set; }

        public SelectList refugioList { get; set; }

        public void LoadDropDownList(IEnumerable<ReportantesTipoListViewModel> reportantesTipoDropdownResults,
                                     IEnumerable<RefugioDropdownViewModel> refugioDropdownResults)
        {
            reportantesTipoList = new SelectList(reportantesTipoDropdownResults, "reptip_Id", "reptip_Descripcion");
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
        }

        #endregion Dropdown

        public ReportesAbandonoFormViewModel()
        {
            repa_FechaReporte = DateTime.Now;
            repa_EstadoAtencion = "Pendiente";
        }
    }
}
