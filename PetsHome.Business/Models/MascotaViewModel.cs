using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetsHome.Common.InternalEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para una mascota.
    /// </summary>
    public partial class MascotaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la mascota.
        /// </summary>
        [Key]
        [Display(Name = "Id mascota")]
        public int masc_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la imagen de la mascota en formato byte array.
        /// </summary>
        public byte[] masc_Imagen { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la mascota.
        /// </summary>
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string masc_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la raza de la mascota.
        /// </summary>
        [Display(Name = "Id raza")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? raza_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la raza de la mascota.
        /// </summary>
        [Display(Name = "Raza")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string? raza_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la edad de la mascota.
        /// </summary>
        [Display(Name = "Edad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? masc_Edad { get; set; }

        /// <summary>
        /// Obtiene o establece el sexo de la mascota.
        /// </summary>
        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string masc_Sexo { get; set; }

        /// <summary>
        /// Obtiene o establece el peso de la mascota.
        /// </summary>
        [Display(Name = "Peso")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? masc_Peso { get; set; }

        /// <summary>
        /// Obtiene o establece la talla de la mascota.
        /// </summary>
        [Display(Name = "Talla")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? masc_Talla { get; set; }

        /// <summary>
        /// Obtiene o establece el color de la mascota.
        /// </summary>
        [Display(Name = "Color")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string masc_Color { get; set; }

        /// <summary>
        /// Obtiene o establece la historia de la mascota.
        /// </summary>
        [Display(Name = "Historia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500)]
        public string masc_Historia { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del refugio de la mascota.
        /// </summary>
        [Display(Name = "Id refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del refugio de la mascota.
        /// </summary>
        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string? refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la procedencia de la mascota.
        /// </summary>
        [Display(Name = "Id procedencia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? proc_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la procedencia de la mascota.
        /// </summary>
        [Display(Name = "Procedencia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string? proc_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la mascota ha sido adoptada.
        /// </summary>
        public bool? masc_EsAdoptado { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la mascota ha sido reservada.
        /// </summary>
        public bool? masc_EsReservado { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la mascota.
        /// </summary>
        public int masc_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la mascota.
        /// </summary>
        public DateTime masc_masc_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la mascota.
        /// </summary>
        public int? masc_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la mascota.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? masc_FechaModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la mascota.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la mascota.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? masc_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fila de la mascota.
        /// </summary>
        public long? masc_Fila { get; set; }

        /// <summary>
        /// Obtiene o establece el archivo de imagen de la mascota.
        /// </summary>
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// Obtiene o establece la ruta de la imagen de la mascota.
        /// </summary>
        public string pathMascotaImage { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.masc_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        /// <summary>
        /// Obtiene o establece la lista desplegable de razas.
        /// </summary>
        public SelectList razaList { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de sexos.
        /// </summary>
        public SelectList sexoList { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de refugios.
        /// </summary>
        public SelectList refugioList { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de procedencias.
        /// </summary>
        public SelectList procedenciaList { get; set; }

        /// <summary>
        /// Carga las listas desplegables con los datos proporcionados.
        /// </summary>
        /// <param name="razaDropdownResults">Resultados de la lista desplegable de razas.</param>
        /// <param name="dropdownlists">Listas desplegables.</param>
        /// <param name="refugioDropdownResults">Resultados de la lista desplegable de refugios.</param>
        /// <param name="procedenciaDropdownResults">Resultados de la lista desplegable de procedencias.</param>
        public void LoadDropDownList(IEnumerable<RazaViewModel> razaDropdownResults,
                                        IEnumerable<Dropdown> dropdownlists,
                                        IEnumerable<RefugioViewModel> refugioDropdownResults,
                                        IEnumerable<ProcedenciaViewModel> procedenciaDropdownResults)
        {
            razaList = new SelectList(razaDropdownResults, "raza_Id", "raza_Descripcion");
            sexoList = new SelectList(dropdownlists, "Value", "Text");
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
            procedenciaList = new SelectList(procedenciaDropdownResults, "proc_Id", "proc_Descripcion");
        }

        #endregion Dropdown
    }
}