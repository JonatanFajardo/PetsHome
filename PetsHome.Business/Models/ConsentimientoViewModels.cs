using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetsHome.Business.Models
{
    // ─── Lista (DataTable) ─────────────────────────
    public partial class ConsentimientoListViewModel
    {
        public int cons_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public int? cita_Id { get; set; }
        public string cons_Tipo { get; set; }
        public string cons_Estado { get; set; }
        public string cons_FirmanteNombre { get; set; }
        public DateTime? cons_FechaFirma { get; set; }
        public DateTime cons_FechaCrea { get; set; }
    }

    // ─── Formulario Create/Edit ────────────────────
    public partial class ConsentimientoFormViewModel
    {
        public int cons_Id { get; set; }

        [Required(ErrorMessage = "Seleccione la mascota.")]
        public int masc_Id { get; set; }

        public int? cli_Id { get; set; }
        public int? cita_Id { get; set; }

        [Required(ErrorMessage = "Seleccione el tipo de consentimiento.")]
        public string cons_Tipo { get; set; }

        [Required(ErrorMessage = "Indique el texto del consentimiento.")]
        public string cons_Texto { get; set; }

        public string cons_Notas { get; set; }

        public bool isEdit => cons_Id > 0;

        // Dropdowns de apoyo (no se persisten)
        public List<MascotaDropdownViewModel> MascotaList { get; set; } = new List<MascotaDropdownViewModel>();

        public List<SelectListItem> TipoList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem("General", "General"),
            new SelectListItem("Cirugía", "Cirugia"),
            new SelectListItem("Anestesia", "Anestesia"),
            new SelectListItem("Eutanasia", "Eutanasia"),
        };
    }

    // ─── Busqueda (pre-llenar el form en edicion) ───
    public partial class ConsentimientoFindViewModel
    {
        public int cons_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cli_Id { get; set; }
        public int? cita_Id { get; set; }
        public string cons_Tipo { get; set; }
        public string cons_Estado { get; set; }
        public string cons_Texto { get; set; }
        public string cons_Notas { get; set; }
    }

    // ─── Detalle (read-only) ───────────────────────
    public partial class ConsentimientoDetailViewModel
    {
        public int cons_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string ResponsableContacto { get; set; }
        public int? cita_Id { get; set; }
        public string cons_Tipo { get; set; }
        public string cons_Estado { get; set; }
        public string cons_Texto { get; set; }
        public string cons_FirmanteNombre { get; set; }
        public DateTime? cons_FechaFirma { get; set; }
        public string cons_Notas { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime cons_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? cons_FechaModifica { get; set; }
    }
}
