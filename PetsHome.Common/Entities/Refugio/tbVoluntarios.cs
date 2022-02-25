﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    public partial class tbVoluntarios
    {
        public tbVoluntarios()
        {
            tbEventos_tbVoluntarios = new HashSet<tbEventos_tbVoluntarios>();
        }

        /// <summary>
        /// Identificador único de la tabla Voluntarios.
        /// </summary>
        public int vol_Id { get; set; }
        public int vol_HorasTrabajadas { get; set; }
        public int per_Id { get; set; }
        public bool? vol_Recurrente { get; set; }
        /// <summary>
        /// Indica si el registro está desactivado permanentemente.
        /// </summary>
        public bool vol_EsEliminado { get; set; }
        public int vol_UsuarioCrea { get; set; }
        /// <summary>
        /// Registra la fecha en que se creó el registro.
        /// </summary>
        public DateTime vol_FechaCrea { get; set; }
        /// <summary>
        /// Indica el identificador del último usuario que modificó el registro.
        /// </summary>
        public int? vol_UsuarioModifica { get; set; }
        /// <summary>
        /// Registra la última fecha en que se modificó el registro.
        /// </summary>
        public DateTime? vol_FechaModifica { get; set; }

        public virtual tbPersonas per { get; set; }
        public virtual tbUsuarios vol_UsuarioCreaNavigation { get; set; }
        public virtual ICollection<tbEventos_tbVoluntarios> tbEventos_tbVoluntarios { get; set; }
    }
}