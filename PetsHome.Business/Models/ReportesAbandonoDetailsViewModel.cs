using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para mostrar los detalles de un reporte de abandono.
    /// </summary>
    public class ReportesAbandonoDetailsViewModel
    {
        [Display(Name = "Id")]
        public int repa_Id { get; set; }

        [Display(Name = "Tipo de Reportante")]
        public int reptip_Id { get; set; }

        [Display(Name = "Descripción Tipo")]
        public string reptip_Descripcion { get; set; }

        [Display(Name = "Nombre del Reportante")]
        public string repa_NombreReportante { get; set; }

        [Display(Name = "Teléfono de Contacto")]
        public string repa_TelefonoContactoContacto { get; set; }

        [Display(Name = "Fecha del Reporte")]
        public DateTime repa_FechaReporte { get; set; }

        [Display(Name = "Ubicación del Incidente")]
        public string repa_UbicacionIncidente { get; set; }

        [Display(Name = "Descripción del Animal")]
        public string repa_DescripcionAnimal { get; set; }

        [Display(Name = "Estado del Animal")]
        public string repa_EstadoAnimal { get; set; }

        [Display(Name = "Estado de Atención")]
        public string repa_EstadoAtencion { get; set; }

        [Display(Name = "Observaciones")]
        public string repa_Observaciones { get; set; }

        [Display(Name = "Refugio")]
        public int? refg_Id { get; set; }

        [Display(Name = "Nombre Refugio")]
        public string refg_Nombre { get; set; }

        public int repa_UsuarioCrea { get; set; }
        public string repa_NombreUsuarioCrea { get; set; }
        public DateTime repa_FechaCrea { get; set; }
        public int? repa_UsuarioModifica { get; set; }
        public string repa_NombreUsuarioModifica { get; set; }
        public DateTime? repa_FechaModifica { get; set; }
    }
}
