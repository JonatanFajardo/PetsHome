using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para mostrar los detalles de una raza.
    /// </summary>
    public class RazaDetailsViewModel
    {
        [Display(Name = "Id raza")]
        public int raza_Id { get; set; }

        [Display(Name = "Descripción")]
        public string raza_Descripcion { get; set; }

        [Display(Name = "Tamaño")]
        public string raza_Tamano { get; set; }

        [Display(Name = "Tipo de Animal")]
        public string raza_TipoAnimal { get; set; }

        [Display(Name = "Tipo de Pelaje")]
        public string raza_TipoPelaje { get; set; }

        [Display(Name = "URL de Imagen")]
        public string raza_ImagenUrl { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime raza_FechaCrea { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? raza_FechaModifica { get; set; }
    }
}
