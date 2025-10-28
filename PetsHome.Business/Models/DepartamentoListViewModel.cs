using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para listar departamentos en el Index/DataTable.
    /// </summary>
    public class DepartamentoListViewModel
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
    }
}
