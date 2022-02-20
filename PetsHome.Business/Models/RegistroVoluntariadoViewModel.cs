using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{

    public partial class EventoViewModel
    {
        [Key]
        [Display(Name = "Id evento")]
        public int eve_Id { get; set; }

        [Display(Name = "Id voluntario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? vol_Id { get; set; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_Nombres { get; set; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_Apellidos { get; set; }

        [Display(Name = "Hora inicio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "time(2)")]
        public TimeSpan? rvo_HoraInicio { get; set; }

        [Display(Name = "Hora final")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "time(2)")]
        public TimeSpan? rvo_HoraFinal { get; set; }

        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "date")]
        public DateTime? rvo_Fecha { get; set; }

        public DateTime? fechaCreacion { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? fechaModificacion { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.eve_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        #endregion Dropdown

    }
}