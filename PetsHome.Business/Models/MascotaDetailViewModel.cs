using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PetsHome.Business.Models
{
    public class MascotaDetailViewModel
    {
        [Display(Name = "ID")]
        public int masc_Id { get; set; }

        [Display(Name = "Imagen")]
        [DataType(DataType.ImageUrl)]
        public string masc_Imagen { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string masc_Nombre { get; set; }

        [Display(Name = "Raza")]
        [Required(ErrorMessage = "La raza es obligatoria")]
        [StringLength(100, ErrorMessage = "La descripción de la raza no puede exceder 100 caracteres")]
        public string raza_Descripcion { get; set; }

        [Display(Name = "Edad")]
        [Required(ErrorMessage = "La edad es obligatoria")]
        [Range(0, 30, ErrorMessage = "La edad debe estar entre 0 y 30 años")]
        public int masc_Edad { get; set; }

        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El sexo es obligatorio")]
        [StringLength(1, ErrorMessage = "El sexo debe ser un carácter")]
        [RegularExpression("^[MF]$", ErrorMessage = "El sexo debe ser M (Macho) o F (Hembra)")]
        public string masc_Sexo { get; set; }

        [Display(Name = "Peso (kg)")]
        [Required(ErrorMessage = "El peso es obligatorio")]
        [Range(0.1, 200.0, ErrorMessage = "El peso debe estar entre 0.1 y 200 kg")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal masc_Peso { get; set; }

        [Display(Name = "Color")]
        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(50, ErrorMessage = "El color no puede exceder 50 caracteres")]
        public string masc_Color { get; set; }

        [Display(Name = "Historia")]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, ErrorMessage = "La historia no puede exceder 1000 caracteres")]
        public string masc_Historia { get; set; }

        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El refugio es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre del refugio no puede exceder 100 caracteres")]
        public string refg_Nombre { get; set; }

        [Display(Name = "Procedencia")]
        [Required(ErrorMessage = "La procedencia es obligatoria")]
        [StringLength(100, ErrorMessage = "La procedencia no puede exceder 100 caracteres")]
        public string proc_Descripcion { get; set; }

        [Display(Name = "¿Está Adoptado?")]
        public bool masc_EsAdoptado { get; set; }

        [Display(Name = "¿Está Reservado?")]
        public bool masc_EsReservado { get; set; }

        [Display(Name = "Usuario Creación")]
        [StringLength(50, ErrorMessage = "El nombre del usuario no puede exceder 50 caracteres")]
        public string UsuarioCreacion { get; set; }

        [Display(Name = "Fecha Creación")]
        [DataType(DataType.DateTime)]
        public DateTime masc_FechaCrea { get; set; }

        [Display(Name = "Usuario Modificación")]
        [StringLength(50, ErrorMessage = "El nombre del usuario no puede exceder 50 caracteres")]
        public string UsuarioModificacion { get; set; }

        [Display(Name = "Fecha Modificación")]
        [DataType(DataType.DateTime)]
        public DateTime? masc_FechaModifica { get; set; }

        // Propiedades computadas para la vista
        [Display(Name = "Sexo Descripción")]
        public string SexoDescripcion => masc_Sexo == "M" ? "Macho" : "Hembra";

        [Display(Name = "Estado")]
        public string EstadoDescripcion
        {
            get
            {
                if (masc_EsAdoptado)
                    return "Adoptado";
                else if (masc_EsReservado)
                    return "Reservado";
                else
                    return "Disponible para Adopción";
            }
        }

        [Display(Name = "Disponible para Adopción")]
        public bool EstaDisponible => !masc_EsAdoptado && !masc_EsReservado;

        [Display(Name = "Tiene Imagen")]
        public bool TieneImagen => !string.IsNullOrEmpty(masc_Imagen);

        [Display(Name = "Peso Formateado")]
        public string PesoFormateado => $"{masc_Peso:F1} kg";

        [Display(Name = "Edad Formateada")]
        public string EdadFormateada
        {
            get
            {
                if (masc_Edad == 0)
                    return "Menos de 1 año";
                else if (masc_Edad == 1)
                    return "1 año";
                else
                    return $"{masc_Edad} años";
            }
        }

        [Display(Name = "Última Actualización")]
        public string UltimaActualizacion => masc_FechaModifica?.ToString("dd/MM/yyyy") ?? "N/A";

        [Display(Name = "Fecha Registro")]
        public string FechaRegistro => masc_FechaCrea.ToString("dd/MM/yyyy HH:mm");

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la mascota.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? masc_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la mascota.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? masc_NombreUsuarioModifica { get; set; }

        // Constructor
        public MascotaDetailViewModel()
        {
            masc_FechaCrea = DateTime.Now;
            masc_EsAdoptado = false;
            masc_EsReservado = false;
        }
    }
}
