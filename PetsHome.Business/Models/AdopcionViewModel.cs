using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para una adopción.
    /// </summary>
    public partial class AdopcionViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la ficha de adopción.
        /// </summary>
        [Key]
        [Display(Name = "Id Ficha")]
        public int adop_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la solicitud.
        /// </summary>
        [Display(Name = "Id solicitud")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int sol_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro de la adopción.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "date")]
        public DateTime adop_FechaRegistro { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la adopción está aprobada.
        /// </summary>
        [Display(Name = "Es aprobado")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string adop_Estado { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la adopción.
        /// </summary>
        public int adop_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la adopción.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? adop_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la adopción.
        /// </summary>
        public DateTime adop_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la adopción.
        /// </summary>
        public int? adop_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la adopción.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? adop_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la adopción.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? adop_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.adop_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre de la mascota.
        /// </summary>
        [Display(Name = "Mascota")]
        public string? masc_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del adoptante.
        /// </summary>
        [Display(Name = "Adoptante")]
        public string? per_Nombre { get; set; }

        // Campos de la nueva lista de adopciones (mascotas + conteo solicitantes)

        [Display(Name = "Id Mascota")]
        public int masc_Id { get; set; }

        [Display(Name = "Mascota")]
        public string? raza_Descripcion { get; set; }

        [Display(Name = "Tipo")]
        public string? raza_TipoAnimal { get; set; }

        [Display(Name = "Edad")]
        public int masc_Edad { get; set; }

        [Display(Name = "Sexo")]
        public string? masc_Sexo { get; set; }

        [Display(Name = "Adoptado")]
        public bool masc_EsAdoptado { get; set; }

        [Display(Name = "Reservado")]
        public bool masc_EsReservado { get; set; }

        [Display(Name = "Solicitantes")]
        public int CantidadSolicitantes { get; set; }
    }
}
