using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para una donación.
    /// </summary>
    public partial class DonacionViewModel
    {
        public DonacionViewModel()
        {
            dona_FechaDonacion = DateTime.Now.Date;
            dona_Estado = "Recibida"; // Estado por defecto
        }
        /// <summary>
        /// Obtiene o establece el ID de la donación.
        /// </summary>
        [Key]
        [Display(Name = "Id Donación")]
        public int dona_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de donación.
        /// </summary>
        [Display(Name = "Tipo de Donación")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string dona_TipoDonacion { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del donante.
        /// </summary>
        [Display(Name = "Nombre del Donante")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string dona_NombreDonante { get; set; }

        /// <summary>
        /// Obtiene o establece el teléfono del donante.
        /// </summary>
        [Display(Name = "Teléfono")]
        [StringLength(15, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string dona_TelefonoDonante { get; set; }

        /// <summary>
        /// Obtiene o establece el email del donante.
        /// </summary>
        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string dona_EmailDonante { get; set; }

        /// <summary>
        /// Obtiene o establece el monto monetario de la donación.
        /// </summary>
        [Display(Name = "Monto Monetario")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99, ErrorMessage = "El {0} debe estar entre {1} y {2}")]
        public decimal? dona_MontoMonetario { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de artículos donados.
        /// </summary>
        [Display(Name = "Descripción de Artículos")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string dona_DescripcionArticulos { get; set; }

        /// <summary>
        /// Obtiene o establece el valor estimado de artículos donados.
        /// </summary>
        [Display(Name = "Valor Estimado")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99, ErrorMessage = "El {0} debe estar entre {1} y {2}")]
        public decimal? dona_ValorEstimado { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la donación.
        /// </summary>
        [Display(Name = "Fecha de Donación")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime dona_FechaDonacion { get; set; }

        /// <summary>
        /// Obtiene o establece el estado de la donación.
        /// </summary>
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(30, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string dona_Estado { get; set; }

        /// <summary>
        /// Obtiene o establece las observaciones de la donación.
        /// </summary>
        [Display(Name = "Observaciones")]
        [StringLength(1000, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string dona_Observaciones { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del refugio.
        /// </summary>
        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del refugio.
        /// </summary>
        [Display(Name = "Refugio")]
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la donación.
        /// </summary>
        public int dona_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la donación.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string dona_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la donación.
        /// </summary>
        [Display(Name = "Fecha creación")]
        public DateTime dona_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la donación.
        /// </summary>
        public int? dona_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la donación.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string dona_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la donación.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? dona_FechaModifica { get; set; }

        /// <summary>
        /// Lista de refugios para dropdown.
        /// </summary>
        public List<SelectListItem> RefugiosDropdown { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Lista de tipos de donación para dropdown.
        /// </summary>
        public List<SelectListItem> TiposDonacionDropdown { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Lista de estados para dropdown.
        /// </summary>
        public List<SelectListItem> EstadosDropdown { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                return this.dona_Id != 0;
            }
        }

        /// <summary>
        /// Carga las listas desplegables.
        /// </summary>
        public void LoadDropDownList(List<SelectListItem> refugios, List<SelectListItem> tiposDonacion, List<SelectListItem> estados)
        {
            RefugiosDropdown = refugios ?? new List<SelectListItem>();
            TiposDonacionDropdown = tiposDonacion ?? new List<SelectListItem>();
            EstadosDropdown = estados ?? new List<SelectListItem>();
        }
    }
}