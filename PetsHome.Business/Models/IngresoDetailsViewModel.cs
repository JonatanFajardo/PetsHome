using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para mostrar los detalles de un ingreso.
    /// </summary>
    public class IngresoDetailsViewModel
    {
        [Display(Name = "Id")]
        public int ingr_Id { get; set; }

        [Display(Name = "Reporte de Abandono")]
        public int? repa_Id { get; set; }

        [Display(Name = "Refugio")]
        public int refg_Id { get; set; }

        [Display(Name = "Nombre Refugio")]
        public string refg_Nombre { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        public DateTime ingr_FechaIngreso { get; set; }

        [Display(Name = "Lugar del Rescate")]
        public string ingr_LugarRescate { get; set; }

        [Display(Name = "Condición Inicial")]
        public string ingr_CondicionInicial { get; set; }

        [Display(Name = "Persona Rescatista")]
        public string ingr_PersonaRescatista { get; set; }

        [Display(Name = "Medio de Transporte")]
        public string ingr_MedioTransporte { get; set; }

        [Display(Name = "Observaciones")]
        public string ingr_Observaciones { get; set; }

        [Display(Name = "Es Emergencia")]
        public bool ingr_EsEmergencia { get; set; }

        [Display(Name = "Ubicación Reporte")]
        public string repa_UbicacionIncidente { get; set; }

        [Display(Name = "Descripción Animal (del reporte)")]
        public string repa_DescripcionAnimal { get; set; }

        [Display(Name = "Estado Atención")]
        public string repa_EstadoAtencion { get; set; }

        [Display(Name = "Reportante")]
        public string repa_NombreReportante { get; set; }

        [Display(Name = "Teléfono Reportante")]
        public string repa_TelefonoContactoContacto { get; set; }

        public int ingr_UsuarioCrea { get; set; }
        public string ingr_NombreUsuarioCrea { get; set; }
        public DateTime ingr_FechaCrea { get; set; }
        public int? ingr_UsuarioModifica { get; set; }
        public string ingr_NombreUsuarioModifica { get; set; }
        public DateTime? ingr_FechaModifica { get; set; }

        // Mascota asociada
        public int TieneMascota { get; set; }

        public int? MascotaId { get; set; }

        public string MascotaNombre { get; set; }
    }
}
