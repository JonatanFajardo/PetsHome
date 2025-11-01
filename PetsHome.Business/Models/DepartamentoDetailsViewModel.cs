using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para mostrar detalles completos de un departamento.
    /// </summary>
    public class DepartamentoDetailsViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del departamento.
        /// </summary>
        [Display(Name = "Id")]
        public int depto_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código del departamento.
        /// </summary>
        [Display(Name = "Código")]
        public string depto_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del departamento.
        /// </summary>
        [Display(Name = "Departamento")]
        public string depto_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la capital del departamento.
        /// </summary>
        [Display(Name = "Capital")]
        public string depto_Capital { get; set; }

        /// <summary>
        /// Obtiene o establece la población del departamento.
        /// </summary>
        [Display(Name = "Población")]
        public int? depto_Poblacion { get; set; }

        /// <summary>
        /// Obtiene o establece el área en km2 del departamento.
        /// </summary>
        [Display(Name = "Área (km²)")]
        public decimal? depto_AreaKm2 { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que creó el departamento.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string UsuarioCreacion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del departamento.
        /// </summary>
        [Display(Name = "Fecha creación")]
        public DateTime? depto_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el departamento.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string UsuarioModificacion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del departamento.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? depto_FechaModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la lista de municipios asociados al departamento.
        /// </summary>
        public List<MunicipioDetailsViewModel> ListadoMunicipios { get; set; }
    }
}
