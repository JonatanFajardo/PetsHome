using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para listar recepciones de mercancía en el Index/DataTable.
    /// </summary>
    public class RecepcionMercanciaListViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la recepción.
        /// </summary>
        [Display(Name = "Id")]
        public int recep_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la recepción.
        /// </summary>
        [Display(Name = "Descripción")]
        public string recep_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la recepción.
        /// </summary>
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime recep_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del refugio.
        /// </summary>
        [Display(Name = "Id Refugio")]
        public int refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del refugio.
        /// </summary>
        [Display(Name = "Refugio")]
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de recepción.
        /// </summary>
        [Display(Name = "Tipo")]
        public string recep_TipoRecepcion { get; set; }

        /// <summary>
        /// Obtiene o establece el número de documento.
        /// </summary>
        [Display(Name = "Documento")]
        public string recep_NumeroDocumento { get; set; }
    }
}
