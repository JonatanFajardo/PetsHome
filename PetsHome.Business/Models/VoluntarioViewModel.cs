﻿using System;
using System.ComponentModel.DataAnnotations;
namespace PetsHome.Business.Models
{
    public partial class VoluntarioViewModel
    {
        [Key]
        [Display(Name = "Id voluntario")]
        public int vol_Id { get; set; }

        [Display(Name = "Horas Trabajadas")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? vol_HorasTrabajadas { get; set; }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? per_Id { get; set; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public int? vol_Recurrente { get; set; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_Apellidos { get; set; }
        public int vol_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? vol_NombreUsuarioCrea { get; set; }
        public DateTime vol_FechaCrea { get; set; }
        public int? vol_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? vol_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? vol_FechaModifica { get; set; }

        #region Dropdown

        #endregion Dropdown
    }
}