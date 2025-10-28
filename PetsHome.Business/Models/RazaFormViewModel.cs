using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para crear o editar razas.
    /// </summary>
    public class RazaFormViewModel
    {
        public long? Fila { get; set; }

        [Display(Name = "Id raza")]
        public int raza_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        [Espacios(ErrorMessage = "bebesita")]
        public string raza_Descripcion { get; set; }

        [Display(Name = "Tamaño")]
        [StringLength(20)]
        public string raza_Tamano { get; set; }

        [Display(Name = "Tipo de Animal")]
        [StringLength(50)]
        public string raza_TipoAnimal { get; set; }

        [Display(Name = "Tipo de Pelaje")]
        [StringLength(30)]
        public string raza_TipoPelaje { get; set; }

        [Display(Name = "URL de Imagen")]
        [StringLength(500)]
        public string raza_ImagenUrl { get; set; }

        public int raza_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string raza_NombreUsuarioCrea { get; set; }

        public DateTime raza_FechaCrea { get; set; }

        public int? raza_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string raza_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? raza_FechaModifica { get; set; }

        public bool isEdit => raza_Id != 0;
    }

    /// <summary>
    /// Validación personalizada utilizada en el formulario de raza.
    /// </summary>
    public class EspaciosAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string texto && texto.Length > 0)
            {
                return texto[0] != ' ';
            }
            return true;
        }
    }
}
