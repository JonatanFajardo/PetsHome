using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para mostrar el detalle de un refugio.
    /// </summary>
    public class RefugioDetailsViewModel
    {
        [Display(Name = "Id")]
        public int refg_Id { get; set; }

        [Display(Name = "Nombre")]
        public string refg_Nombre { get; set; }

        [Display(Name = "Ubicacion")]
        public string refg_Ubicacion { get; set; }

        [Display(Name = "RTN")]
        public string refg_RTN { get; set; }

        [Display(Name = "Telefono")]
        public string refg_Telefono { get; set; }

        [Display(Name = "Correo")]
        public string refg_Correo { get; set; }

        [Display(Name = "Departamento")]
        public int depto_Id { get; set; }

        [Display(Name = "Municipio")]
        public int mpio_Id { get; set; }

        [Display(Name = "InformacionAdicional")]
        public string refg_InformacionAdicional { get; set; }

        public string EsActivo { get; set; }

        public bool refg_EsActivo => string.Equals(EsActivo, "Activo", StringComparison.OrdinalIgnoreCase);

        public int refg_UsuarioCrea { get; set; }

        public string refg_NombreUsuarioCrea { get; set; }

        public DateTime refg_FechaCrea { get; set; }

        public int? refg_UsuarioModifica { get; set; }

        public string refg_NombreUsuarioModifica { get; set; }

        public DateTime? refg_FechaModifica { get; set; }
    }
}
