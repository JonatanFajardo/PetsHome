using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para listar municipios en el Index/DataTable.
    /// </summary>
    public class MunicipioListViewModel
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
    }
}
