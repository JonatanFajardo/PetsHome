using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista para un refugio.
    /// </summary>
    public partial class RefugioViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del refugio.
        /// </summary>
        [Key]
        [Display(Name = "Id")]
        public int refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del refugio.
        /// </summary>
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece la ubicación del refugio.
        /// </summary>
        [Display(Name = "Ubicacion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string refg_Ubicacion { get; set; }

        /// <summary>
        /// Obtiene o establece el RTN del refugio.
        /// </summary>
        [Display(Name = "RTN")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(14)]
        public string refg_RTN { get; set; }

        /// <summary>
        /// Obtiene o establece el teléfono del refugio.
        /// </summary>
        [Display(Name = "Telefono")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(8)]
        public string refg_Telefono { get; set; }

        /// <summary>
        /// Obtiene o establece el correo del refugio.
        /// </summary>
        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string refg_Correo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del departamento del refugio.
        /// </summary>
        [Display(Name = "Departamento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string depto_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del municipio del refugio.
        /// </summary>
        [Display(Name = "Municipio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string mpio_Id { get; set; }

        /// <summary>
        /// Obtiene o establece información adicional sobre el refugio.
        /// </summary>
        [Display(Name = "InformacionAdicional")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500)]
        public string refg_InformacionAdicional { get; set; }

        /// <summary>
        /// Obtiene o establece si el refugio está activo.
        /// </summary>
        public bool refg_EsActivo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el refugio.
        /// </summary>
        public int refg_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el refugio.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? refg_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del refugio.
        /// </summary>
        [Display(Name = "Fecha creación")]
        public DateTime refg_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el refugio.
        /// </summary>
        public int? refg_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el refugio.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? refg_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del refugio.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? refg_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.refg_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        /// <summary>
        /// Obtiene o establece la lista desplegable de departamentos.
        /// </summary>
        public SelectList departamentoList { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de municipios.
        /// </summary>
        public SelectList municipioList { get; set; }

        /// <summary>
        /// Carga las listas desplegables de departamentos y municipios.
        /// </summary>
        /// <param name="departamentoResults">Resultados de departamentos.</param>
        /// <param name="municipioResults">Resultados de municipios.</param>
        public void LoadDropDownList(IEnumerable<DepartamentoViewModel> departamentoResults, IEnumerable<MunicipioViewModel> municipioResults)
        {
            departamentoList = new SelectList(departamentoResults, "depto_Id", "depto_Descripcion");
            municipioList = new SelectList(municipioResults, "mpio_Id", "mpio_Descripcion");
        }

        #endregion Dropdown
    }
}