using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetsHome.Business.Models
{
    // ─── Lista (DataTable) ─────────────────────────
    public partial class LaboratorioListViewModel
    {
        public int lab_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cita_Id { get; set; }
        public string lab_Examen { get; set; }
        public string lab_Tipo { get; set; }
        public string lab_Estado { get; set; }
        public DateTime lab_FechaSolicitud { get; set; }
        public DateTime? lab_FechaResultado { get; set; }
        public int? lab_Veterinario { get; set; }
        public string Veterinario { get; set; }
        public int TotalParametros { get; set; }
        public int ParametrosFueraRango { get; set; }
    }

    // ─── Formulario Create/Edit ────────────────────
    public partial class LaboratorioFormViewModel
    {
        public int lab_Id { get; set; }

        [Required(ErrorMessage = "Seleccione la mascota.")]
        public int masc_Id { get; set; }

        public int? cita_Id { get; set; }

        [Required(ErrorMessage = "Indique el nombre del examen.")]
        public string lab_Examen { get; set; }

        [Required(ErrorMessage = "Indique el tipo de examen.")]
        public string lab_Tipo { get; set; }

        public string lab_Estado { get; set; }
        public int? lab_Veterinario { get; set; }
        public string lab_Observaciones { get; set; }

        public bool isEdit => lab_Id > 0;

        // Dropdowns de apoyo (no se persisten)
        public List<MascotaDropdownViewModel> MascotaList { get; set; } = new List<MascotaDropdownViewModel>();

        public List<SelectListItem> TipoList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem("Sangre", "Sangre"),
            new SelectListItem("Orina", "Orina"),
            new SelectListItem("Heces", "Heces"),
            new SelectListItem("Imagen", "Imagen"),
            new SelectListItem("Otro", "Otro"),
        };
    }

    // ─── Busqueda (pre-llenar el form en edicion) ───
    public partial class LaboratorioFindViewModel
    {
        public int lab_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cita_Id { get; set; }
        public string lab_Examen { get; set; }
        public string lab_Tipo { get; set; }
        public string lab_Estado { get; set; }
        public int? lab_Veterinario { get; set; }
        public string lab_Observaciones { get; set; }
    }

    // ─── Detalle (read-only) ───────────────────────
    public partial class LaboratorioDetailViewModel
    {
        public int lab_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cita_Id { get; set; }
        public string lab_Examen { get; set; }
        public string lab_Tipo { get; set; }
        public string lab_Estado { get; set; }
        public DateTime lab_FechaSolicitud { get; set; }
        public DateTime? lab_FechaResultado { get; set; }
        public int? lab_Veterinario { get; set; }
        public string Veterinario { get; set; }
        public string lab_Observaciones { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime lab_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? lab_FechaModifica { get; set; }

        // Parametros (filas del examen)
        public List<LaboratorioDetalleViewModel> Parametros { get; set; } = new List<LaboratorioDetalleViewModel>();
    }

    // ─── Parametro / fila de resultado ─────────────
    public partial class LaboratorioDetalleViewModel
    {
        public int labd_Id { get; set; }
        public int lab_Id { get; set; }
        public string labd_Parametro { get; set; }
        public string labd_Valor { get; set; }
        public string labd_Unidad { get; set; }
        public decimal? labd_RangoMin { get; set; }
        public decimal? labd_RangoMax { get; set; }
        public bool labd_FueraRango { get; set; }
    }
}
