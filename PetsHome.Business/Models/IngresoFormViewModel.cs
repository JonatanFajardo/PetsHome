using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para crear o editar ingresos.
    /// </summary>
    public class IngresoFormViewModel
    {
        [Key]
        public int ingr_Id { get; set; }

        [Display(Name = "Reporte de Abandono")]
        public int? repa_Id { get; set; }

        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int refg_Id { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime ingr_FechaIngreso { get; set; }

        [Display(Name = "Lugar del Rescate")]
        [StringLength(200)]
        public string ingr_LugarRescate { get; set; }

        [Display(Name = "Condición Inicial")]
        [StringLength(200)]
        public string ingr_CondicionInicial { get; set; }

        [Display(Name = "Persona Rescatista")]
        [StringLength(150)]
        public string ingr_PersonaRescatista { get; set; }

        [Display(Name = "Medio de Transporte")]
        [StringLength(100)]
        public string ingr_MedioTransporte { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(300)]
        public string ingr_Observaciones { get; set; }

        [Display(Name = "Es Emergencia")]
        public bool ingr_EsEmergencia { get; set; }

        public int ingr_UsuarioCrea { get; set; }

        public DateTime ingr_FechaCrea { get; set; }

        public int? ingr_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? ingr_FechaModifica { get; set; }

        // Campos relacionados
        public string NombreRefugio { get; set; }

        public string LugarReporte { get; set; }

        public string repa_DescripcionAnimal { get; set; }

        public string repa_EstadoAtencion { get; set; }

        public string repa_NombreReportante { get; set; }

        public string TelefonoReportante { get; set; }

        // Mascota asociada
        public int TieneMascota { get; set; }

        public int? MascotaId { get; set; }

        public string MascotaNombre { get; set; }

        public bool isEdit => ingr_Id != 0;

        #region Dropdown

        public SelectList reportesList { get; set; }

        public SelectList refugioList { get; set; }

        public void LoadDropDownList(IEnumerable<ReportesAbandonoListViewModel> reportesDropdownResults,
                                     IEnumerable<RefugioDropdownViewModel> refugioDropdownResults)
        {
            reportesList = new SelectList(reportesDropdownResults, "repa_Id", "repa_UbicacionIncidente");
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
        }

        #endregion Dropdown

        public IngresoFormViewModel()
        {
            ingr_FechaIngreso = DateTime.Now;
        }
    }
}
