using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    // ─── Lista (DataTable) ─────────────────────────
    public partial class ProveedoresListViewModel
    {
        public int prov_Id { get; set; }
        public string prov_Nombre { get; set; }
        public string prov_Email { get; set; }
        public string prov_Telefono { get; set; }
        public string prov_EsActivo { get; set; }
    }

    // ─── Formulario Create/Edit ────────────────────
    public partial class ProveedoresFormViewModel
    {
        [Display(Name = "Id")]
        public int prov_Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string prov_Nombre { get; set; }

        [Display(Name = "Correo")]
        [EmailAddress(ErrorMessage = "Ingrese un correo valido")]
        [StringLength(150)]
        public string prov_Email { get; set; }

        [Display(Name = "Telefono")]
        [StringLength(40)]
        public string prov_Telefono { get; set; }

        [Display(Name = "Direccion")]
        [StringLength(250)]
        public string prov_Direccion { get; set; }

        [Display(Name = "Notas")]
        [StringLength(500)]
        public string prov_Notas { get; set; }

        [Display(Name = "Estado")]
        public bool prov_EsActivo { get; set; } = true;

        public bool isEdit => prov_Id > 0;
    }

    // ─── Busqueda (para pre-llenar el form en edicion) ─
    public partial class ProveedoresFindViewModel
    {
        public int prov_Id { get; set; }
        public string prov_Nombre { get; set; }
        public string prov_Email { get; set; }
        public string prov_Telefono { get; set; }
        public string prov_Direccion { get; set; }
        public string prov_Notas { get; set; }
        public bool prov_EsActivo { get; set; }
    }

    // ─── Detalle (read-only) ───────────────────────
    public partial class ProveedoresDetailViewModel
    {
        public int prov_Id { get; set; }
        public string prov_Nombre { get; set; }
        public string prov_Email { get; set; }
        public string prov_Telefono { get; set; }
        public string prov_Direccion { get; set; }
        public string prov_Notas { get; set; }
        public string prov_EsActivo { get; set; }

        public string UsuarioCreacion { get; set; }
        public DateTime prov_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? prov_FechaModifica { get; set; }
    }

    // ─── Dropdown ─────────────────────────────────
    public partial class ProveedoresDropdownViewModel
    {
        public int prov_Id { get; set; }
        public string Descripcion { get; set; }
    }
}
