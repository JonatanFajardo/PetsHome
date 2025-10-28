using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para mostrar detalles completos de un municipio.
    /// </summary>
    public class MunicipioDetailsViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del municipio.
        /// </summary>
        [Display(Name = "Id")]
        public int mpio_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código del municipio.
        /// </summary>
        [Display(Name = "Código")]
        public string mpio_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del municipio.
        /// </summary>
        [Display(Name = "Municipio")]
        public string mpio_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del departamento al que pertenece.
        /// </summary>
        [Display(Name = "Id Departamento")]
        public int depto_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código del departamento al que pertenece.
        /// </summary>
        [Display(Name = "Código Departamento")]
        public string depto_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que creó el municipio.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string UsuarioCreacion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del municipio.
        /// </summary>
        [Display(Name = "Fecha creación")]
        public DateTime? mpio_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el municipio.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string UsuarioModificacion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del municipio.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? mpio_FechaModifica { get; set; }
    }
}
