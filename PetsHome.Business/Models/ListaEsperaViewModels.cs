using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetsHome.Business.Models
{
    // ─── Lista (DataTable) ─────────────────────────
    public partial class ListaEsperaListViewModel
    {
        public int lesp_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string Telefono { get; set; }
        public int? tipoCon_Id { get; set; }
        public string TipoConsulta { get; set; }
        public DateTime? lesp_FechaDeseada { get; set; }
        public string lesp_Prioridad { get; set; }
        public string lesp_Estado { get; set; }
        public string lesp_Notas { get; set; }
        public DateTime lesp_FechaCrea { get; set; }
        public int DiasEsperando { get; set; }
    }

    // ─── Formulario Create/Edit ────────────────────
    public partial class ListaEsperaFormViewModel
    {
        public int lesp_Id { get; set; }

        [Required(ErrorMessage = "Seleccione la mascota.")]
        public int masc_Id { get; set; }

        public int? cli_Id { get; set; }
        public int? tipoCon_Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime? lesp_FechaDeseada { get; set; }

        [Required(ErrorMessage = "Seleccione la prioridad.")]
        public string lesp_Prioridad { get; set; } = "Normal";

        public string lesp_Notas { get; set; }

        public bool isEdit => lesp_Id > 0;

        // Dropdowns de apoyo (no se persisten)
        public List<MascotaDropdownViewModel> MascotaList { get; set; } = new List<MascotaDropdownViewModel>();
        public List<TipoConsultaViewModel> TipoConsultaList { get; set; } = new List<TipoConsultaViewModel>();

        public List<SelectListItem> PrioridadList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem("Normal", "Normal"),
            new SelectListItem("Alta", "Alta"),
            new SelectListItem("Urgente", "Urgente"),
        };
    }

    // ─── Busqueda (pre-llenar el form en edicion) ───
    public partial class ListaEsperaFindViewModel
    {
        public int lesp_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cli_Id { get; set; }
        public int? tipoCon_Id { get; set; }
        public DateTime? lesp_FechaDeseada { get; set; }
        public string lesp_Prioridad { get; set; }
        public string lesp_Estado { get; set; }
        public string lesp_Notas { get; set; }
    }

    // ─── Detalle (read-only) ───────────────────────
    public partial class ListaEsperaDetailViewModel
    {
        public int lesp_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string ResponsableContacto { get; set; }
        public int? tipoCon_Id { get; set; }
        public string TipoConsulta { get; set; }
        public DateTime? lesp_FechaDeseada { get; set; }
        public string lesp_Prioridad { get; set; }
        public string lesp_Estado { get; set; }
        public string lesp_Notas { get; set; }
        public int DiasEsperando { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime lesp_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? lesp_FechaModifica { get; set; }
    }

    // ─── Sugerencia "¿a quien llamar?" para un hueco ─
    public partial class ListaEsperaSugerenciaViewModel
    {
        public int Fila { get; set; }
        public int lesp_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string Telefono { get; set; }
        public int? tipoCon_Id { get; set; }
        public string TipoConsulta { get; set; }
        public DateTime? lesp_FechaDeseada { get; set; }
        public string lesp_Prioridad { get; set; }
        public DateTime lesp_FechaCrea { get; set; }
        public int DiasEsperando { get; set; }
    }
}
